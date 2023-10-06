using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

[DisableAutoCreation]
public class PlayerAbilitiesSystem : AbilitySystem
{

    public static Entity playerEntity;
    public static bool controlled = false;
    public static TileData CurrentTile => playerEntity.CurrentTile();
    public static bool pathMovementStoped;

    public static InventoryEventHandler OnItemAddedToInventory;
    public static InventoryEventHandler OnItemRemovedFromInventory;

    public delegate void InventoryEventHandler(Entity item);

    public static Action BeforePlayersTurn;


    public static Entity RightArm => playerEntity.GetComponentData<AnatomyComponent>().GetBodyPart((BodyPartTag.RightArm));
    public static Entity Head => playerEntity.GetComponentData<AnatomyComponent>().GetBodyPart((BodyPartTag.Head));
    public static Entity LeftArm => playerEntity.GetComponentData<AnatomyComponent>().GetBodyPart((BodyPartTag.LeftArm));
    public static Entity Body => playerEntity.GetComponentData<AnatomyComponent>().GetBodyPart((BodyPartTag.Body));

    public static HashSet<Entity> CreaturesInSight = new HashSet<Entity>();
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    public static PlayerAbilitiesSystem i
    {
        get
        {
            if (_i == null) _i = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerAbilitiesSystem>();
            return _i;
        }
    }

    private static PlayerAbilitiesSystem _i;

    public static void Die()
    {
        Debug.Log("you died");
        Camera.main.transform.SetParent(null);
        CurrentTile.Drop(playerEntity);
        Controller.controllerState = Controller.ControllerState.dead;
        UiManager.i.ToggleUICanvas(UiManager.Menu.Death, UiManager.UICanvasState.On);
    }
    public static void SwapHands()
    {
        var equip = playerEntity.GetComponentData<EquipmentComponent>();
        var itemInMainHand = equip.itemInMainHand;
        var itemInOffHand = equip.itemInOffHand;
        equip.UnequipItem(itemInMainHand);
        equip.UnequipItem(itemInOffHand);

        equip.EquipItem(itemInMainHand, EquipPice.Shield);
        equip.EquipItem(itemInOffHand, EquipPice.Weapon);

    }
    public async Task UseMakeStep(TileData targeTile)
    {

        if (targeTile != playerEntity.CurrentTile())
        {
            if (targeTile.isWalkable(playerEntity))
            {
                playerEntity.AddComponentData(new AbilityComponent() { ability = Ability.MakeStep, targetTile = targeTile });
                UpdateSystem();
            }
        }
    }
    public void UseKickSelected()
    {
        if (Selector.selectedTile != TileData.Null)
        {
            if (playerEntity.CurrentTile() != Selector.selectedTile)
            {
                TileData targetTile = BaseMethodsClass.GetNearestTileFromList(playerEntity.CurrentTile().GetNeibors(true).ToList(), Selector.selectedTile);
                var target = Entity.Null;
                

                if (targetTile.SolidLayer != Entity.Null)
                {
                    target = targetTile.SolidLayer;

                }
                else if (targetTile.DropLayer.Count > 0)
                {
                    target = targetTile.DropLayer.Last();
                }
                playerEntity.AddComponentData(new AbilityComponent(Ability.Kick, targetTile, target));
                UpdateSystem();
            } 
        }
    }
    public void UsePicUp()
    {
        if (playerEntity.CurrentTile().DropLayer.Count > 0)
        {
            if (PlayersInventoryInterface.i.EmptySlotCount > 0)
            {
                var item = CurrentTile.DropLayer.OrderBy(o => o.GetComponentData<PhysicsComponent>().mass).ToList()[0];
                playerEntity.AddComponentData(new AbilityComponent(Ability.PicUp, item));

                UpdateSystem();
            }
            else
            {
                PopUpCreator.i.CreatePopUp("Inventory is full");
            }
        }
        else
        {
            PopUpCreator.i.CreatePopUp("There is nothing you can picup");
        }
    }
    public void UseThrow()
    {
       

        var item = equipment.itemInMainHand;
        if(item == Entity.Null)
        {
            item = equipment.itemInOffHand;
        }
        if (Selector.selectedTile != CurrentTile)
        {
            if (!CurrentTile.GetNeibors(true).Contains(Selector.selectedTile) || Selector.selectedTile.SolidLayer == Entity.Null)
            {
                if (item != Entity.Null)
                {
                    playerEntity.AddComponentData(new AbilityComponent() { ability = Ability.Throw, targetEntity = item, targetTile = Selector.selectedTile });


                    UpdateSystem();
                }
            }
        }
    }
    public void UseAtack()
    {
        if (playerEntity.GetComponentData<EquipmentComponent>().itemInMainHand.HasComponent<RangedWeaponComponent>())
        {
            playerEntity.AddComponentData(new AbilityComponent(Ability.Shoot, Selector.selectedTile));
            UpdateSystem();

        }
        else
        {
            if (playerEntity.CurrentTile().GetNeibors(true).ToList().Contains(Selector.selectedTile) )
            {
                playerEntity.AddComponentData(new AbilityComponent(Ability.Atack, Selector.selectedTile));
                UpdateSystem();
            }  
        }

        
    }
    public void UseJump()
    {

    }
    public void UseEat()
    {
        playerEntity.AddComponentData(new AbilityComponent(Ability.Eat));
        UpdateSystem();

    }
    public override async Task SpendTime(int time)
    {
        controlled = false;
        Debug.Log("player is waiting for " + time + " ticks");
        BeforePlayersTurn?.Invoke();
        await TimeSystem.SpendTime(time);
        controlled = true;
    }
    public async void MoveToSelected()
    {
        if (Selector.selectedTile.maped && Selector.selectedTile != CurrentTile)
        {

            if (Selector.selectedTile.isWalkable(playerEntity))
            {

                PathFinderPath path = Selector.path;
                if (path != PathFinderPath.NUll)
                {

                    pathMovementStoped = false;
                    var pathEnd = path.targetTile;
                    int t = 0;
                    while (path != PathFinderPath.NUll && pathMovementStoped == false && t < 1000 && path.tiles.Count > 1 && playerEntity.CurrentTile() != pathEnd)
                    {

                        t++;
                        if (true)
                        {
                            var targetTile = path.tiles[0];
                            playerEntity.AddComponentData(new AbilityComponent(Ability.MakeStep, targetTile));

                            UpdateValues(playerEntity);

                            await MakeStep();
                            foreach (var creature in CreaturesInSight)
                            {
                                if(creature.IsHostile(playerEntity) && !creature.HasTag(Tag.Immaterial))
                                {
                                    pathMovementStoped = true;
                                    break;
                                }
                            }
                        }

                        Selector.i.UpdateTileSelection();
                        if(pathEnd != playerEntity.CurrentTile()) path = Pathfinder.FindPath(playerEntity.CurrentTile(), pathEnd, playerEntity,1000);
                        else
                        {
                            break;
                        }
                    }

                }

            }
        }
    }

    public async void EnterStaircase()
    {
        foreach (var id in DungeonStructure.CurrentLocation.transitionsIDs)
        {
            var transition = DungeonStructure.GetTransition(id);
            if (transition.positionFrom.Equals(CurrentTile.position) || transition.positionTo.Equals(CurrentTile.position))
            {

                Debug.Log("Entering staircase!");
                var currentLocation = DungeonStructure.CurrentLocation;
                Location nextLocation;
                Debug.Log(transition.LocationFromID+ " to " +transition.LocationToID);
                var nextPlayerPosition = transition.positionTo;
                if (transition.LocationFromID == currentLocation.id)
                {
                    nextLocation = DungeonStructure.GetLocation(transition.LocationToID);
                }
                else
                {
                    nextLocation = DungeonStructure.GetLocation(transition.LocationFromID);
                    nextPlayerPosition = transition.positionFrom;
                }

                await LoadingScreen.i.Show();
                DungeonStructure.CurrentLocation = nextLocation;
                await SaveLoader.Save(currentLocation);
                Debug.Log( "transition "+transition.id +" " + transition.positionFrom +" "+ transition.positionTo);
                if(SaveLoader.LoadCurrentLocation())
                {

                    Spawner.SpawnPlayer(nextPlayerPosition);
                }
                else
                {
                    //Pooler.i.RecreatePools();
                    
                    MapGenerator.i.Generate(nextLocation);
                }
                TileUpdater.FOVUpdateScheduled = true;
                TileUpdater.Update();
                LoadingScreen.i.Hide();
                return;
            }
        }

        
        
       
        Debug.Log("There is no staircase here!");
        
    }
    private void UpdateSystem()
    {
        
         World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerAbilitiesSystem>().Update();
    }
}
