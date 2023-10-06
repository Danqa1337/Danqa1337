using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public struct InventoryBufferElement : IBufferElementData
{
    public Entity entity;

    public InventoryBufferElement(Entity entity)
    {
        this.entity = entity;
    }
}
public struct Inventory : IComponentData
{
    public Entity parent;
    private DynamicBuffer<InventoryBufferElement> _items =>parent.GetBuffer<InventoryBufferElement>();

    public Inventory(Entity parent)
    {
        this.parent = parent;
        parent.AddBuffer<InventoryBufferElement>();
    }

    public HashSet<Entity> items
    {
        
        get
        {   var hashset = new HashSet<Entity>();
            foreach (var item in _items)
            {
                hashset.Add(item.entity);
            }
            return hashset;
        }
    }
    
    

    public Entity Add(Entity item, bool updateSlot = true)
    {
        
        if (item != Entity.Null)
        {

            if (!items.Contains(item))
            {
                item.CurrentTile().Remove(item);
                var tr = item.GetComponentObject<Transform>();
                item.GetComponentObject<EntityAuthoring>().trail.emitting = false;

                tr.position = new float2(-1, -1).ToRealPosition();
                _items.Add(new InventoryBufferElement(item));
                
                Debug.Log("Adding " + item.GetName() + " to " + parent.GetName() + "'s inventory ");
                if (updateSlot && parent.IsPlayer())
                {
                    PlayerAbilitiesSystem.OnItemAddedToInventory?.Invoke(item);
                }
                if (item.HasComponent<ImpulseComponent>())
                {
                    item.RemoveComponent<ImpulseComponent>();
                }
                Debug.Log(item.GetName() + " added to " + parent.GetName() + "'s inventory");
            }
            else
            {
                throw new Exception("Item is already in inventory");
            }
        }
        parent.SetComponentData(this);
        return item;
    }
    public bool HasItem(string name)
    {
        Entity entity;
        return HasItem(name, out entity);
    }
    public bool HasItem(string name, out Entity item)
    {
        foreach (var i in items)
        {
            if(i.GetName() == name)
            {
                item = i;
                return true;
            }
        }
        item = Entity.Null;
        return false;
    }
    public Entity DropOut(Entity item, bool updateSlot = true)
    {

        if (_items.Contains(item))
        {
            
            _items.Remove(new InventoryBufferElement(item));
            var tr = item.GetComponentObject<Transform>();
            var ph = item.GetComponentData<PhysicsComponent>();


            if (item.HasComponent<ImpulseComponent>()) item.RemoveComponent<ImpulseComponent>();

            tr.position = parent.CurrentTile().position.ToRealPosition();

            item.SetComponentData(ph);
            parent.CurrentTile().Drop(item);
            if (updateSlot && parent == PlayerAbilitiesSystem.playerEntity)
            {
                
                PlayerAbilitiesSystem.OnItemRemovedFromInventory?.Invoke(item);
            }

        }
        parent.SetComponentData(this);
        return item;
    }
    public void Clear()
    {
        for (int i = 0; i < _items.Length; i++)
        {
           
            //Destroy(_items[i]);
            
        }
        _items.Clear();

    }

    public void DropAll(int chance = 100)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if(BaseMethodsClass.Chance(chance))   DropOut(_items[i].entity);
        }
    }
    public Entity recieveItemFromGenerator(Entity _thing)
    {
        //Thing thing = Instantiate(ItemDataBase.i.GetItem(_thing.name)).GetComponent<Thing>();
        //Add(thing);
        return _thing;
    }

    public  Entity recieveItemFromGenerator(string _name)
    {
        if (_name != null)
        {

        }
        return Entity.Null;
    }
    public Entity recieveItemFromGenerator(List<Tag> _tags, int levelBonus = 0, Biome biome = Biome.Any)
    {
        //Thing thing = Instantiate(ItemDataBase.i.GetRandomThing(_tags, DungeonStructure.i.currentLocation.level + levelBonus, biome));
        //Add(thing);
        return Entity.Null;
    }


}
