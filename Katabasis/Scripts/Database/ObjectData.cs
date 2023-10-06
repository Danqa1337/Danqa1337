using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class ObjectData : ICloneable
{
    public int id;
    public DynamicObjectData dynamicData;
    public StaticObjectData staticData;
    public ObjectData(Entity_ItemTable.Param param)
    {
        dynamicData = new DynamicObjectData(param);
        staticData = new StaticObjectData(param);
        
        foreach (var name in staticData.partNames)
        {
            dynamicData.bodyParts.Add(ItemDataBase.GetObject(name).dynamicData);
        }

    }

    public ObjectData()
    {

    }

    public object Clone()
    {
        var dynamicClone = dynamicData.Clone() as DynamicObjectData;
        if(dynamicData.itemInMainHand != null)dynamicClone.itemInMainHand = dynamicData.itemInMainHand.Clone() as DynamicObjectData;
        if(dynamicData.itemInOffHand != null)dynamicClone.itemInOffHand = dynamicData.itemInOffHand.Clone() as DynamicObjectData;
        if(dynamicData.itemOnHead != null)dynamicClone.itemOnHead = dynamicData.itemOnHead.Clone() as DynamicObjectData;
        if(dynamicData.itemOnChest != null)dynamicClone.itemOnChest = dynamicData.itemOnChest.Clone() as DynamicObjectData;
        var staticClone = staticData.Clone() as StaticObjectData;

        return new ObjectData() {dynamicData = dynamicClone,staticData = staticClone
            ,
            id = id};
    }

}
[System.Serializable]
public class StaticObjectData : ICloneable
{
    public string name;
    public string realName = "";
    public string description = "";

    public BodyPartTag bodyPartTag;
    public TileState defaultTileState;
    public List<Tag> allowedWeaponTags;
    public List<Tag> allowedArmorTags;
    public Gender gender;
    public List<DropElement> drop = new List<DropElement>();

    public List<Sprite> sprites = new List<Sprite>();
    public List<Sprite> equipSprites = new List<Sprite>();
    public List<EffectOnConsumptionElement> effectOnConsumptionComponents = new List<EffectOnConsumptionElement>();
    public string blood;
    public int MaxReloadingPhases;
    public float aerodynamicsDamageMultiplier;

    public Size size;
    public EquipPice defaultEquipTag;
    public int viewDistance;
    public int baseMovementCost;
    public float fistSharpness;
    public bool hasSeamlessTexture;
    public Vector2 spriteCenterOffset;

    public float2 rightArmHolderOffset;
    public float2 leftArmHolderOffset;
    public float2 headHolderOffset;
    public float2 chestHolderOffset;
    public float2 legsHolderOffset;

    public int XPOnDeath;
    

    public List<string> partNames = new List<string>();
    public StaticObjectData()
    {

    }
    public StaticObjectData(Entity_ItemTable.Param param)
    {

        
        var partsTags = BaseMethodsClass.DecodeCharSeparatedEnums<BodyPartTag>(param.bodyparts);

        string path = "Sprites/";
        if(param.race != "")
        {
            path += param.race;
        }
        else
        {
            path += param.shorthand;

        }
        sprites = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>(path + "Body").ToList();
        if (sprites.Count == 0)
        {
            sprites = Resources.LoadAll<Sprite>(path).ToList(); 
        }

        if(sprites.Count>0)
        {
            var texture = sprites[0].texture;
            var rect = sprites[0].textureRect;
            float pixelNum = 0;
            float pixelSize = 1f / rect.height;
            var positionSum = int2.zero;
            


            for (int y = (int)rect.yMin; y < rect.yMax; y++)
            {
                float xSum = 0;
                
                for (int x = (int)rect.xMin; x < rect.xMax; x++)
                {
                    
                    if (texture.GetPixel(x, y).a > 0)
                    {
                        positionSum += new int2(x, y);
                        pixelNum++;

                    }
                }




                

            }
            var center = new float2(positionSum.x / pixelNum, positionSum.y / pixelNum) * pixelSize;
            spriteCenterOffset =  new float2(0.5f,0.5f) - center;

        }
        if (sprites.Count == 64)
        {
            hasSeamlessTexture = true;
        }
        else
        {
            hasSeamlessTexture = false;
        }

        





        name = param.shorthand;
        realName = param.realName;
        description = param.description;
        
        if(param.gender != "")gender = param.gender.DecodeCharSeparatedEnumsAndGetFirst<Gender>(); 
        defaultEquipTag = param.equipTag.DecodeCharSeparatedEnumsAndGetFirst<EquipPice>();
        defaultTileState = param.defaultState.DecodeCharSeparatedEnumsAndGetFirst<TileState>();

        allowedArmorTags = param.alowedArmorTags.DecodeCharSeparatedEnums<Tag>();
        allowedWeaponTags = param.alowedWeaponTags.DecodeCharSeparatedEnums<Tag>();

        size = (Size)param.size;
        bodyPartTag = BodyPartTag.Body;
        viewDistance = param.viewDistance;
        fistSharpness = param.fistSharpness;
        baseMovementCost = param.movementCost;
        aerodynamicsDamageMultiplier = param.AerodynamicsDamageMultipler;
        MaxReloadingPhases = param.movementCost;

        XPOnDeath = param.expOnDeath;

        if(!string.IsNullOrEmpty(param.drop))
        {
            var drops = param.drop.Split(',');
            
            foreach (var dropString in drops)
            {
                var nameAndChance = dropString.Split('*');
                string dropName = nameAndChance[0];
                float amount = float.Parse(nameAndChance[1]);
                for (float i = amount; i > 0; i -= 100)
                {
                    var dropData = new DropElement(ItemDataBase.GetObject(dropName).id, math.min(i, 100));
                    drop.Add(dropData);

                }




            }
        }


        if (!string.IsNullOrEmpty(param.blood))
        {
            var newBlood = ItemDataBase.GetObject(param.blood);
            newBlood.staticData.sprites = new List<Sprite>();
            newBlood.staticData.sprites = Resources.LoadAll<Sprite>("Sprites/" + param.blood).ToList();
            newBlood.dynamicData.objectType = ObjectType.Liquid;
            newBlood.staticData.name = name + "'s " + param.blood;
            ItemDataBase.RegisterNewObject(newBlood);
            blood = newBlood.staticData.name;
        }

        if (!string.IsNullOrEmpty(param.Offsets))
        {
           var stringOffsets = param.Offsets.Split(' ');
           for (int i = 0; i < stringOffsets.Length; i++)
           {
               var stringCords = stringOffsets[i].Split(',');
               switch (i)
               {
                   case 0:
                       if(float.TryParse(stringCords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out rightArmHolderOffset.x)==false) Debug.Log("can not parse " + stringCords[0]);
                       float.TryParse(stringCords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out rightArmHolderOffset.y);
                       break;
                   case 1:
                       float.TryParse(stringCords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out leftArmHolderOffset.x);
                       float.TryParse(stringCords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out leftArmHolderOffset.y);
                       break; 
                   case 2:
                       float.TryParse(stringCords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out headHolderOffset.x);
                       float.TryParse(stringCords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out headHolderOffset.y);
                       break;
                   case 3:
                       float.TryParse(stringCords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out chestHolderOffset.x);
                       float.TryParse(stringCords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out chestHolderOffset.y);
                       break; 
                   case 4:
                       float.TryParse(stringCords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out legsHolderOffset.x);
                       float.TryParse(stringCords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out legsHolderOffset.y);
                       break;

               }
           }
        }


        if (param.EffectsOnConsumption != "")
        {
            foreach (var effect in param.EffectsOnConsumption.DecodeCharSeparatedEnums<EffectType>())
            {
                effectOnConsumptionComponents.Add(new EffectOnConsumptionElement(effect));
            }
        }
        

        var partCount = partsTags.Count;

        for (int i = 0; i < partCount; i++)
        {
            var objectData = new ObjectData();
            var dynamicData = new DynamicObjectData(param);
            var staticData = new StaticObjectData();

            var partName = this.name + "'s " + partsTags[i];
            staticData.name = partName;
            dynamicData.name = partName;
            staticData.realName = partName;
            staticData.description = "This is " + partName + " that was torn off";
            partNames.Add(partName);


            dynamicData.weight = param.weight * 0.1f;
            dynamicData.resistance = param.resistance;
            dynamicData.tags = new List<Tag>() { };
            dynamicData.maxDurability = (int)(param.maxDurability );
            dynamicData.curDurability = (int)(param.maxDurability );
            if (param.tags.DecodeCharSeparatedEnums<Tag>().Contains(Tag.Food))
            {
                dynamicData.tags.Add(Tag.Food);
            }

            staticData.sprites = new List<Sprite>();
            staticData.sprites = Resources.LoadAll<Sprite>(path + partsTags[i].ToString()).ToList();

            staticData.description = this.name + "'s " + partsTags[i];
            staticData.bodyPartTag = partsTags[i];
            staticData.blood = blood;



            objectData.dynamicData = dynamicData;
            objectData.staticData = staticData;

            ItemDataBase.RegisterNewObject(objectData);

        }



    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
[System.Serializable]
public class DynamicObjectData : ICloneable
{
    public string name = "unnamed";
    public int id;
    public float2 position;

    public List<Tag> tags = new List<Tag>();
    public List<Tag> interestTags = new List<Tag>();
    public List<Tag> enemyTags = new List<Tag>();
    public List<EffectElement> activeEffects = new List<EffectElement>();
    public List<BodyPartTag> missingBodypart = new List<BodyPartTag>();
    public ObjectType objectType;
    public DynamicObjectData itemInMainHand;
    public DynamicObjectData itemInOffHand;
    public DynamicObjectData itemOnHead;
    public DynamicObjectData itemOnChest;
    public List<DynamicObjectData> bodyParts = new List<DynamicObjectData>();
    public List<DynamicObjectData> itemsInInventory = new List<DynamicObjectData>();

    public int worth;
    public float weight;
    public float accuracy;
    public float baseDamage;
    public float resistance;
    public int power;
    public int curentReloadPahase;
    public Scaling scalingSTR = Scaling._;
    public Scaling scalingAGL = Scaling._;

    public int maxDurability;
    public int curDurability;
    public int maxHealth;
    public int curHealth;

    public int STR;
    public int AGL;
    public int rndSpriteNum;
    public int transitionId;
    public bool alive;
    
    public DynamicObjectData(Entity_ItemTable.Param param)
    {
        this.name = param.shorthand;
        this.tags = param.tags.DecodeCharSeparatedEnums<Tag>();
        this.interestTags = param.interestTags.DecodeCharSeparatedEnums<Tag>();
        this.enemyTags = param.enemyTags.DecodeCharSeparatedEnums<Tag>();
        this.objectType = param.objectType.DecodeCharSeparatedEnumsAndGetFirst<ObjectType>();

        this.missingBodypart = new List<BodyPartTag>();
        this.worth = param.worth;
        this.weight = param.weight;
        this.accuracy = (float)param.accuracity;
        this.baseDamage = (int)param.baseDamage;
        this.resistance = param.resistance;
        if (param.scalingSTR != "") this.scalingSTR = param.scalingSTR.DecodeCharSeparatedEnumsAndGetFirst<Scaling>();

        if (param.scalingAGL != "") this.scalingAGL = param.scalingAGL.DecodeCharSeparatedEnumsAndGetFirst<Scaling>();
        this.maxDurability = param.maxDurability;
        this.curDurability = param.maxDurability;
        this.maxHealth = param.maxHealth;
        this.curHealth = param.maxHealth;

        this.STR = param.STR;
        this.AGL = param.AGL;
        this.power = param.STR;
        this.rndSpriteNum = -1;
        this.alive = true;
        
        if(param.PermanentEffects != "")
        {
            foreach (var effect in param.PermanentEffects.DecodeCharSeparatedEnums<EffectType>())
            {
                activeEffects.Add(new EffectElement(effect, -1));
            }
        }

        if (!string.IsNullOrEmpty(param.defaultItemsInInventory))
        {
            var items = param.defaultItemsInInventory.Split(',');

            foreach (var itemString in items)
            {
                var nameAndChance = itemString.Split('*');
                string dropName = nameAndChance[0];
                float amount = float.Parse(nameAndChance[1]);
                for (float i = amount; i > 0; i -= 100)
                {
                    if (BaseMethodsClass.Chance(i))
                    {
                        itemsInInventory.Add(ItemDataBase.GetObject(dropName).dynamicData);
                    }
                }




            }
        }

        if (param.defaultItemInMainHand != "")
        {
            this.itemInMainHand = ItemDataBase.GetObject(param.defaultItemInMainHand).dynamicData;
        }
        if(param.defaultItemInOffHand != "")
        {
            this.itemInOffHand = ItemDataBase.GetObject(param.defaultItemInOffHand).dynamicData;
        }
        if(param.defaultItemInOnChest != "")
        {
            this.itemOnChest = ItemDataBase.GetObject(param.defaultItemInOnChest).dynamicData;
        }
        if(param.defaultItemInOnHead != "")
        {
            this.itemOnHead = ItemDataBase.GetObject(param.defaultItemInOnHead).dynamicData;
        }

    }

    public DynamicObjectData()
    {

    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
[System.Serializable]
public class SpawnData : ISpawnDataByChance
{
    public string name;
    public List<Biome> biomes;
    public int flockSize = 1;
    public bool IsCreature = false;
    public int normalDepthStart { get; set; }
    public int normalDepthEnd { get; set; }
    public float baseSpawnChance { get; set; }



    public ObjectData objectData;
    public SpawnData()
    {

    }
    public SpawnData(ObjectData objectData)
    {
        this.objectData = objectData;
    }

    public SpawnData(Entity_ItemTable.Param param)
    {
        name = param.shorthand;
        if (param.tags.DecodeCharSeparatedEnums<Tag>().Contains(Tag.Creature)) IsCreature = true;
        
        normalDepthStart = param.normalDepthStart;
        normalDepthEnd = param.normalDepthEnd;
        baseSpawnChance = param.baseSpawnChance;
        flockSize = 1;
        objectData = new ObjectData(param);
        biomes = param.biome.DecodeCharSeparatedEnums<Biome>();
    }



}
