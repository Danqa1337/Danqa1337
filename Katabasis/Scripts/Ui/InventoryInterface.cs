using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InventoryInterface : BaseMethodsClass
{
    public List<InventorySlot> slots;
    public List<EquipSlot> equipSlots;

    protected virtual void Awake()
    {
        PlayerAbilitiesSystem.OnItemAddedToInventory += PlaceToEmptySlot;
        PlayerAbilitiesSystem.OnItemRemovedFromInventory += Remove;
    }

    private void OnDisable()
    {
        PlayerAbilitiesSystem.OnItemAddedToInventory -= PlaceToEmptySlot;
        PlayerAbilitiesSystem.OnItemRemovedFromInventory -= Remove;

    }
    //void SetSlotWithItemEmpty(Thing thing)
    //{
    //    InventorySlot slot = FindSlotwithItem(thing);
    //    if (slot != null) slot.Clear();
    //}



    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].currentItem == Entity.Null)
                {
                    counter++;
                }
            }
            return counter;
        }
    }
    public InventorySlot FindSlotwithItem(Entity _item)
    {

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentItem == _item)
            {
                return slots[i];
            }
        }
        for (int i = 0; i < equipSlots.Count; i++)
        {
            if (equipSlots[i].currentItem == _item)
            {
                return equipSlots[i];
            }
        }
        return null;
    }
    //public List<InventorySlot> FindSlotsWithID(string id)
    //{
    //    List<InventorySlot> slots = new List<InventorySlot>();
    //    slots.AddRange(equipSlots);

    //    for (int i = 0; i < slots.Count; i++)
    //    {
    //        if (slots[i].item != null && slots[i].item.name == id)
    //        {
    //            slots.Add(slots[i]);
    //        }
    //    }
    //    return slots;

    //}
    //public InventorySlot FindSlotwithSameID(Thing _item)
    //{

    //    for (int i = 0; i < slots.Count; i++)
    //    {
    //        if (slots[i].item != null && slots[i].item.name == _item.name)
    //        {
    //            return slots[i];
    //        }
    //    }
    //    return null;
    //}
    public void Remove(Entity item)
    {
        var slot = FindSlotwithItem(item);
        if(slot != null) slot.Clear();

    }
    public virtual void PlaceToEmptySlot(Entity item)
    {

        if (EmptySlotCount > 0)
        {
            foreach (var slot in slots)
            {
                if (slot.currentItem == Entity.Null)
                {  
                    slot.PlaceItemFromComponent(item);
                    return;

                }
            }
        }
        else
        {
            //inventory.DropOut(item);
            Debug.Log("Inventory is full");
            GameLog.i.NewLine("Your inventory is full");
        }
    }
}
