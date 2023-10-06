using Unity.Collections;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System.Reflection;

public struct PhysicsComponent : IComponentData
{
    public Size size;
    public float mass;
    public float damage;
    public float resistance;
    public float accuracy;
    public float aerodynamicsDamageMultiplier;

    public Scaling ScalingSTR;
    public Scaling ScalingAGL;

    public EquipPice defaultEquipTag;
    public BodyPartTag bodyPart;

    public PhysicsComponent(ObjectData data)
    {
        this.size = (Size)data.staticData.size;
        this.mass = data.dynamicData.weight;
        this.damage = data.dynamicData.baseDamage;
        this.accuracy = data.dynamicData.accuracy;
        this.resistance = data.dynamicData.resistance;
        this.ScalingAGL = data.dynamicData.scalingAGL;
        this.ScalingSTR = data.dynamicData.scalingSTR;
        this.aerodynamicsDamageMultiplier = data.staticData.aerodynamicsDamageMultiplier;
        defaultEquipTag = data.staticData.defaultEquipTag;
        bodyPart = data.staticData.bodyPartTag;
    }

}

public struct ImpulseComponent : IComponentData
{
    public Entity responsibleEntity;
    public float2 axelerationVector;
    public float2 err;
    public float H;
    public int bonusDamage;
    
    public TileData calculateNextTile(TileData currentTile)
    {
        float2 err;
        return calculateNextTile( currentTile, out err);
    }
    public TileData calculateNextTile(TileData currentTile, out float2 error)
    {
        var step = math.normalize(axelerationVector);

        var oldPos = currentTile.position;
        var newPosfloat = oldPos + err + step;
        var newPosInt = new int2(Mathf.RoundToInt(newPosfloat.x), Mathf.RoundToInt(newPosfloat.y));
        var difference = newPosfloat - newPosInt;
        error = difference;
        return newPosInt.ToTileData();
    }
    public ImpulseComponent(float2 axelerationVector, int bonusDamage, float h, Entity responsibleEntity)
    {
        this.axelerationVector = axelerationVector;
        this.err = 0;
        this.bonusDamage = bonusDamage;
        H = h;
        this.responsibleEntity = responsibleEntity;
    }
}
public struct PlayerTag : IComponentData
{

}

public struct RangedWeaponComponent : IComponentData
{
    public int MaxReloadingPhases;
    public int CurrentReloadPhase;
    public int Power;
    public int AmmoId;
    public bool Ready => CurrentReloadPhase == MaxReloadingPhases;

    public RangedWeaponComponent(ObjectData data)
    {
        MaxReloadingPhases = data.staticData.MaxReloadingPhases;
        CurrentReloadPhase = data.dynamicData.curentReloadPahase;
        Power = data.dynamicData.power;
        AmmoId = ItemDataBase.GetObject("Arrow").id;
    }

}

public struct DescriptionComponent : IComponentData
{
    private FixedString64 _firstName;
    private FixedString512 _description;
    public int upgrade;

    public DescriptionComponent(ObjectData param)
    { 
        
        this._firstName = new FixedString64(param.staticData.realName);
        this._description = new FixedString512(param.staticData.description);
        this.upgrade = 0;
    }
    public DescriptionComponent(string name)
    {
        this._firstName = new FixedString64(name);
        this._description = new FixedString512("");
        this.upgrade = 0;
    }

    public string Description => _description.Value;
    public string Name
    {
        get
        {
            string s = _firstName.Value;

            if (upgrade > 0) s = s + " +" + upgrade;
            if (upgrade < 0) s = s + " " + upgrade;
            return s;
        }
    }
}
public struct DurabilityComponent : IComponentData
{
    private Entity self;
    public int currentDurability;
    public int MaxDurability;
    public float GetDurabilityPercent => ((float)currentDurability / (float)MaxDurability) * 100f;

    public DurabilityComponent(ObjectData param, Entity self)
    {
        currentDurability = param.dynamicData.maxDurability;
        MaxDurability = param.dynamicData.maxDurability;
        this.self = self;
    }
    public DurabilityComponent(int durability, Entity self)
    {
        currentDurability = durability;
        MaxDurability =durability;
        this.self = self;

    }
    



}

public struct DurabilityChangedTag : IComponentData
{

}
public enum MovemetType
{
    SelfPropeled,
    Forced,
}


public struct MoveComponent : IComponentData
{
    public int prevTileId;
    public int nextTileId;
    public MovemetType movemetType;

    public MoveComponent(TileData prevTile, TileData nextTile, MovemetType movemetType)
    {
        this.prevTileId = prevTile.index;
        this.nextTileId = nextTile.index;
        this.movemetType = movemetType;
    }
}
public struct CurrentTileComponent : IComponentData
{
    
    public int currentTileId;
    public ObjectType objectType;
    public CurrentTileComponent(int currentTileId, ObjectData objectData)
    {
        this.objectType = objectData.dynamicData.objectType;
        this.currentTileId = currentTileId;
    }
    public CurrentTileComponent(int currentTileId, ObjectType objectType )
    {
        this.objectType = objectType;
        this.currentTileId = currentTileId;
    }
    
    

    //public TileData currentTile
    //{
    //    get => currentTileId.ToTileData();
    //    set
    //    {
    //        currentTileId = value.index;
    //    }

    //}

}
public struct AIComponent : IComponentData
{
    
    public Entity self;
    public float morale;
    public Entity target;
    public int abilityCooldown;
    public int targetSearchCooldown;

    public bool AbilityReady => abilityCooldown == 0;
    public Entity itemInMainHand => self.GetComponentData<EquipmentComponent>().itemInMainHand;
    public Entity itemInOffHand => self.GetComponentData<EquipmentComponent>().itemInOffHand;
    public Entity chestPlate => self.GetComponentData<EquipmentComponent>().chestPlate;
    public Entity boots => self.GetComponentData<EquipmentComponent>().boots;
    public Entity helmet => self.GetComponentData<EquipmentComponent>().helmet;

    public bool isFleeing => morale < IAIBehaviour.moraleFleeTreshold;
    public void ChangeMorale(int num)
    {
        morale = math.clamp(morale + num, 0, 100);
    }

    public AIComponent(Entity self, int cooldown)
    {
        this.self = self;
        this.abilityCooldown = cooldown;
        this.targetSearchCooldown = 0;
        this.target = Entity.Null;
        this.morale = 100;
    }

}
public struct DefaultBodypartBufferElement : IBufferElementData
{
    public BodyPartTag tag;
}

public struct TagBufferElement : IBufferElementData
{
    public Tag tag;

    public TagBufferElement(Tag tag)
    {
        this.tag = tag;
    }
}
public struct InterestTagBufferElement : IBufferElementData
{
    public Tag tag;

    public InterestTagBufferElement(Tag tag)
    {
        this.tag = tag;
    }
}
public struct MissingBodypartBufferElement : IBufferElementData
{
    public BodyPartTag tag;
}

public struct DamagedByPlayerTag : IComponentData
{

}
public struct CreatureComponent : IComponentData
{
    public Entity self;
    public int viewDistance;
    public int str;
    public int agl;
    public int baseMovementCost;
    public int currentHealth { get => _currentHealth; }
    public int MaxHealth;

    public Gender gender;

    public Entity fists;

    public int XPOnDeath;
    [SerializeField] private int _currentHealth;

    public float GetHealthPercent => ( (float)currentHealth / (float)MaxHealth) * 100f;
    public void ChangeHealth(float num)
    {



        if (currentHealth != -1)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + (int)num, 0, MaxHealth);
        }
        self.SetComponentData(this);
    }
    public CreatureComponent(Entity parent, ObjectData param, Entity fists)
    {
        this.viewDistance = param.staticData.viewDistance;
        this.str = param.dynamicData.STR;
        this.agl = param.dynamicData.AGL;
        this.MaxHealth = param.dynamicData.maxHealth;
        this._currentHealth = param.dynamicData.curHealth;
        this.self = parent;
        this.gender = param.staticData.gender;
        this.baseMovementCost = param.staticData.baseMovementCost;
        this.fists = fists;
        this.XPOnDeath = param.staticData.XPOnDeath;
    }

    public List<Tag> GetHostileTags()
    {
        var list = new List<Tag>();
        foreach (var item in self.GetBuffer<InterestTagBufferElement>())
        {
            list.Add(item.tag);
        }

        return list;
       

    }
}
public struct InternalLiquidComponent : IComponentData
{
    public int liquidSpaltterId;

    public InternalLiquidComponent(int liquidSpaltterId)
    {
        this.liquidSpaltterId = liquidSpaltterId;
    }
}

public struct EquipmentComponent : IComponentData
{
    public Entity self;
    public Entity itemInMainHand
    {
        get
        {
            var arm = anatomy.GetBodyPart(BodyPartTag.RightArm);
            if (arm != Entity.Null)
            {
                var parts = arm.GetComponentData<AnatomyComponent>().GetExternalBodyParts();
                if (parts.Count > 0)
                {
                    return parts[0];
                }
                
            }
            return Entity.Null;
        }
    }

    public Entity itemInOffHand
    {
        get
        {
            var arm = anatomy.GetBodyPart(BodyPartTag.LeftArm);
            if (arm != Entity.Null)
            {
                var parts = arm.GetComponentData<AnatomyComponent>().GetExternalBodyParts();
                if (parts.Count > 0)
                {
                    return parts[0];
                }

            }
            return Entity.Null;
        }
    }
    public Entity chestPlate
    {
        get
        {
            var body = anatomy.GetBodyPart(BodyPartTag.Body);
            if (body != Entity.Null)
            {
                var parts = body.GetComponentData<AnatomyComponent>().GetExternalBodyParts();
                if (parts.Count > 0)
                {
                    return parts[0];
                }

            }
            return Entity.Null;
        }
    }
    public Entity helmet
    {
        get
        {
            var head = anatomy.GetBodyPart(BodyPartTag.Head);
            if (head != Entity.Null)
            {
                var parts = head.GetComponentData<AnatomyComponent>().GetExternalBodyParts();
                if (parts.Count > 0)
                {
                    return parts[0];
                }

            }
            return Entity.Null;
        }
    }
    public Entity boots;

    

    public float2 rightArmHolderOffset;
    public float2 leftArmHolderOffset;
    public float2 headHolderOffset;
    public float2 chestHolderOffset;
    public float2 legsHolderOffset;
    AnatomyComponent anatomy => self.GetComponentData<AnatomyComponent>();

    public EquipmentComponent(Entity self, ObjectData data)
    {
        this.self = self;
        this.boots = Entity.Null;

        rightArmHolderOffset = data.staticData.rightArmHolderOffset;
        leftArmHolderOffset = data.staticData.leftArmHolderOffset;
        headHolderOffset = data.staticData.headHolderOffset;
        chestHolderOffset = data.staticData.chestHolderOffset;
        legsHolderOffset = data.staticData.legsHolderOffset;


    }

    private Entity GetEquipPice(EquipPice tag) => tag switch
    {
        EquipPice.Weapon =>itemInMainHand,
        EquipPice.Chestplate => chestPlate,
        EquipPice.Shield => itemInOffHand,
        EquipPice.Boots => boots,
        EquipPice.Headwear => helmet,
        _ => itemInMainHand,

    };


    public bool CanUseEquipPice(EquipPice tag)
    {

        return self.GetComponentData<AnatomyComponent>().GetPartThatCanHold(tag) != Entity.Null;
       // return GetThingHolder(tag) != null;
    }
    public bool CanUseEquipPice(Entity item)
    {
        var tag = item.GetComponentData<PhysicsComponent>().defaultEquipTag;
        return self.GetComponentData<AnatomyComponent>().GetPartThatCanHold(tag) != Entity.Null;
       // return GetThingHolder(tag) != null;
    }
    public Entity EquipItem(Entity item, EquipPice equipTag, bool updateSlot = true)
    {
        if (item != Entity.Null)
        {
            if (CanUseEquipPice(equipTag))
            {
                var inventory = self.GetComponentData<Inventory>();
                var anatomy = self.GetComponentData<AnatomyComponent>();
                var part = anatomy.GetPartThatCanHold(equipTag);
                var partAnatomy = part.GetComponentData<AnatomyComponent>();
                var renderer = item.GetComponentObject<RendererComponent>();
                var transform = item.GetComponentObject<Transform>();

                if(inventory.items.Contains(item))
                {
                    inventory.DropOut(item);
                }
                item.CurrentTile().Remove(item);

                if (GetEquipPice(equipTag) != Entity.Null) UnequipItem(equipTag);


                
                if(item.GetComponentData<AnatomyComponent>().GetParentJoint() != Joint.Null)
                {
                    item.GetComponentData<AnatomyComponent>().GetParentJoint().Destroy();

                }
                 
                partAnatomy.AttachPart(item, GetJointForce(equipTag));
                
                
                renderer.spritesSortingLayerName = "Objects";

                float2 offset = GetOffset(equipTag);
                if(equipTag == EquipPice.Weapon || equipTag == EquipPice.Shield)
                {
                    offset += renderer.spriteCenterOffset.ToFloat2();
                }
                transform.localPosition = new Vector3(offset.x, offset.y, GetRendererZ(equipTag));
                renderer.transform.localPosition = Vector3.zero;
                if (updateSlot && self == PlayerAbilitiesSystem.playerEntity)
                {
                    PlayersInventoryInterface.i.GetEquipSlot(equipTag).PlaceItemFromComponent(item);
                }
                

                



                return item;
            }
            else throw new ArgumentOutOfRangeException(self.GetName() + " trying to equip " + item + " as " + equipTag + " , but can not hold It");
        }
        return Entity.Null;
    }
    private int GetRendererZ(EquipPice tag) => tag switch
    {
        EquipPice.Weapon => -3,
        EquipPice.Shield => -2,
        EquipPice.Headwear => -1,
        EquipPice.Chestplate => -1,
        EquipPice.Boots => -1,
        EquipPice.None => -1
    };
    private float2 GetOffset(EquipPice tag) => tag switch
    {
        EquipPice.Weapon => rightArmHolderOffset,
        EquipPice.Shield => leftArmHolderOffset,
        EquipPice.Headwear => headHolderOffset,
        EquipPice.Chestplate => chestHolderOffset,
        EquipPice.Boots => legsHolderOffset,
        EquipPice.None => rightArmHolderOffset
    };
    private float GetJointForce(EquipPice tag) => tag switch
    {
        EquipPice.Weapon => 150,
        EquipPice.Shield => 200,
        EquipPice.Headwear => 200,
        EquipPice.Chestplate => 800,
        EquipPice.Boots => 100,
        EquipPice.None => 0
    };


    public Entity UnequipItem(Entity thing, bool updateSlot = true)
    {
        if (thing != Entity.Null)
        {
            if (itemInMainHand == thing) UnequipItem(EquipPice.Weapon, updateSlot);
            if (itemInOffHand == thing) UnequipItem(EquipPice.Shield, updateSlot);
            if (helmet == thing) UnequipItem(EquipPice.Headwear, updateSlot);
            if (chestPlate == thing) UnequipItem(EquipPice.Chestplate, updateSlot);
            if (boots == thing) UnequipItem(EquipPice.Boots, updateSlot);

        }
        return thing;
    }
    public Entity UnequipItem(EquipPice tag, bool updateSlot = true)
    {
        Entity item = GetEquipPice(tag);

        if (item != Entity.Null)
        {
            var holdingPart = anatomy.GetPartThatCanHold(tag);
            var renderer = item.GetComponentObject<RendererComponent>();
            holdingPart.GetComponentData<AnatomyComponent>().DetachPart(item);
            if (updateSlot && PlayerAbilitiesSystem.playerEntity == self)
            {
                PlayersInventoryInterface.i.GetEquipSlot(tag).Clear();
            }


            renderer.Z = 0;
            self.SetComponentData(this);
        }

        return item;
    }


    public List<Entity> GetEquipment()
    {
        List<Entity> things = new List<Entity>();
        if (chestPlate != Entity.Null) things.Add(chestPlate);
        if (helmet != Entity.Null) things.Add(helmet);
        if (boots != Entity.Null) things.Add(boots);
        if (itemInMainHand != Entity.Null) things.Add(itemInMainHand);
        if (itemInOffHand != Entity.Null) things.Add(itemInOffHand);
        return things;
    }
}  
public struct BodypartComponent : IComponentData
{
    public BodyPartTag bodyPartTag;
}

public struct Collision : IComponentData
{
    public Entity responsibleEntity;
    public Entity entity1;
    public Entity entity2;

    public Collision(Entity entity1, Entity entity2, Entity responsibleEntity)
    {
        this.entity1 = entity1;
        this.entity2 = entity2;
        this.responsibleEntity = responsibleEntity;
    }
}
public struct LOSBlockTag : IComponentData
{

}
public struct SeamlessTextureTag : IComponentData
{

}
public struct IDComponent : IComponentData
{
    public int ID;

    public IDComponent(int ID)
    {
        this.ID = ID;
    }
}

public struct StairsComponent : IComponentData
{
    public int transitionId;
}

public struct VirtualBodypart : IComponentData
{
    public Entity anatomyParrent;

    public VirtualBodypart(Entity anatomyParrent)
    {
        this.anatomyParrent = anatomyParrent;
    }
    
}

public struct DestroyTag : IComponentData
{

}
public struct DestroyImmediatelyTag : IComponentData
{

}


public struct EatableComponent : IComponentData
{
    public int nutrition;

    public EatableComponent(int nutrition)
    {
        this.nutrition = nutrition;
    }
}

public struct ContainerComponent : IComponentData
{

}

public struct IsGoingToSwapTag : IComponentData
{
    public Entity EntityToSwapWith;
    public bool swaped;

    public IsGoingToSwapTag(Entity entityToSwapWith)
    {
        EntityToSwapWith = entityToSwapWith;
        swaped = false;
    }
}
    
public struct CloudComponent : IComponentData
{

}
public struct DropElement : IBufferElementData
{
    public float chance;
    public int id;

    public DropElement(int id, float chance = 100)
    {
        this.chance = chance;
        this.id = id;
    }
}



