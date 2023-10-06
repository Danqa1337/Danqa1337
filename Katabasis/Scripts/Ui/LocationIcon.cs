using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationIcon : MonoBehaviour
{
    public Location location;
    public Color selectedColor;
    public ParticleSystem system;
    public TextMeshProUGUI id;
    public TextMeshProUGUI depth;
    public TextMeshProUGUI Name; 
    public Image currentLoactionIndicator;
    public Image icon;
    public Image background;
    public Sprite dungeon;
    public Sprite Cave;
    public Sprite Pit;
    public Sprite Lake;
    public Sprite LavaLake;


    public void Visualize(Location _location)
    {
        location = _location;
         id.text = "ID " + location.id.ToString();
         depth.text = "Level" + location.level.ToString();
        Name.text = location.name;
        icon.sprite = chooseSprite();
      // background.color = Color.Lerp(Color.green, Color.red, (float)level / (float)DungeonStructure.i.NumberOfLayers);



        Sprite chooseSprite()
        {
            switch(_location.generationPreset)
            {
                case GenerationPresetType.Dungeon: return dungeon;
                case GenerationPresetType.Lake: return Lake;
                case GenerationPresetType.Pit: return Pit;

            }
            return null;
        }

    }
}
