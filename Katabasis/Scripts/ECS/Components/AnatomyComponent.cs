using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
public struct AnatomyComponent : IComponentData
{
    private Entity self;
    public BodyPartTag bodyPartTag;
    
    public HashSet<Entity> GetLoverHierarchy()
    {
        var list = new HashSet<Entity> { self };
        var joints = GetChildJoints();
        if (joints.Count > 0)
        {
            
            foreach (var joint in joints)
            {
                list.AddRange(joint.child.GetComponentData<AnatomyComponent>().GetLoverHierarchy());
            }
        }

        return list;
    }
    
    public List<Entity> GetUpperHierarchy()
    {
        var list = new List<Entity>() {self };
        if (self.GetBuffer<parentJoint>().Length > 0)
        {
            list.AddRange(self.GetBuffer<parentJoint>()[0].joint.parent.GetComponentData<AnatomyComponent>().GetUpperHierarchy());
        }
        return list;
    }
    public Entity GetParentPart()
    {
        var joints =self.GetBuffer<parentJoint>();
        if (joints.Length > 0) return joints[0].joint.parent;
        else
        {
            return Entity.Null;
        }
    }

    public Entity GetRootPart()
    {
        return GetUpperHierarchy().Last();
    }
    public List<Entity> GetBodyPartsAndItself()
    {
        var list = new List<Entity>();
        list.Add(self);
        foreach (var item in self.GetBuffer<Joint>())
        {
            list.Add(item.child);
        }
        
        return list;
    }

    public List<Entity> GetInternalBodyParts()
    {
        var list = new List<Entity>();
        foreach (var item in self.GetBuffer<Joint>())
        {
            if (item.IsInternalJoint) list.Add(item.child);
        }

        return list;
    }

    public List<Entity> GetExternalBodyParts()
    {
        var list = new List<Entity>();
        foreach (var item in self.GetBuffer<Joint>())
        {
            if(!item.IsInternalJoint)list.Add(item.child);
        }
        
        return list;
    }
    public List<Entity> GetBodypartsWithoutItself()
    {
        var parts = GetBodyPartsAndItself();
        parts.Remove(self);
        return parts;
    } 
    public Entity GetBodyPart(BodyPartTag bodyPartTag)
    {
        foreach (var item in GetBodyPartsAndItself())
        {
            if(item.GetComponentData<AnatomyComponent>().bodyPartTag == bodyPartTag)
            {
                return item;
            }
        }
        return Entity.Null;
    }
    public Entity GetPartThatCanHold(EquipPice equipPice)
    {

        switch (equipPice)
        {
            case EquipPice.Weapon:
                foreach (var item in GetBodyPartsAndItself())
                {
                    var tag = item.GetComponentData<PhysicsComponent>().bodyPart;
                    if (tag == BodyPartTag.RightArm)
                    {
                        return item;
                    }
                }
                break;

            case EquipPice.Shield:
                foreach (var item in GetBodyPartsAndItself())
                {
                    var tag = item.GetComponentData<PhysicsComponent>().bodyPart;
                    if (tag == BodyPartTag.LeftArm)
                    {
                        return item;
                    }
                }
                break;

            case EquipPice.Headwear:
                foreach (var item in GetBodyPartsAndItself())
                {
                    var tag = item.GetComponentData<PhysicsComponent>().bodyPart;
                    if (tag == BodyPartTag.Head)
                    {
                        return item;
                    }
                }
                break;

            case EquipPice.Chestplate:
                foreach (var item in GetBodyPartsAndItself())
                {
                    var tag = item.GetComponentData<PhysicsComponent>().bodyPart;
                    if (tag == BodyPartTag.Body)
                    {
                        return item;
                    }
                }
                break;

            case EquipPice.Boots:
                foreach (var item in GetBodyPartsAndItself())
                {
                    var tag = item.GetComponentData<PhysicsComponent>().bodyPart;
                    if (tag == BodyPartTag.Body)
                    {
                        return item;
                    }
                }
                break;

            case EquipPice.None:
                break;
            
        }

        return Entity.Null;

    }
    public List<BodyPartTag> GetMissingPartTags()
    {
        var buffer = self.GetBuffer<MissingBodypartBufferElement>();
        var list = new List<BodyPartTag>();
        foreach (var VARIABLE in buffer)
        {
            list.Add(VARIABLE.tag);
        }

        return list;
    }
    public List<BodyPartTag> GetDefaultBodypartTags()
    {
        var buffer = self.GetBuffer<DefaultBodypartBufferElement>();
        var list = new List<BodyPartTag>();
        foreach (var VARIABLE in buffer)
        {
            list.Add(VARIABLE.tag);
        }

        return list;
    }
    public Joint GetParentJoint()
    {
        var joints = self.GetBuffer<parentJoint>();
        if (joints.Length > 0) return joints[0].joint;
        else
        {
            return Joint.Null;
        }
    }
    public List<Joint> GetChildJoints()
    {
        var buffer = self.GetBuffer<Joint>();
        var list = new List<Joint>();
        foreach (var VARIABLE in buffer)
        {
            list.Add(VARIABLE);
        }

        return list;
    }
    public void AttachPart(Entity entity)
    {

        AttachPart(entity, GetJointForce(entity.GetComponentData<AnatomyComponent>().bodyPartTag));
    }
    private float GetJointForce(BodyPartTag bodyPartTag) => (bodyPartTag) switch
    {
        BodyPartTag.None => 0,
        BodyPartTag.Head => 80,
        BodyPartTag.RightArm => 60,
        BodyPartTag.LeftArm => 60,
        BodyPartTag.RightFrontLeg => 60,
        BodyPartTag.RightRearLeg => 60,
        BodyPartTag.LeftFrontLeg => 60,
        BodyPartTag.LeftRearLeg => 60,
        BodyPartTag.RightFrontPaw => 60,
        BodyPartTag.RightRearPaw => 60,
        BodyPartTag.LeftFrontPaw => 60,
        BodyPartTag.LeftRearPaw => 60,
        BodyPartTag.Body => 60,
        BodyPartTag.Tail => 60,
        BodyPartTag.Tentacle => 60,
        BodyPartTag.FirstTentacle => 60,
        BodyPartTag.SecondTentacle => 60,
        BodyPartTag.Fin => 30,
        BodyPartTag.FirstFin => 30,
        BodyPartTag.SecondFin => 30,
        BodyPartTag.RightClaw => 60,
        BodyPartTag.LeftClaw => 60,
        BodyPartTag.Teeth => 60,
        BodyPartTag.Fists => 0,
        BodyPartTag.LowerBody => 100,
    };
    public void AttachPart(Entity entity, float jointForce = 30)
    {
        new Joint(self, entity, jointForce);
        var tr = entity.GetComponentObject<Transform>();
        var parentTr = self.GetComponentObject<EntityAuthoring>().partsHolder;
        var ph = entity.GetComponentData<PhysicsComponent>();

        entity.SetComponentData(ph);
        tr.rotation = Quaternion.Euler(0,0,0);
        tr.position = parentTr.position;
        tr.SetParent(parentTr);
    }
    public void DropPart(Entity entity)
    {
        Debug.Log("!");
        self.CurrentTile().Drop(DetachPart(entity));
    }

    public void SpillLiquid(TileData tile)
    {
        if (self.HasComponent<InternalLiquidComponent>())
        {
            if (!tile.isAbyss)
            {
                var splatter = Spawner.Spawn(self.GetComponentData<InternalLiquidComponent>().liquidSpaltterId,
                    tile.position);
                var splatterTransform = splatter.GetComponentObject<Transform>();
                
                
                if(!tile.GetNeibors(true).Any(t=>t.isAbyss || t.HasLOSblock) )
                {
                    splatterTransform.position += UnityEngine.Random.insideUnitCircle.ToVector3();
                }
                

                splatterTransform.rotation = BaseMethodsClass.GetRandomRotation();
            }
        }
    }
    public AnatomyComponent(Entity parent, BodyPartTag bodyPartTag)
    {
        this.self = parent;
        this.bodyPartTag = bodyPartTag;
        parent.AddBuffer<parentJoint>();
        parent.AddBuffer<Joint>();
    }

    public Entity DetachPart(Entity part)
    {
        if (GetBodypartsWithoutItself().Contains(part))
        {
            var joint = part.GetBuffer<parentJoint>()[0];
            joint.joint.Destroy();
        }
        else
        {
            throw new NullReferenceException("Trying to remove part that doesn't exist in anatomy");
        }
        return part;

    }
    
}