using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Random = UnityEngine.Random;

public struct Joint : IBufferElementData
{
    public static Joint Null;



    public float initialJointForce;

    public bool IsInternalJoint => child.GetComponentData<AnatomyComponent>().bodyPartTag != BodyPartTag.Body;


    [SerializeField]private Entity _parent;
    [SerializeField]private Entity _child;
    public Entity parent { get => _parent; }
    public Entity child { get => _child; }

    public override bool Equals(object obj)
    {
        return (obj is Joint joint && joint.child == _child && joint.parent == _parent);
    }
    public static bool operator ==(Joint joint1, Joint joint2)
    {
        return joint1.child == joint2._child && joint1.parent == joint2._parent;
    }
    public static bool operator !=(Joint joint1, Joint joint2)
    {
        return !(joint1.child == joint2._child && joint1.parent == joint2._parent);
    }

    public Joint(Entity _parent, Entity _child, float initialJointForce)
    {
        if(_parent == null) throw new System.NullReferenceException("Trying to create joint with null parent");
        if(_child == null) throw new System.NullReferenceException("Trying to create joint with null child");

        this.initialJointForce = initialJointForce;
        this._parent = _parent;
        this._child = _child;

        if (_child.GetBuffer<parentJoint>().Length > 0) throw new Exception(child.GetName() + " already has a rootJoint");
        this._parent.GetBuffer<Joint>().Add(this);
        this._child.GetBuffer<parentJoint>().Add(new parentJoint(this));

        Debug.Log("A new joint between " + _parent.GetName() + " and " + _child.GetName() + " with force of " + initialJointForce+ " has been created");

    }
    public void Destroy()
    {
        var parentAnatomy = _parent.GetComponentData<AnatomyComponent>();
        var childAnatomy = _child.GetComponentData<AnatomyComponent>();
        

        _child.GetComponentObject<Transform>().SetParent(null);

        if (parentAnatomy.GetDefaultBodypartTags().Contains(childAnatomy.bodyPartTag))
        {
            parent.GetBuffer<MissingBodypartBufferElement>().Add(new MissingBodypartBufferElement { tag = childAnatomy.bodyPartTag });
//            DurabilitySystem.ChangeDurability(parent, child.GetComponentData<DurabilityComponent>().currentDurability);

        }
        child.CurrentTile().Drop(child);
        parent.GetBuffer<Joint>().Remove(this);
        child.GetBuffer<parentJoint>().Clear();
        
        foreach (var item in child.GetComponentData<AnatomyComponent>().GetChildJoints())
        {
            item.Destroy();
        }

        Debug.Log("A joint between " + _parent.GetName() + " and " + _child.GetName() + " was destroyed");
    }
    public bool TryToBreak(float force, Entity responsibleEntity)
    {

        if (!parent.GetComponentData<AnatomyComponent>().GetRootPart().HasTag(Tag.ImmuneToDismemberment))
        {
            if (force > 0)
            {
                var maxDur = child.GetComponentData<DurabilityComponent>().MaxDurability;
                var curDur = child.GetComponentData<DurabilityComponent>().currentDurability;
                var jointForce = initialJointForce * ((float)curDur / (float)maxDur);
                Debug.Log("A joint between " + _parent.GetName() + " and " + _child.GetName() + " has force of " + jointForce + " and experiencing force of " + force);



                if (force > jointForce)
                {
                   // if (child.HasComponent<InternalLiquidComponent>()) EffectSystem.AddEffect(child, EffectType.Bleeding, 25);
                    //if (parent.HasComponent<InternalLiquidComponent>()) EffectSystem.AddEffect(parent, EffectType.Bleeding, 25);

                    child.GetComponentData<AnatomyComponent>().SpillLiquid(child.CurrentTile());
                    Destroy();
                    
                    var tile = parent.CurrentTile();
                    tile.Drop(child);
                    var h = Random.Range(1, 4);
                    child.AddComponentData(new ImpulseComponent(UnityEngine.Random.insideUnitCircle, 0, h, responsibleEntity));



                    return true;
                }
                //else
                //{
                //    initilJointForce -= force;
                //    parent.GetBuffer<Joint>().Remove(this);
                //    child.GetBuffer<parentJoint>().Clear();

                //    new Joint(parent, child, initilJointForce);
                //}
            }
        }

        return false;
    }

}
public struct parentJoint : IBufferElementData
{
    public Joint joint;

    public parentJoint(Joint joint)
    {
        this.joint = joint;
    }
}

