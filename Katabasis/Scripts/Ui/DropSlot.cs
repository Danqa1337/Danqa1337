using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DropSlot : InventorySlot
{
    public override void PlaceItemFromSlot(Entity item)
    {
        if (item != Entity.Null)
        {
            PlayerAbilitiesSystem.playerEntity.CurrentTile().Drop(item);
        }
    }
    public override void Clear()
    {
        
    }

}
