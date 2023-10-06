using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ObjectDataFactory
{
    public static ObjectData GetPlayer()
    {
        var playerObjectData = ItemDataBase.GetObject("Ciclops");

        if (Application.isPlaying)
        {
            var playerDynamicData = SaveLoader.LoadPlayerData();
            if (playerDynamicData != null)
            {
               playerObjectData = playerObjectData.Clone() as ObjectData;
               playerObjectData.dynamicData = playerDynamicData;
               
            }

        }

        return playerObjectData;
        
    }
    public static ObjectData GetCreature(string name,
        string itemInMainHand = "",
        string itemInOffHand = "",
        string itemOnHead = "",
        string itemOnChest = "",
        string customName = "",
        List<string> itemsInInventory = null,
        List<Tag> enemyTags = null)
    {

        var creatureData = ItemDataBase.GetObject(name).Clone() as ObjectData;
        if(itemInMainHand != "") creatureData.dynamicData.itemInMainHand = (ItemDataBase.GetObject(itemInMainHand).Clone() as ObjectData).dynamicData;
        if(itemInOffHand != "") creatureData.dynamicData.itemInOffHand = (ItemDataBase.GetObject(itemInOffHand).Clone() as ObjectData).dynamicData;
        if(itemOnHead != "") creatureData.dynamicData.itemOnHead = (ItemDataBase.GetObject(itemOnHead).Clone() as ObjectData).dynamicData;
        if(itemOnChest != "") creatureData.dynamicData.itemOnChest = (ItemDataBase.GetObject(itemOnChest).Clone() as ObjectData).dynamicData;
        if(customName != "") creatureData.staticData.realName = customName;
        if (enemyTags != null) creatureData.dynamicData.enemyTags = enemyTags;

        if(itemsInInventory != null)
        {
            foreach (var item in itemsInInventory)
            {
                var itemData = ItemDataBase.GetObject(item);
                creatureData.dynamicData.itemsInInventory.Add(itemData.dynamicData);
   
            }
        }

        return creatureData;
    }
    public static ObjectData GetRandomCreature(int level, Biome biome, List<Tag> tags = null, bool withRandomEquip = false, string name = "")
    {
        if (tags == null) tags = new List<Tag>();
        tags.Add(Tag.Creature);

        var creature = new ObjectData();

        if (name == "")
        {
            creature = ItemDataBase.GetRandomItem(level, biome, tags).objectData.Clone() as ObjectData;
        }
        else
        {
            creature = ItemDataBase.GetObject(name).Clone() as ObjectData;
        }

        if (creature.dynamicData.itemInMainHand == null
            && BaseMethodsClass.Chance(80)
            && withRandomEquip 
            && (creature.staticData.allowedWeaponTags.Count>0
            || creature.staticData.allowedWeaponTags.Contains(Tag.Any)))
        {
            var weaponTags = creature.staticData.allowedWeaponTags;
            var itemInMainHand = (ItemDataBase.GetRandomItem(level, biome, weaponTags).objectData)
                .dynamicData;
            creature.dynamicData.itemInMainHand = itemInMainHand; 
            if (creature.dynamicData.itemInOffHand == null && BaseMethodsClass.Chance(5) )
            {
                var itemInOffHand = (ItemDataBase.GetRandomItem(level, biome, new List<Tag> {Tag.Shield}).objectData).dynamicData;
                creature.dynamicData.itemInOffHand = itemInOffHand;
            }
        }

        if(creature.dynamicData.itemOnHead == null
            && withRandomEquip && BaseMethodsClass.Chance(30)
            && (creature.staticData.allowedArmorTags.Contains(Tag.Headwear) 
            || creature.staticData.allowedArmorTags.Contains(Tag.Any)))
        {
            var itemOnHead = (ItemDataBase.GetRandomItem(level, biome, new List<Tag> {Tag.Headwear}).objectData).dynamicData;
            creature.dynamicData.itemOnHead = itemOnHead;
        }
        if(creature.dynamicData.itemOnChest == null
            && withRandomEquip && BaseMethodsClass.Chance(30) 
            && (creature.staticData.allowedArmorTags.Contains(Tag.Chestplate) 
            || creature.staticData.allowedArmorTags.Contains(Tag.Any)))
        {
            var itemOnChest = (ItemDataBase.GetRandomItem(level, biome, new List<Tag> {Tag.Chestplate}).objectData).dynamicData;
            creature.dynamicData.itemOnChest = itemOnChest;
        }

        

        return creature;

    }
    

}
