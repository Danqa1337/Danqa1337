using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect 
{
    
    public float realisationsPerTick;
    public int duration;
    public int power;
    protected float score;
    public UiEffectIcon EffectIcon;
    
    //public Thing parent;
    public abstract Sprite icon();
    public abstract  EffectType effectType();


   
    public virtual void EndEffect()
    {
       // if(parent != null && parent.currentHp !=0) parent.RemoveEffect(effectType());
    }
  
  


    public abstract void EffectRealisation();
    
  
}
