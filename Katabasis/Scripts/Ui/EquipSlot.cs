using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EquipSlot : InventorySlot
{
    public EquipPice equipTag;
    public delegate void SlotUpdatedDelegate(Entity item, Tag tag);
    public event SlotUpdatedDelegate OnSlotUpdated;
    public override void Awake()
    {
        base.Awake();
    }

    public override void PlaceItemFromSlot(Entity item)
    {
        
        PlayerAbilitiesSystem.playerEntity.GetComponentData<EquipmentComponent>().EquipItem(item, equipTag, false);
        PlaceItem(item);
        PlayersInventoryInterface.i.UpdatePortrait();
    }

    public override void Clear()
    {
        if (currentItem != Entity.Null)
        {
            PlayerAbilitiesSystem.playerEntity.GetComponentData<EquipmentComponent>().UnequipItem(currentItem,false);

        }
        base.Clear();
        PlayersInventoryInterface.i.UpdatePortrait();

    }
}
