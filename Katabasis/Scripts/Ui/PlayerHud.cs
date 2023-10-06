using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;

public class PlayerHud : Describer
{
    public Bar hpBar;
    public Bar ExpBar;
    public Bar AwareBar;
   
    public Counter movementCostCounter;
    public Counter atkCostCounter;
    public Counter levelCounter;
    public Counter HpCounter;

    public VerticalLayoutGroup EffectsLayout;
    public Image painImage;
    public Canvas rangedWeaponInfo;
    public List<UiEffectIcon> effectIcons;

    public static PlayerHud i { get; private set; }
    private void Awake()
    {
        i = this;
        XPCounter.OnExpChanged += UpdateXPBars;
        UpdateXPBars();
    }

    public void UpdateXPBars()
    {
        ExpBar.SetNewValue(XPCounter.CurrentXP, XPCounter.nextLevelCost);
        levelCounter.SetValue(XPCounter.CurrentLevel);
    }
    private void AddIcon(EffectElement effect)
    {
        var obj = Pooler.Take("EffectIcon");
        var icon = obj.GetComponent<UiEffectIcon>();
        obj.transform.SetParent(EffectsLayout.transform);
        icon.icon.sprite = ItemDataBase.GetEffectIcon(effect.type);
        icon.UpdateIcon(effect);
        obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        obj.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        effectIcons.Add(obj.GetComponent<UiEffectIcon>());
        
    }
    public void UpdateEffects()
    {
        var buffer = PlayerAbilitiesSystem.playerEntity.GetBuffer<EffectElement>();
        foreach (var effect in buffer)
        {
            bool iconFound = false;
            foreach (var icon in effectIcons)
            {
                if (effect.type == icon.effect.type)
                {
                    icon.UpdateIcon(effect);
                    iconFound = true;
                    break;
                }

            }
            if (!iconFound)
            {
                AddIcon(effect);
            }
        }
        for (int i = 0; i < effectIcons.Count; i++)
        {
            if(effectIcons[i].effect.Duration == 0)
            {
                RemoveIcon(effectIcons[i].effect.type);
            }
        }

        // foreach (var icon in effectIcons)
        //{
        //    bool effectFound = false;
        //    foreach (var effect in buffer)
        //    {
        //        if (effect.Duration != 0 && effect.type == icon.effect.type)
        //        {
        //            effectFound = true;
        //            break;
        //        }
        //    }
        //    if (!effectFound)
        //    {
        //        RemoveIcon(icon.effect.type);
        //    }
        //}

    }
    private void RemoveIcon(EffectType type)
    {
        for (int i = 0; i < effectIcons.Count; i++)
        {

       
            if(effectIcons[i].effect.type == type)
            {
                
                effectIcons[i].transform.SetParent(null);
                Pooler.PutObjectBackToPool(effectIcons[i].gameObject);
                effectIcons.Remove(effectIcons[i]);
                return;
            }
        }
    }

    private void Update()
    {
        if(PlayerAbilitiesSystem.playerEntity != Entity.Null && PlayerAbilitiesSystem.playerEntity.HasComponent<CreatureComponent>())
        {
            var playerCreature = PlayerAbilitiesSystem.playerEntity.GetComponentData<CreatureComponent>();

            //painImage.material.SetFloat("_alphaMultipler", Mathf.Lerp(0, 0.4f, Player.i.maxHp / Mathf.Max(1, Player.i.currentHp) * 0.1f));
            
            hpBar.SetNewValue(playerCreature.currentHealth, playerCreature.MaxHealth);
            HpCounter.SetValue(playerCreature.currentHealth);
        }


        //ExpBar.SetNewValue(Player.i.playerStats.currentExp, Player.i.playerStats.expToNextLvl);
       // levelCounter.SetValue(Player.i.playerStats.lvl, "", "LVL");

        //movementCostCounter.SetValue(RoundFloat(Player.i.MovementCost));
        //atkCostCounter.SetValue(RoundFloat(Player.i.CalculateAtackCost(Player.i.currentWeapon)));

        //if (AwareBar != null)
        //{
        //    int a = 0;
        //    switch (Player.i.currentReadinness)
        //    {
        //        case ReadinessLevel.Sleeping:
        //            a = 0;
        //            break;
        //        case ReadinessLevel.Loitering:
        //            a = 1;
        //            break;
        //        case ReadinessLevel.OnAllert:
        //            a = 2;
        //            break;
        //        case ReadinessLevel.FullyConcentrated:
        //            a = 3;
        //            break;
        //        default:
        //            break;
        //    }
        //    //AwareBar.rectTransform.sizeDelta = new Vector2((awareBarMax.x / 3) * a, awareBarMax.y);
        //}


    }
}
