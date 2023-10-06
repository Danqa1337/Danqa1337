using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public class Selector : Singleton<Selector>
{
    public static TileData selectedTile;
    private static TileData lastSelectedTile; 

    public delegate void SelectionChangedDelegate();
    public event SelectionChangedDelegate OnSelectionChanged;
    public UnityEngine.Material thingMaterial;
    public static PathFinderPath path;
    public CreatureHud creatureHud;
    private void Awake()
    {
        OnSelectionChanged += UpdateTileSelection;
        Engine.i.PathMask.Clear();
        creatureHud.Hide();
    }
    void Update()
    {
        
        if (Controller.i.selectionEnabled)
        {
            TileData tile = new float2
                (UiManager.i.mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue()).x + 0.5f,
                    UiManager.i.mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue()).y + 0.5f).ToTileData();

            if (tile != TileData.Null)
            {
               
                selectedTile = tile;

                if (selectedTile != lastSelectedTile)
                {
                    var tileBelow = lastSelectedTile - new int2(0, 1);

                    if (tileBelow != TileData.Null && tileBelow.SolidLayer != Entity.Null)
                    {
                        tileBelow.SolidLayer.GetComponentObject<RendererComponent>().spriteRenderer.material.SetFloat("HideWall", 0);
                    }

                    UpdateTileSelection();
                    
                }
                lastSelectedTile = selectedTile;
            }
            else selectedTile = TileData.Null;
        }
    }
    public void UpdateTileSelection()
    {
        
        if (selectedTile != TileData.Null)
        {
            if (path.length != 0) //clear markers
            {
                foreach (var item in path.tiles)
                {
                    Engine.i.PathMask.SetPixel(item.x, item.y, Color.clear);
                }
            }

            if (selectedTile.isWalkable(PlayerAbilitiesSystem.playerEntity) && selectedTile != PlayerAbilitiesSystem.CurrentTile) //draw new markers
            {
                path = Pathfinder.FindPath(PlayerAbilitiesSystem.CurrentTile, selectedTile, PlayerAbilitiesSystem.playerEntity, 1000);
                if (path.length != 0)
                {
                    foreach (var item in path.tiles)
                    {
                        Engine.i.PathMask.SetPixel(item.x, item.y, Color.white);
                    }
                }

            }
            if (!selectedTile.SolidLayer.IsInInvis() && selectedTile.visible && selectedTile.SolidLayer.HasComponent<AIComponent>())
            {
                creatureHud.Show(selectedTile.SolidLayer);
            }
            else
            {
                creatureHud.Hide();
            }

            transform.position = new Vector3(selectedTile.position.x, selectedTile.position.y, 10000);
            var tileBelow = selectedTile - new int2(0, 1);
            if(!selectedTile.HasLOSblock && selectedTile.visible && Controller.controllerState == Controller.ControllerState.movement)
            {

                if (tileBelow != TileData.Null && tileBelow.SolidLayer != Entity.Null)
                {
                    if (tileBelow.SolidLayer.GetComponentObject<RendererComponent>().sprite.textureRect.height>32)
                    {
                        tileBelow.SolidLayer.GetComponentObject<RendererComponent>().spriteRenderer.material.SetFloat("HideWall", 1);
                    }
                }
            }
           
            //thingMaterial.SetVector("SelectionPos", new Vector4(selectedTile.position.x, selectedTile.position.y,0,0));
            Engine.i.PathMask.Apply();
        }
    }
    
    
   

}
