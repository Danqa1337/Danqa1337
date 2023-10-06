using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class UiEffectIcon : MonoBehaviour
{ 
    public Counter timer;
    public Image icon;
    public EffectElement effect;
    public void UpdateIcon(EffectElement effect)
    {
        this.effect = effect;
        timer.SetValue(effect.Duration);
    }

}
