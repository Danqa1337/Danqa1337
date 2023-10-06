using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public enum PopupType
{
    Fear,
    Interaction,
    Bleeding,
    NoAmmo,
    Death,
    Parry,
    Evade,
    jump,
}
public class PopUpCreator : Singleton<PopUpCreator>
{
    //static Queue<PopUp> sheludedPopups;
    public int delayBetweenPopUps;
    public Color damagePopupColor;
    public Color imagePopupColor;
    private bool ready = true;
    
    

    [Header("Sprites")]
    public Sprite fear;
    public Sprite interaction;
    public Sprite Bleeding;
    public Sprite NoAmmo;
    public Sprite Death;
    public Sprite Parry;
    public Sprite Evasion;
    public Sprite Jump;
    async private void CreatePopUp(Transform tr, Color textColor,string text = "",  Sprite sprite = null, float offset = 1.5f, float animationSpeed = 1)
    {
        
        float2 position = tr.position.ToFloat2();

        while(ready == false)
        {
            await Task.Delay(delayBetweenPopUps);
        }
            
        ready = false;
        if (tr.gameObject.activeSelf)
        {
            position = tr.position.ToFloat2();
        }
        if (position.ToTileData().visible)
        {
            PopUp newPopUp = Pooler.Take("PopUp").GetComponent<PopUp>();
                

            newPopUp.transform.position = (position + UnityEngine.Random.insideUnitCircle.ToFloat2() * 0.25f).ToRealPosition();
            newPopUp.Draw(text, textColor, sprite, animationSpeed);
                
               
                
        } 
        ready = true;
        
    }

    async public void CreatePopUp(Transform tr, Sprite sprite, float offset = 1.5f, float animationSpeed = 1)
    {
        CreatePopUp(tr, Color.clear, "", sprite, offset, animationSpeed);
    }
    async public void CreatePopUp(string text)
    {
        CreatePopUp(PlayerAbilitiesSystem.playerEntity.GetComponentObject<Transform>(), Color.gray, text, null);
    }
    async public void CreatePopUp(Transform tr, string text)
    {
        CreatePopUp(tr, Color.gray, text, null);
    }
    async public void CreatePopUp(Transform tr, string text, Color color,float offset = 1.5f, float animationSpeed = 1)
    {
        CreatePopUp(tr, color,text, null, offset, animationSpeed);
    }

    async public void CreatePopUp(Transform tr, PopupType popupType, float offset = 1.5f)
    {
        switch (popupType)
        {
            case PopupType.Fear: CreatePopUp(tr, fear, offset);
                break;
            case PopupType.Interaction: CreatePopUp(tr, interaction, offset);
                break;
            case PopupType.Bleeding:
                CreatePopUp(tr, Bleeding, offset);
                break;
            case PopupType.NoAmmo:
                CreatePopUp(tr, NoAmmo, offset);
                break;
            case PopupType.Death:
                CreatePopUp(tr, Death, offset);
                break;
            case PopupType.Parry:
                CreatePopUp(tr, Parry, offset);
                break;
            case PopupType.Evade:
                CreatePopUp(tr, Evasion, offset);
                break;
            case PopupType.jump:
                CreatePopUp(tr, Jump, offset);
                break;
            default:
                break;
        }


    }

    
}
