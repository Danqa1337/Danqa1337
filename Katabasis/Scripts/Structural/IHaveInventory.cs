using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHaveInventory 
{
  
    public Inventory inventory 
    {
         get;
    }
    public abstract TileData currentTileData
    {
        get;
    }

   
}
public interface IAmInventory
{
    public Transform holder
    {
        get;
        set;
    }
    public IHaveInventory parent
    {
        get;
        set;
    }

}
