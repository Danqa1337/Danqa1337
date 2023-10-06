using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Entities;

public class EntitySerializer
{

    public static SerializableTileData SerizlizeTile(TileData tile)
    {
        

        var sTile = new SerializableTileData();
        tile.updateScheduled = true;
        sTile.tile = tile;
        sTile.floorLayer = DeconstructEntity(tile.FloorLayer);
        sTile.liquidLayer = DeconstructEntity(tile.LiquidLayer);       
        if(!tile.SolidLayer.HasComponent<PlayerTag>())
        {
            sTile.solidlayer = DeconstructEntity(tile.SolidLayer);
        }

        foreach (var entity in tile.DropLayer)
        {
            sTile.dropLayer.Add(DeconstructEntity(entity));
        }
        foreach (var entity in tile.GroundCoverLayer)
        {
            sTile.groundCover.Add(DeconstructEntity(entity));
        }

        return sTile;
    }
    public static DynamicObjectData DeconstructEntity(Entity entity)
    {
        
        if (entity == Entity.Null) return null;
        var data = new DynamicObjectData();
        data.name = entity.GetName();

        if (entity.HasComponent<IDComponent>())
        {
            var component = entity.GetComponentData<IDComponent>();
            data.id = component.ID;
            data.name = ItemDataBase.GetObject(component.ID).staticData.name;
        } 
        if (entity.HasComponent<CurrentTileComponent>())
        {
            var component = entity.GetComponentData<CurrentTileComponent>();
            data.position = entity.CurrentTile().position;
            data.objectType = component.objectType;
        } 
        if (entity.HasComponent<CreatureComponent>())
        {
            var component = entity.GetComponentData<CreatureComponent>();
            foreach (var tag in component.GetHostileTags())
            {
                data.enemyTags.Add(tag);
            }
            foreach (var tag in entity.GetBuffer<InterestTagBufferElement>())
            {
                data.enemyTags.Add(tag.tag);
            }

            data.STR = component.str;
            data.AGL = component.agl;
            data.maxHealth = component.MaxHealth;
            data.curHealth = component.currentHealth;
            data.alive = true;
        }
        else
        {
            data.alive = false;
        }

        if (entity.HasComponent<PhysicsComponent>())
        {
            var component = entity.GetComponentData<PhysicsComponent>();
            data.weight = component.mass;
            data.accuracy = component.accuracy;
            data.baseDamage = component.damage;
            data.resistance = component.resistance;
            data.scalingAGL = component.ScalingAGL;
            data.scalingSTR = component.ScalingSTR;
            
        }

        if (entity.HasComponent<Inventory>())
        {
            var component = entity.GetComponentData<Inventory>();
            foreach (var item in component.items)
            {
                data.itemsInInventory.Add(DeconstructEntity(item));
            }

        }

        if (entity.HasComponent<DurabilityComponent>())
        {
            var component = entity.GetComponentData<DurabilityComponent>();
            data.maxDurability = component.MaxDurability;
            data.curDurability = component.currentDurability;

        }
        //if (entity.HasComponent<EquipmentComponent>())
        //{
        //    var component = entity.GetComponentData<EquipmentComponent>();
            
        //    data.itemInMainHand = DeconstructEntity(component.itemInMainHand);
        //    data.itemInOffHand = DeconstructEntity(component.itemInOffHand);
        //    data.itemOnHead = DeconstructEntity(component.helmet);
        //    data.itemOnChest = DeconstructEntity(component.chestPlate);
          
        //}
        if (entity.HasComponent<AnatomyComponent>())
        {
            var component = entity.GetComponentData<AnatomyComponent>();
            foreach (var tag in component.GetMissingPartTags())
            {
                data.missingBodypart.Add(tag);
            }

            foreach (var part in component.GetBodypartsWithoutItself())
            {
                data.bodyParts.Add(DeconstructEntity(part));
            }

        }
        if (entity.HasComponent<RangedWeaponComponent>())
        {
            var component = entity.GetComponentData<RangedWeaponComponent>();
            data.curentReloadPahase = component.CurrentReloadPhase;
            data.power = component.Power;
        } 
        if (entity.HasComponent<StairsComponent>())
        {
            var component = entity.GetComponentData<StairsComponent>();
            data.transitionId = component.transitionId;
        }
        else
        {
            data.transitionId = -1;
        }

        data.rndSpriteNum = entity.GetComponentObject<RendererComponent>().randomSpriteNum;

        foreach (var tag in entity.GetBuffer<TagBufferElement>())
        {
            data.tags.Add(tag.tag);
        }
        if (entity.HasBuffer<EffectElement>())
        {
            

            foreach (var effect in entity.GetBuffer<EffectElement>())
            {
                data.activeEffects.Add(effect);
            }
        }

        return data;
    }
}
[System.Serializable]
public class SerializableTileData
{
    public TileData tile;
    public DynamicObjectData solidlayer;
    public DynamicObjectData floorLayer;
    public DynamicObjectData liquidLayer;
    public List<DynamicObjectData> groundCover = new List<DynamicObjectData>( );
    public List<DynamicObjectData> dropLayer = new List<DynamicObjectData>();
}

