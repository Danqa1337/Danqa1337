using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public static class Spawner
{
    private static List<(ObjectData, int2)> spawnSchedule = new List<(ObjectData, int2)>();
    public static void Update()
    {
        foreach (var item in spawnSchedule)
        {
            Spawn(item.Item1,item.Item2);
        }
        spawnSchedule.Clear();
    }
    public static void ScheduleSpawn(ObjectData data, int2 position )
    {
        Debug.Log("!");
        spawnSchedule.Add( (data, position));
    }
    public static Entity SpawnPlayer(float2 position)
    {
        return Spawn(ObjectDataFactory.GetPlayer(), position);
    }
    public static Entity SpawnPlayer()
    {
        var player = ObjectDataFactory.GetPlayer();
        return Spawn(player, player.dynamicData.position);
    }
    public static Entity Spawn(int id, float2 position)
    {
        return Spawn(ItemDataBase.GetObject(id), position);
    }
    public static Entity Spawn(string Name, float2 position)
    {
       return Spawn(ItemDataBase.GetObject(Name), position);
    }

    public static Entity Spawn(DynamicObjectData objectData)
    {
        if(objectData == null) return Entity.Null;
       return Spawn(objectData, objectData.position);
    }

    public static Entity Spawn(DynamicObjectData dynamicData, float2 position)
    {
        if(dynamicData == null) return  Entity.Null;
        var objectData = ItemDataBase.GetObject(dynamicData.name).Clone() as ObjectData;
        objectData.dynamicData = dynamicData;
        return Spawn(objectData, position);
    }
    public static Entity Spawn(ObjectData objectData, float2 position)
    {
       
        var staticData = objectData.staticData;
        var dynamicData = objectData.dynamicData;
        var obj = Pooler.Take("SimpleObject").GetComponent<EntityAuthoring>();
        var transform = obj.transform;
        var renderer = obj.bodyRenderer;
        
       
      
        var currentTile = position.ToTileData();
        var tags = dynamicData.tags;
        var objectType = dynamicData.objectType;
        var name = objectData.staticData.name;
        int spriteCount = staticData.sprites.Count;

        transform.position = position.ToRealPosition();
        transform.rotation = quaternion.identity;
        transform.localScale = new Vector3(1, 1, 1);
        renderer.transform.localScale = new Vector3(1, 1, 1);
        renderer.transform.position = transform.position;
        

        if (spriteCount > 0)
        {
            if (!staticData.hasSeamlessTexture)
            {
                var rndSpriteNum = 1;
                if(dynamicData.rndSpriteNum == -1)
                {
                    rndSpriteNum = Random.Range(0, spriteCount);
                }
                else
                {
                    rndSpriteNum = dynamicData.rndSpriteNum;
                }
                renderer.randomSpriteNum = rndSpriteNum;

                renderer.sprite = staticData.sprites[rndSpriteNum];
            }
            else
            {
                renderer.sprite = staticData.sprites[30];
            }

            renderer.spriteCenterOffset = staticData.spriteCenterOffset;
        }
        else renderer.sprite = ItemDataBase.i.missingTextureSprite;

        switch (dynamicData.objectType)
        {
            case ObjectType.Solid: 
                renderer.spritesSortingLayerName = "Objects";
                break;
            case ObjectType.Drop:
                renderer.spritesSortingLayerName = "Drop";
                break;
            case ObjectType.Floor:
                renderer.spritesSortingLayerName = "Floor";

                break;
            case ObjectType.Liquid:
                renderer.spritesSortingLayerName = "Liquid";
                break;
            case ObjectType.GroundCover:
                renderer.spritesSortingLayerName = "Floor";
                break;
            case ObjectType.Hovering:
                renderer.spritesSortingLayerName = "Hovering";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        if (Application.isPlaying)
        { 
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = dstManager.CreateEntity();

            entity.SetName(name);
            entity.AddComponentData(new IDComponent(objectData.id));
            obj.name = name;
            if(staticData.hasSeamlessTexture) entity.AddComponentData(new  SeamlessTextureTag());
            renderer.parent = entity;
            if (tags.Contains(Tag.Player))
            {
                PlayerAbilitiesSystem.playerEntity = entity;
            }


            entity.AddComponentObject(renderer);
            entity.AddComponentObject(obj);
            entity.AddComponentData(new CurrentTileComponent(currentTile.index, objectData));
            entity.AddBuffer<TagBufferElement>();
            foreach (var VARIABLE in dynamicData.tags)
            {
                entity.GetBuffer<TagBufferElement>().Add(new TagBufferElement(VARIABLE));
            }

            entity.AddBuffer<InterestTagBufferElement>();
            foreach (var VARIABLE in dynamicData.enemyTags)
            {
                entity.GetBuffer<InterestTagBufferElement>().Add(new InterestTagBufferElement(VARIABLE));
            }
                    

              

            switch (objectType)
            {
                case ObjectType.Solid:
                    if (tags.Contains(Tag.LOSblock)) entity.AddComponentData(new LOSBlockTag());
                    entity.AddComponentData(new PhysicsComponent(objectData));
                    entity.AddComponentData(new DurabilityComponent(objectData, entity));
                    entity.AddComponentData(new DescriptionComponent(objectData));
                    entity.AddComponentObject(transform);
                    entity.AddComponentData(new AnatomyComponent(entity, objectData.staticData.bodyPartTag));

                    

                    currentTile.SolidLayer = entity;


                    break;
                case ObjectType.Drop:
                    entity.AddComponentData(new PhysicsComponent(objectData));
                    entity.AddComponentData(new DurabilityComponent(objectData, entity));
                    entity.AddComponentObject(transform);
                    entity.AddComponentData(new DescriptionComponent(objectData));
                    entity.AddComponentData(new AnatomyComponent(entity, objectData.staticData.bodyPartTag));



                    currentTile.Drop(entity);

                    break;
                case ObjectType.Floor:

                    currentTile.FloorLayer = entity;
                    entity.AddComponentData(new DescriptionComponent(objectData));
                    entity.AddComponentObject(transform);
                    break;
                case ObjectType.Liquid:
                    currentTile.LiquidLayer = entity;
                    entity.AddComponentData(new DescriptionComponent(objectData));
                    entity.AddComponentObject(transform);
                    break;
                case ObjectType.GroundCover:
                    currentTile.GroundCoverLayer.Add(entity);
                    entity.AddComponentData(new DescriptionComponent(objectData));
                    entity.AddComponentObject(transform);
                    break;
                default: goto case ObjectType.Solid;
                    break;
            }
            entity.AddBuffer<AnimationElement>();
            entity.AddBuffer<DropElement>();
            foreach (var item in staticData.drop)
            {
                entity.GetBuffer<DropElement>().Add(item);
            }

            entity.AddBuffer<EffectElement>();
            if (staticData.effectOnConsumptionComponents.Count > 0)
            {
                entity.AddBuffer<EffectOnConsumptionElement>();

                foreach (var effect in staticData.effectOnConsumptionComponents)
                {
                    entity.GetBuffer<EffectOnConsumptionElement>().Add(effect);
    
                }
            }
            foreach (var effect in dynamicData.activeEffects)
            {
                EffectSystem.AddEffect(entity, effect);
            }



            if (tags.Contains(Tag.Container))
            {
                if (Application.isPlaying)
                {
                    entity.AddComponentData(new ContainerComponent());
                    entity.AddComponentData(new Inventory(entity));
                    var inventory = entity.GetComponentData<Inventory>();
                    var item = Spawn(ItemDataBase.GetRandomItem(
                        DungeonStructure.CurrentLocation.level + 2, Biome.Any, new List<Tag>() {Tag.Crafted}
                        ).objectData, position);
                    currentTile.Remove(item);
                    inventory.Add(item);
                }
            }

            if (tags.Contains(Tag.Food))
            {
                entity.AddComponentData(new EatableComponent((int)(dynamicData.weight * 3f)));
            }
            if (!string.IsNullOrEmpty(objectData.staticData.blood))
                entity.AddComponentData(
                    new InternalLiquidComponent(ItemDataBase.GetObject(objectData.staticData.blood).id));
            if (tags.Contains(Tag.Wall)) entity.AddComponentData(new LOSBlockTag());
            if(tags.Contains(Tag.RangedWeapon)) entity.AddComponentData(new RangedWeaponComponent(objectData));

            if (entity.HasComponent<AnatomyComponent>())
            {
                var anatomy = entity.GetComponentData<AnatomyComponent>();
                entity.AddBuffer<DefaultBodypartBufferElement>();
                entity.AddBuffer<MissingBodypartBufferElement>();

                for (int i = 0; i < dynamicData.bodyParts.Count; i++)
                {
                    var newPartEntity = Spawn(dynamicData.bodyParts[i], position);

                    anatomy.AttachPart(newPartEntity);
                    currentTile.Remove(newPartEntity);
                    entity.GetBuffer<DefaultBodypartBufferElement>().Add(new DefaultBodypartBufferElement {tag =  newPartEntity.GetComponentData<AnatomyComponent>().bodyPartTag});
                }


                




                if (tags.Contains(Tag.Creature) && dynamicData.alive)
                {

                    if (tags.Contains(Tag.Player))
                    {
                        entity.AddComponentData(new PlayerTag());
                        MainCameraHandler.i.transform.SetParent(transform);
                        MainCameraHandler.i.transform.position = transform.position;
                    }
                    else if(!tags.Contains(Tag.Dummy)) entity.AddComponentData(new AIComponent(entity,0));

                    

                    entity.AddComponentData(new EquipmentComponent(entity, objectData));
                    entity.AddComponentData(new Inventory(entity));
                    

                    if (dynamicData.itemsInInventory.Count > 0)
                    {
                        var inventory = entity.GetComponentData<Inventory>();
                        foreach (var item in dynamicData.itemsInInventory)
                        {
                            var itemEntity = Spawn(item,position);
                            currentTile.Remove(itemEntity);
                            inventory.Add(itemEntity);
                        }
                    }

                    var fists = dstManager.CreateEntity();
                    fists.AddComponentData(new PhysicsComponent()
                    {
                        damage = objectData.staticData.fistSharpness,
                        mass = math.max(2, dynamicData.weight /20),
                        resistance = 0f,
                        accuracy = 90,
                        ScalingSTR = Scaling.C,
                        ScalingAGL = Scaling.C,
                        bodyPart = BodyPartTag.Fists,

                    });
                    fists.AddComponentData(new DurabilityComponent(-1, fists));
                    fists.AddComponentData(new DescriptionComponent(name + "'s fists"));
                    fists.AddComponentData(new AnatomyComponent(fists, BodyPartTag.Fists));
                    fists.AddComponentData(new VirtualBodypart(entity));
                    fists.AddBuffer<TagBufferElement>();
                    var creatureComponent = new CreatureComponent(entity, objectData, fists);

                    entity.AddComponentData(creatureComponent);


                    
                    var equip = entity.GetComponentData<EquipmentComponent>();
                    if(equip.itemInMainHand != Entity.Null) equip.EquipItem(equip.itemInMainHand, EquipPice.Weapon);
                    else
                    {
                        var itemInMainHand = Spawn(dynamicData.itemInMainHand, position);
                        
                        equip.EquipItem(itemInMainHand, EquipPice.Weapon);
                        currentTile.Remove(itemInMainHand);
                    }

                    if (equip.itemInOffHand != Entity.Null) equip.EquipItem(equip.itemInOffHand, EquipPice.Shield);
                    else
                    {
                        var itemInOffHand = Spawn(dynamicData.itemInOffHand, position);
                        
                        equip.EquipItem(itemInOffHand, EquipPice.Shield);
                        currentTile.Remove(itemInOffHand);

                    }
                    if (equip.helmet != Entity.Null) equip.EquipItem(equip.helmet, EquipPice.Headwear);
                    else
                    {
                        var itemOnHead = Spawn(dynamicData.itemOnHead, position);
                        
                        equip.EquipItem(itemOnHead, EquipPice.Headwear);
                        currentTile.Remove(itemOnHead);

                    }
                    if (equip.chestPlate != Entity.Null)
                    {
                        equip.EquipItem(equip.chestPlate, EquipPice.Chestplate);
                    }
                    else
                    {
                        var itemOnChest = Spawn(dynamicData.itemOnChest, position);

                        equip.EquipItem(itemOnChest, EquipPice.Chestplate);
                        currentTile.Remove(itemOnChest);
                    }
                    currentTile.Remove(equip.chestPlate);
                    currentTile.Remove(equip.itemInMainHand);
                    currentTile.Remove(equip.itemInOffHand);
                    currentTile.Remove(equip.helmet); 
                   
                    



                }



            }


            
            //if(currentTile.visible || tags.Contains(Tag.Player))
            //{
            //    if(entity.HasComponent<AnatomyComponent>())
            //    {
            //        foreach (var item in entity.GetComponentData<AnatomyComponent>().GetLoverHierarchy())
            //        {
            //            item.GetComponentObject<RendererComponent>().Show();
            //        } 
            //    }
            //}
            //else
            //{
            //    if (entity.HasComponent<AnatomyComponent>())
            //    {
            //        foreach (var item in entity.GetComponentData<AnatomyComponent>().GetLoverHierarchy())
            //        {
            //            item.GetComponentObject<RendererComponent>().Hide();
            //        }
            //    }
            //}



            currentTile.Save();
            if (tags.Contains(Tag.Player))
            {
                Debug.Log("player spawned on" + transform.position);
                PlayersInventoryInterface.i.UpdateStats();

            }
            return entity;
        }
        return  Entity.Null;
        
    }




}
