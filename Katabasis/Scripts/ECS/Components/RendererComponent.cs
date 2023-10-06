using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RendererComponent : MonoBehaviour
{
    public Entity parent;
    public SpriteRenderer spriteRenderer;
    public SpriteMask mask;
    public Vector2 spriteCenterOffset;
    public List<Sprite> sprites => ItemDataBase.GetObject(parent.GetComponentData<IDComponent>().ID).staticData.sprites;

    public List<Sprite> equipSprites => ItemDataBase.GetObject(parent.GetComponentData<IDComponent>().ID).staticData.equipSprites;


    public int randomSpriteNum;

    public Color color
    {
        get => spriteRenderer.color;
        set => spriteRenderer.color = value;
    }
    public Sprite sprite
    { 
        get => spriteRenderer.sprite;
        set { spriteRenderer.sprite = value; mask.sprite = value; }
    }
    public int sortingLayer
    {
        get
        {
            return spriteRenderer.gameObject.layer;
        }
        set
        {
            spriteRenderer.gameObject.layer = value;
        }
    }
    public float Z
    {
        get
        {
            return transform.position.z;
        }
        set
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, value);
        }
    }
    public int sortingOrder
    {
        get
        {
            return spriteRenderer.sortingOrder;
        }
        set
        {
            spriteRenderer.sortingOrder = value;
        }
    }
    public string spritesSortingLayerName
    {
        get
        {
            return spriteRenderer.sortingLayerName;
        }
        set
        {
            spriteRenderer.sortingLayerName = value;
            mask.sortingLayerName = value;
        }
    }
    public void DrawDropSprite()
    {
        spriteRenderer.sprite = sprites[randomSpriteNum];
    }
    public void DrawEquipSprite()
    {
        Debug.Log("drawn as equip");
        spriteRenderer.sprite = equipSprites[randomSpriteNum];
    }
    public void ChooseSeamlessSprite()
    {
        //if (sprites.Length == 64)
        //{
        //    spriteRenderer.sprite = sprites[SeamlessTextureAligner.GetSpriteNum(parent)];
        //}
    }
    public void ChooseRandomSprite()
    {
        if (sprites[randomSpriteNum] != null)
        {
            DrawDropSprite();
        }
    }
    public void BecomeInvisible()
    {

        spriteRenderer.material.SetFloat("invisible", 1);
        if (parent.GetComponentData<AnatomyComponent>().GetRootPart() != PlayerAbilitiesSystem.playerEntity)
        {
            spriteRenderer.gameObject.layer = 6;
        }
        
    }
    public void Hide()
    {
        spriteRenderer.gameObject.layer = 6;
    }
    public void Show()
    {
        spriteRenderer.gameObject.layer = 0;
    }
    public void Becomevisible()
    {

        spriteRenderer.material.SetFloat("invisible", 0);
        spriteRenderer.gameObject.layer = 0;
        

    }

    public async void DrawDamageAnimation()
    {
        for (int i = 0; i < 5; i++)
        {
            color = Color.gray;
            await Task.Delay(30);
        }
        color = Color.white;
    }

}
