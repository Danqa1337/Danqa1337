using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class InventorySlot : BaseMethodsClass
{

    public Tag[] allowedTags = new Tag[0];

    public Entity currentItem;
    public int amount;
    public ImagesStack imagesStack;





    public virtual void Dispose()
    {
        currentItem = Entity.Null;
        if(imagesStack != null) imagesStack.Clear();
    }
 
    
    public virtual void Awake()
    {
        Engine.OnDispose += Dispose;
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnter(); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExit(); });
        AddEvent(gameObject, EventTriggerType.BeginDrag, delegate { OnDragStart(); });
        AddEvent(gameObject, EventTriggerType.EndDrag, delegate { OnDragEnd(); });
        AddEvent(gameObject, EventTriggerType.Drag, delegate { OnDrag(); });
    }

    public virtual void SetItemInComponent(Entity _item)
    {
        PlayerAbilitiesSystem.playerEntity.GetComponentData<Inventory>().Add(_item, false);
    }
    public virtual void RemoveItemFromComponent(Entity _item)
    {
        PlayerAbilitiesSystem.playerEntity.GetComponentData<Inventory>().DropOut(_item, false);
    }

    protected void PlaceItem(Entity item)
    {
      
        if (item != Entity.Null)
        {
            currentItem = item;
            imagesStack.DrawItem(item);
            currentItem.GetComponentObject<EntityAuthoring>().OnDeath += Clear;
        }

    }
    public virtual void PlaceItemFromComponent(Entity item)
    {
        PlaceItem(item);
    }
    public virtual void PlaceItemFromSlot(Entity item)
    {
        if (currentItem != item)
        {
            PlaceItem(item);

            if (!PlayerAbilitiesSystem.playerEntity.GetComponentData<Inventory>().items.Contains(item))
            {
                PlayerAbilitiesSystem.playerEntity.GetComponentData<Inventory>().Add(item, false);
            }
        }
    }
    public virtual void Clear()
    {
        if (currentItem != Entity.Null)
        {
            currentItem.GetComponentObject<EntityAuthoring>().OnDeath -= Clear;
            PlayerAbilitiesSystem.playerEntity.GetComponentData<Inventory>().DropOut(currentItem, false);
        }

        currentItem = Entity.Null;
        
        imagesStack.Clear();
        amount = 0;
        
    }

    public bool CanPlaceInSlot(Entity item)
    {
        
        if (allowedTags.Length == 0 || item == Entity.Null) return true;
        foreach (var tag in allowedTags)
        {
                if (item.GetTags().Contains(tag)) return true;
        }
        return false;
    }
       
    public virtual void UpdateSlot()
    {
        //if(!parentInterface.inventory.items.Contains(item))
        //{
        //        item = null;
        //}
    }
    public void OnEnter()
    {
        MouseData.slotHoveringOver = this;
        if(currentItem != Entity.Null)InventoryDescriber.i.Describe(currentItem);
    }
    public void OnExit()
    {
        MouseData.slotHoveringOver = null;
        
    }

    public void OnClick()
    {
       InventoryDescriber.i.item = currentItem;
    }
    public void OnDragStart()
    {
        if(currentItem!= Entity.Null)
        {
            Coursor.i.imagesStack.DrawItem(currentItem);
            MouseData.slotTakenFrom = this;
        }
        //if(item != Entity.Null) InventoryDescriber.i.item = item;


    }

    public void OnDragEnd()
    {

        

        SwapSlots(MouseData.slotHoveringOver, MouseData.slotTakenFrom);
        Coursor.i.imagesStack.Clear();
        MouseData.slotTakenFrom = null;

        
    }
    public void OnDrag()
    {

    }
   
  
}
