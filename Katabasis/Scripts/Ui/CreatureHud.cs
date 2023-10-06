using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class CreatureHud : MonoBehaviour
{
    public Bar hpbar;
    public Bar moralebar;
    public Bar awareBar;
    public Image hostilityIndicator;
    public Canvas canvas;
    public SpriteRenderer stunEffect;

    public void Show(Entity creature)
    {
        canvas.enabled = true;
        var creatureComponent = creature.GetComponentData<CreatureComponent>();
        hpbar.SetNewValue(creatureComponent.currentHealth, creatureComponent.MaxHealth);
        var ai = creature.GetComponentData<AIComponent>();
        moralebar.SetNewValue(ai.morale, 100);
    }
    public void Hide()
    {
        canvas.enabled = false;
        //stunEffect.enabled = false;
    }
    public void UpdateValues()
    {
        //{
        //    hpbar.SetNewValue(creature.currentHp, creature.maxHp);
        //    moralebar.SetNewValue(creature.currentMorale, 100);
           

        //    if (awareBar != null)
        //    {
        //        int a = 0;
        //        switch (creature.currentReadinness)
        //        {
        //            case ReadinessLevel.Sleeping: a = 0;
        //                break;
        //            case ReadinessLevel.Loitering: a = 1;
        //                break;
        //            case ReadinessLevel.OnAllert: a = 2;
        //                break;
        //            case ReadinessLevel.FullyConcentrated: a = 3;
        //                break;
        //            default:
        //                break;
        //        }
        //        awareBar.SetNewValue(a, 3);
        //    }

        //    if (creature.activeEffectTypes.Contains(EffectType.Stun))
        //    {
        //        stunEffect.enabled = true;
        //    }
        //    else
        //    {
        //        stunEffect.enabled = false;
        //    }
        //    ChangeHostilityIndicator();
        //}    
    }


    //public void ChangeHostilityIndicator()
    //{
    //    if (creature.teamTag == Tag.Player)
    //    {
    //        hostilityIndicator.color = Color.green;
    //    }
    //    else
    //    { 
    //     if (creature.enemyTags.Contains(Tag.Player))
    //    {
    //          if(creature.currentMorale < creature.moraleBreakTreshold) hostilityIndicator.color = Color.blue;
    //            else hostilityIndicator.color = Color.red;
            
    //        return;
    //    }
    //    else
    //    {
    //        hostilityIndicator.color = Color.yellow;

    //    }
    // }

       
    //}
}
