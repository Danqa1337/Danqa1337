using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public enum Stat
{
    STR,
    AGL,
}
public class IncreaseStatButton : MonoBehaviour
{

    public Counter statCounter;
    public Stat stat;

    private void Awake()
    {
        
    }

    public void Increace()
    {
        if (XPCounter.XPpoints > 0)
        {
            XPCounter.XPpoints--;


            var creature = PlayerAbilitiesSystem.playerEntity.GetComponentData<CreatureComponent>();

            switch (stat)
            {
                case Stat.STR:
                    creature.str++;
                    break;
                case Stat.AGL:
                    creature.agl++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            InventoryDescriber.i.Describe();
            PlayerAbilitiesSystem.playerEntity.SetComponentData(creature);
            PlayersInventoryInterface.i.UpdateStats();
        }
    }
    

    

}
