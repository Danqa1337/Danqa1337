using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class DeepDescriber : Describer
{
    
    public Label Description;
    public Label NameLabel;
    public RawImage portrait;
    public PhotoCamera portraitCamera;
    public CircularList<Entity> Objects;
    public Button leftButton, rightButton;
    private static DeepDescriber _i;
    public static DeepDescriber i
    {
        get
        {
            if (_i == null) _i = FindObjectOfType<DeepDescriber>();
            return _i;
        }
    }
    
    public void DescribeTile(TileData tile)
    {
        if (tile != TileData.Null)
        {
            Objects = new CircularList<Entity>();
            portraitCamera.transform.position = tile.position.ToRealPosition() + new Vector3(0, 0.5f, 0);

            if (tile.SolidLayer != Entity.Null && !tile.SolidLayer.IsInInvis())
            {
                Objects.Add(tile.SolidLayer);
            }

            foreach (var item in tile.DropLayer)
            {
                /*if (!item.IsInInvis)*/
                Objects.Add(item);
            }

            if (tile.LiquidLayer != Entity.Null) Objects.Add(tile.LiquidLayer);


            foreach (var item in tile.GroundCoverLayer)
            {
                /*if (!item.IsInInvis && item != tile.FloorLayer[0])*/
                Objects.Add(item);
            }

            if (tile.FloorLayer != Entity.Null) Objects.Add(tile.FloorLayer);


            leftButton.interactable = Objects.Count > 1;
            rightButton.interactable = Objects.Count > 1;
            if (Objects.Count > 0)
            {
                DescribeThing(Objects[0]);
            }
            else
            {
                NameLabel.SetText("Abyss");
                Description.SetText("You can not see the bottom. Who knows how deep is it?");
            }
        }
    }
    

    public void DescribeThing(Entity thing)
    {
        if (thing == null) throw new System.ArgumentNullException("trying to describe null");

        portraitCamera.MakePhoto(thing);
        
        NameLabel.SetText(thing.GetComponentData<DescriptionComponent>().Name);

        Cases cases = new Cases(thing);
        string text = thing.GetComponentData<DescriptionComponent>().Description;
        var objecType = thing.GetComponentData<CurrentTileComponent>().objectType;

        switch (objecType)
        {
            case ObjectType.Solid:
                if (thing.HasComponent<CreatureComponent>())
                {
                    text += BehaviourParametersDescription();
                    text += EquipParametersDescription();

                }
                text += PhysicalParametersDescription();
                text += SpatialParametersDescription();
                text += EffectsDescription();

                break;
            case ObjectType.Drop:
                goto case ObjectType.Solid;
                break;
            case ObjectType.Floor:
                break;
            case ObjectType.Liquid:
                break;
            case ObjectType.GroundCover:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Description.SetText(text);

        string PhysicalParametersDescription()
        {
            string _string = "";

            var anatomy = thing.GetComponentData<AnatomyComponent>();
            var physics = thing.GetComponentData<PhysicsComponent>();
            _string += "\n In comparison with you " + cases.nominative + " is  " + physics.size.ToString().ToLower();
            _string += "\n" + cases.nominative + " weights about " + RoundFloat(physics.mass) + " kilos";

            //if (physics.mass > Player.i.maxWeightCreatureCanLift * 2f) _string += ", that is defenetly too much weight for you to even budge";
            //else
            {
                //if (physics.mass > Player.i.maxWeightCreatureCanLift * 0.5) _string += ", that seems that you can budge " + cases.accusative + " with some effort";
               // else if (thing.Weight > Player.i.maxWeightCreatureCanLift * 0.1) _string += ", that seems that you can easily lift " + cases.accusative;
                //else _string += " - so light, that you could juggle with it if you knew how to" + cases.accusative;
                //if (thing.IsScrewedToTheFloor) _string += ", but seems connected to the ground so tightly that would rather break than let you move " + cases.accusative + " from " + cases.genitive + " place.";
               // else _string += ". " + cases.genitive + " movement is not constrained in any way.";
            }

            if(thing.HasComponent<CreatureComponent>())
            {
                var creature = thing.GetComponentData<CreatureComponent>();
                var healthPercent = creature.GetHealthPercent;

                for (int i = 25; i <= 100; i += 25)
                {
                    if (healthPercent <= i)
                    {
                        if (i == 25) _string += "\n" + cases.nominative + " looks near death";
                        if (i == 50) _string += "\n" + cases.nominative + " looks severely wounded ";
                        if (i == 75) _string += "\n" + cases.nominative + " looks wounded ";
                        if (i == 100) _string += "\n" + cases.nominative + " looks healthy";
                        break;
                    }

                }
            }
            var parts = anatomy.GetBodyPartsAndItself();
            if (parts.Count > 1)
            {
                foreach (var part in parts)
                {
                    _string += "\n " + cases.genitive + " " + part.GetComponentData<AnatomyComponent>().bodyPartTag + " " 
                        + getDurabilityDescription(part);
                }
            }
            else
            {
                _string += "\n " + cases.nominative + " " + getDurabilityDescription(thing);
            }

            
            //if (thing.flammabilityTreshold < 300) _string += "\n" + cases.nominative + " can burn well";

            string getDurabilityDescription(Entity entity)
            {
                var durability = entity.GetComponentData<DurabilityComponent>();
                var durPercent = durability.GetDurabilityPercent;
                string durabilityDescription = "";
                for (int i = 25; i <= 100; i += 25)
                {
                    if (durPercent <= i)
                    {
                        if (i == 25) durabilityDescription =  " looks trashed";
                        if (i == 50) durabilityDescription = " looks severely damaged ";
                        if (i == 75) durabilityDescription =  " looks damaged ";
                        if (i == 100) durabilityDescription =  " looks intact";
                        break;
                    }

                }
                return durabilityDescription;
            }
            return _string;
        }
        string SpatialParametersDescription()
        {
            string _string = "";
            if (!thing.HasComponent<ImpulseComponent>())
            {
                var objectType = thing.GetComponentData<CurrentTileComponent>().objectType;
                if (objectType == ObjectType.Solid) _string += "\n" + cases.nominative + " is not affected by any external physical forces.";
                if (objectType == ObjectType.Drop) _string += "\n" + cases.nominative + " is laying still on the floor;";
            }
            else
            {

                //if (thing.V != 0 && !thing.AxelerationVector.Equals(Vector2.zero)) _string += "\n Beware! " + cases.nominative + " is swiftly moving over the ground affected by inertia";
                //else if (thing.H != 0) _string += "\n" + cases.nominative + " is floating above ground";

            }
            return _string;
        }
        string EquipParametersDescription()
        {
            var equip = thing.GetComponentData<EquipmentComponent>();
            var anatomy = thing.GetComponentData<AnatomyComponent>();
            string _string = "";
            if (equip.itemInMainHand != Entity.Null) _string += "\n" + cases.nominative + " weilds a " + equip.itemInMainHand.GetName();
            if (equip.chestPlate != Entity.Null) _string += "\n" + cases.nominative + " wears " + equip.chestPlate.GetName() + " on " + cases.genitive + "chest.";
            if (equip.helmet != Entity.Null) _string += "\n" + cases.nominative + " wears " + equip.helmet.GetName() + " on " + cases.genitive + " head.";
            if (equip.boots != Entity.Null) _string += "\n" + cases.nominative + " wears " + equip.boots.GetName() + " on  " + cases.genitive + "  feet.";

            if (anatomy.GetMissingPartTags().Count > 0)
            {
                foreach (var limb in anatomy.GetMissingPartTags())
                {
                    _string += "\n " + cases.genitive + " " + limb.ToString() + " is missing.";
                }
            }

            return _string;
        }
        string BehaviourParametersDescription()
        {
            var creature = thing.GetComponentData<CreatureComponent>();
            string _string = "";
            if (creature.GetHostileTags().Contains(Tag.Player)) _string += "\n" + cases.nominative + " is agressive agains you";
            float speedDiference = creature.baseMovementCost - PlayerAbilitiesSystem.playerEntity.GetComponentData<CreatureComponent>().baseMovementCost;
            if (speedDiference < 0) _string += "\n" + cases.nominative + " can move faster then you.";
            if (speedDiference == 0) _string += "\n" + cases.nominative + " can move as fast as you.";
            if (speedDiference > 0) _string += "\n" + "You move faster then " + cases.accusative + ".";
            return _string;
        }
        string EffectsDescription()
        {
            string _string = "";
            //if (thing.activeEffectTypes.Count > 0)
            //{
            //    //_string += "\n" + cases.nominative + " is affected by folowing effects:";
            //    //foreach (EffectType effect in creature.activeEffectTypes)
            //    //{
            //    //    _string += "\n" + effect.ToString();
            //    //}
            //}
            return _string;
        }
    }


    public void Next()
    {
        Debug.Log("next");
        DescribeThing(Objects.Next());
    }
    public void Previous()
    {
        DescribeThing(Objects.Prev());
    }
    private class Cases
    {
        public string nominative = "It";
        public string genitive= "Tts";
        public string accusative = "it";
        public Cases(Entity entity)
        {
            if (entity.HasComponent<CreatureComponent>())
            {
                var creature = entity.GetComponentData<CreatureComponent>();
                if (creature.gender == Gender.Male)
                {
                    nominative = "He";
                    genitive = "His";
                    accusative = "Him";
                }
                if (creature.gender == Gender.Female)
                {
                    nominative = "She";
                    genitive = "Her";
                    accusative = "Her";
                }
                    
                
            }
        }
    }
}
