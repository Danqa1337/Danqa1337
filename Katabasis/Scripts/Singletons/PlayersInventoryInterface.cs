using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayersInventoryInterface : InventoryInterface
{
    private static PlayersInventoryInterface _i;

    public EquipSlot itemInMainHand;
    public EquipSlot itemInOffHand;
    public EquipSlot itemOnHead;
    public EquipSlot itemOnChest;
    public InventorySlot dropSlot;
    public PhotoCamera portraitCamera;

    public Counter XpPointsCounter;
    public Counter StrCounter;
    public Counter AGLCounter;

    protected override void Awake()
    {
        base.Awake();
        XPCounter.OnExpChanged += UpdateStats;
    }

    public void UpdateStats()
    {
        XpPointsCounter.SetValue(XPCounter.XPpoints);
        StrCounter.SetValue(PlayerAbilitiesSystem.playerEntity.GetComponentData<CreatureComponent>().str);
        AGLCounter.SetValue(PlayerAbilitiesSystem.playerEntity.GetComponentData<CreatureComponent>().agl);
    }
    public void UpdatePortrait()
    {

        portraitCamera.MakePhoto(PlayerAbilitiesSystem.playerEntity);
    }
    public static PlayersInventoryInterface i
    {
        get
        {
            if (_i == null) _i = FindObjectOfType<PlayersInventoryInterface>();
            return _i;
        }
    }



    public EquipSlot GetEquipSlot(EquipPice tag) => (tag) switch
    {
        EquipPice.Weapon => itemInMainHand,
        EquipPice.Shield => itemInOffHand,
        EquipPice.Headwear => itemOnHead,
        EquipPice.Chestplate => itemOnChest,
        EquipPice.Boots => itemInMainHand,
    };
}
