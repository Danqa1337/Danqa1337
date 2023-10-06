using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class DungeonStructureVisualizer : Singleton<DungeonStructureVisualizer>
{
    public int h;
    public int w;
    public Color TransitionColor;
    public float transitionWidth;
    public List<LocationIcon> icons;
    public RectTransform holder;
    private float maxY = 0;
    public Sprite transitionSprite;

    private void Awake()
    {
        Clear();
    }
    public void UpdateCurrentLocationInfo()
    {
      

            //foreach (var item in icons)
            //{
            //    if (item.location == dungeonStructure.currentLocation) item.currentLoactionIndicator.enabled = true;
            //    else item.currentLoactionIndicator.enabled = false;
            //}
        
    }

    public void Visualize()
    {
        Clear();
        holder = new GameObject("Holder").AddComponent<RectTransform>();
        holder.SetParent(GetComponent<RectTransform>());
        holder.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        maxY = 0;


        for (int y = 0; y < DungeonStructure.depth; y++)
        {
            var locationsOnLayer = DungeonStructure.dungeonStructureData.locations
                .Where( l => l.level == y).ToList();

            for (int x = 0; x < locationsOnLayer.Count; x++)
            {
                LocationIcon levelIcon = Instantiate(ItemDataBase.i.levelIcon).GetComponent<LocationIcon>();
                RectTransform rectTransform = levelIcon.GetComponent<RectTransform>();
                
                rectTransform.SetParent(holder);
                levelIcon.GetComponent<Canvas>().sortingLayerName = "Ui";
                levelIcon.Visualize(locationsOnLayer[x]);
                icons.Add(levelIcon);

                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);

                int mX = 1;
                int mY = 1;
                //if (MapGenUtills.Chance(50)) mX = -1;


                if (locationsOnLayer[x] == DungeonStructure.dungeonStructureData.pit)
                {
                    mX = -1;
                }

                float X = x * w * mX;
                float Y = -y * h;

                if (Y < maxY) maxY = Y;
                rectTransform.anchoredPosition = new Vector2(X, Y);
            }
        }

        foreach (var icon in icons)
        {
            foreach (var id in icon.location.transitionsIDs)
            {
                var transition = DungeonStructure.GetTransition(id);
                if (transition.LocationFromID != -1 && transition.LocationToID != -1)
                {
                    Connect(GetIcon(transition.LocationFromID),
                        GetIcon(transition.LocationToID));
                }
            }
        }
    }

    private bool locationAllreadyVisualized(Location location)
    {
        foreach (var item in icons)
        {
            if (item.location == location)
            {
                return true;
            }

        }
        return false;
    }
    private LocationIcon GetIcon(int id)
    {
        foreach (var icon in icons)
        {
            if (icon.location.id == id) return icon;
        }
        return null;
    }
    //private Vector3 calculatePosition(LocationTransition level)
    //{

    //    switch (level.biome)
    //    {
    //        case Biome.Dungeon: return new Vector3(0, -level.level * h, 0);
    //        case Biome.Pit: return new Vector3(w, -level.level * h, 0);
    //        case Biome.Cave: return new Vector3(-w, -level.level * h - w, 0);

    //    }

    //    return Vector3.zero;
    //}
    private void Connect(LocationIcon locationA, LocationIcon locationB)
    {
        Vector2 posA = locationA.GetComponent<RectTransform>().anchoredPosition;
        Vector2 posB = locationB.GetComponent<RectTransform>().anchoredPosition;

        GameObject transition = new GameObject("Transition", typeof(Image));
        var canvas = transition.AddComponent<Canvas>();
        canvas.sortingLayerID = -1;
        transition.transform.SetParent(holder, true);
        var image = transition.GetComponent<Image>();
        image.color = Color.white;
        image.sprite = transitionSprite;
        image.type = Image.Type.Tiled;
        image.pixelsPerUnitMultiplier = 32;
        RectTransform rectTransform = transition.GetComponent<RectTransform>();
        Vector2 dir = (posB - posA).normalized;
        float distance = Vector2.Distance(posA, posB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, transitionWidth);
        rectTransform.anchoredPosition = posA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, CodeMonkey.Utils.UtilsClass.GetAngleFromVectorFloat(dir));
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
       
        if (holder != null) DestroyImmediate(holder.gameObject);
        icons = new List<LocationIcon>();
    }
}
