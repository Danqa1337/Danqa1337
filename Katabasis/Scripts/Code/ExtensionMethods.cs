using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using System.Linq;
using JetBrains.Annotations;

public static class MathematicsExtensions
{

    public static float Magnitude(this float2 vector)
    {
        return math.sqrt(math.pow(vector.x, 2) + math.pow(vector.y, 2));
    }
    public static float SqrMagnitude(this float2 vector)
    {
        return math.pow(vector.x, 2) + math.pow(vector.y, 2);
    }
    public static float Magnitude(this int2 vector)
    {
        return math.sqrt(math.pow(vector.x, 2) + math.pow(vector.y, 2));
    }
    public static float SqrMagnitude(this int2 vector)
    {
        return math.pow(vector.x, 2) + math.pow(vector.y, 2);
    }
    public static Vector3 ToVector3(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }
    public static Vector3 ToVector3(this float2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }
    public static float2 ToFloat2(this Vector3 vector)
    {
        return new float2(vector.x, vector.y);
    }
    public static float2 ToFloat2(this Vector2 vector)
    {
        return new float2(vector.x, vector.y);
    }
    public static int2 ToMapPosition(this int index)
    {
        int x = Mathf.Max(index % 64, 0);
        int y = (index - x) / 64;
        return new int2(x, y);
    }
   
    public static int2 ToInt2(this Vector2 vector)
    {
        return new int2((int)vector.x, (int)vector.y);
    }
    public static int2 ToInt2(this Vector3 vector)
    {
        return new int2((int)vector.x, (int)vector.y);
    }
    public static Vector3 ToRealPosition(this float2 vector)
    {
        return new Vector3(vector.x, vector.y, vector.y * 10);
    }
    public static Vector3 ToRealPosition(this int2 vector)
    {
        return new Vector3(vector.x, vector.y, vector.y * 10);
    }
    
    public static int ToMapIndex(this float2 vector)
    {
        int mapSize = 64;
        int2 v = (int2)vector;
        if (v.x >= 0 && v.x < mapSize && v.y >= 0 && v.y < mapSize)
        {
            return v.y * mapSize + v.x;
        }
        else
        {
            return -1;
        }
    }
    public static int ToMapIndex(this int2 v)
    {
        int mapSize = 64;
        if (v.x >= 0 && v.x < mapSize && v.y >= 0 && v.y < mapSize)
        {
            return v.y * mapSize + v.x;
        }
        else
        {
            return -1;
        }
    }
    
    public static TileData ToTileData(this float2 vector)
    {
        return vector.ToMapIndex().ToTileData();
    }
    public static TileData ToTileData(this float2 vector, NativeArray<TileData> array)
    {
        return vector.ToMapIndex().ToTileData(array);
    }
    public static TileData ToTileData(this int2 vector)
    {
        return vector.ToMapIndex().ToTileData();
    }
    public static TileData ToTileData(this int2 vector,  NativeArray<TileData> array)
    {
        return vector.ToMapIndex().ToTileData(array);
    }
    public static TileData ToTileData(this int index)
    {
        if (index != -1) return TileUpdater.current[index];
        else return TileData.Null;
    }
    public static TileData ToTileData(this int index, NativeArray<TileData> array)
    {
        if (index != -1) return array[index];
        else return TileData.Null;
    }

    

    public static void Randomize(this float2 center, float range)
    {
        center.x += UnityEngine.Random.Range(-range, range);
        center.y += UnityEngine.Random.Range(-range, range);
    }
    public static float GetDistance(this int2 start, int2 target)
    {
        return (target - start).Magnitude();
    }
    public static float GetSqrDistance(this int2 start, int2 target)
    {
        return (target - start).SqrMagnitude();
    }
    public static float GetDistance(this TileData start, TileData target)
    {
        return (target - start).Magnitude();
    }
    public static float GetSqrDistance(this TileData start, TileData target)
    {
        return (target - start).SqrMagnitude();
    }
    

    public static void Clear(this Texture2D texture)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, UnityEngine.Color.clear);
            }
        }
        texture.Apply();
    }
    public static void SetAlpha(this Texture2D texture, float a)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                UnityEngine.Color color = texture.GetPixel(x, y);
                texture.SetPixel(x, y, new UnityEngine.Color(color.r, color.g, color.b,a));
            }
        }
        texture.Apply();
    }
}
public static class EntityExtensions
{
    public static bool IsPlayer(this Entity entity)
    {
        return entity == PlayerAbilitiesSystem.playerEntity && entity != Entity.Null;
    }
    public static bool IsInInvis(this Entity entity)
    {
        if(entity.HasComponent<HasEffectTag>())
        {
            foreach (var item in entity.GetBuffer<EffectElement>())
            {
                if(item.type == EffectType.Invisibility)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public static bool HasEffect(this Entity entity, EffectType type)
    {
        if (entity.HasBuffer<EffectElement>())
        {
            foreach (var item in entity.GetBuffer<EffectElement>())
            {
                if(item.Duration != 0 && item.type == type)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public static bool HasTag(this Entity entity, Tag tag)
    {
        return entity.GetTags().Contains(tag);
    }
    public static HashSet<EffectType> GetEffects(this Entity entity)
    {
        var effects = new HashSet<EffectType>();
        if (entity.HasBuffer<EffectElement>())
        {
            foreach (var item in entity.GetBuffer<EffectElement>())
            {
                effects.Add(item.type);
            }
        }
        return effects;
    }
    
    public static void ShowRenderer(this Entity entity)
    {
        if(entity.HasComponent<AnatomyComponent>())
        {
            foreach (var item in entity.GetComponentData<AnatomyComponent>().GetLoverHierarchy())
            {
                item.GetComponentObject<RendererComponent>().Show();
            }
        }
        else
        {
            Debug.Log("There is no anatomy");
        }
    }
    public static void HideRenderer(this Entity entity)
    {
        if(entity.HasComponent<AnatomyComponent>())
        {
            foreach (var item in entity.GetComponentData<AnatomyComponent>().GetLoverHierarchy())
            {
                item.GetComponentObject<RendererComponent>().Hide();
            }
        }
        else
        {
            Debug.Log("There is no anatomy");
        }
    }
    public static void Remove<T>(this NativeList<T> list, T element) where T : struct
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].Equals(element))
            {
                list.RemoveAtSwapBack(i);
                return;
            }
        }
       // Debug.Log(list.Contains(element));
    }
    public static void Remove<T>(this DynamicBuffer<T> list, T element) where T : struct
    {
        for (int i = 0; i < list.Length; i++)
        {
            if(list[i].Equals(element))
            {
                list.RemoveAtSwapBack(i);
            }
        }
    }

    public static void Update<T>(this ComponentSystem system) where T : ComponentSystem
    {
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<T>().Update();

    }
    public static bool Contains<T>(this DynamicBuffer<T> buffer, Entity element) where T : struct
    {
        InventoryBufferElement bufferElement = new InventoryBufferElement() { entity = element };
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].Equals(bufferElement)) return true;
        }
        return false;
    }
    public static bool Contains<T>(this DynamicBuffer<T> buffer, T element) where T : struct, IBufferElementData
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].Equals(element)) return true;
        }
        return false;
    }
    public static bool CanBeSwapedRightNow(this Entity passiveEntity, Entity activeEntity)
    {
        
        if (passiveEntity.CanBeSwapedPotentialy(activeEntity)) //is not null
        {
            if (activeEntity.IsPlayer())
            {
                return true;
            }
            else
            {
                var passiveAi = passiveEntity.GetComponentData<AIComponent>();
                var activeAi = activeEntity.GetComponentData<AIComponent>();

                if (passiveAi.target != activeAi.target)
                {
                    return true;
                }

            }

        }
        return false;
    }
    public static bool CanBeSwapedPotentialy(this Entity passiveEntity, Entity activeEntity)
    {

        if (passiveEntity != Entity.Null) //is not null
        {
            if (passiveEntity.HasComponent<AIComponent>()) //is creature
            {
                if (!passiveEntity.HasComponent<ImpulseComponent>() && !passiveEntity.HasComponent<IsGoingToSwapTag>() && !passiveEntity.HasComponent<MoveComponent>())
                {
                    if (!passiveEntity.IsHostile(activeEntity)) //is not hostile
                    {
                        var passiveAi = passiveEntity.GetComponentData<AIComponent>();
                        if (activeEntity.IsPlayer())
                        {
                            return true;
                        }
                        else
                        {
                            if (passiveAi.target == Entity.Null)
                            {
                                return true;
                            }
                            else
                            {
                                var activeAi = activeEntity.GetComponentData<AIComponent>();

                                if (!(activeAi.isFleeing && passiveAi.isFleeing)) //at least one creature is not fleeing
                                {
                                    if (!passiveEntity.HasEffect(EffectType.EngagedInBattle))
                                    {
                                        return true;
                                    }
                                }

                            }
                        }
                    }
                }

            }
            
        }
        return false;
    }
    public static TileData CurrentTile(this Entity entity)
    {
        
        if (entity.HasComponent<VirtualBodypart>())
        {
            return entity.GetComponentData<VirtualBodypart>().anatomyParrent.CurrentTile();
        }
        else
        {
            if(entity.HasComponent<AnatomyComponent>())
            {
                var root = entity.GetComponentData<AnatomyComponent>().GetRootPart();

                if(root.HasComponent<CurrentTileComponent>())
                {
                    //Debug.Log(entity.GetName() + " root is " + root.GetName());
                    return root.GetComponentData<CurrentTileComponent>().currentTileId.ToTileData();

                } 
            }
            else
            {
                return entity.GetComponentData<CurrentTileComponent>().currentTileId.ToTileData();
            }
            throw new Exception("There is no currentTileComponent");
            
        }
    }

    public static HashSet<Tag> GetTags(this Entity self)
    {
        var list = new HashSet<Tag>();
        foreach (var item in self.GetBuffer<TagBufferElement>())
        {
            list.Add(item.tag);
        }
        return list;
    }
    public static bool IsHostile(this Entity self, Entity entity)
    {
        if (self != Entity.Null && entity != Entity.Null)
        {
            if (self.HasComponent<CreatureComponent>())
            {
                var tags = entity.GetTags();
                var hostileTags = self.GetComponentData<CreatureComponent>().GetHostileTags();

                foreach (var tag in tags)
                {
                    if (hostileTags.Contains(tag))
                    {
                        return true;
                    }
                }

            }
        }

        return false;
    }
    public static void SetName(this Entity entity, string name)
    {
        //World.DefaultGameObjectInjectionWorld.EntityManager.SetName(entity,name);
    }
    public static void Destroy(this Entity entity)
    {
        World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(entity);
    }

    public static ObjectType GetObjectType(this Entity entity)
    {
        return entity.GetComponentData<CurrentTileComponent>().objectType;
    }
    public static string GetName(this Entity entity)
    {
        string name = "unnamed entity";
        if (entity.HasComponent<DescriptionComponent>())
        {
           name = entity.GetComponentData<DescriptionComponent>().Name;
        }
        return name;// World.DefaultGameObjectInjectionWorld.EntityManager.GetName(entity);
    }
    public static void AddBuffer<T>(this Entity entity) where T : struct, IBufferElementData
    {
        World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<T>(entity);
    }
    public static DynamicBuffer<T> GetBuffer<T>(this Entity entity) where T : struct, IBufferElementData
    {
        return World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<T>(entity);
    }
    public static T GetComponentData<T>(this Entity entity) where T : struct, IComponentData
    {
        if (entity.HasComponent<T>())
        {
            return World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<T>(entity);
            
        }
        throw new NullReferenceException("no such component on " + entity.GetName());
    }
    public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> list)
    {
        foreach (var item in list)
        {
            set.Add(item);
        }
    }
    public static T GetComponentObject<T>(this Entity entity)
    {

        return World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentObject<T>(entity);
    }
    public static void SetComponentData<T>(this Entity entity, T data) where T : struct, IComponentData
    {
        if (entity.HasComponent<T>())
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, data);
        }
        else
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(entity, data);
        }
    }
    public static void AddComponentData<T>(this Entity entity, T data) where T : struct, IComponentData
    {
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(entity, data);
    }
    public static void AddComponentData(this Entity entity, IComponentData d)
    {
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(entity, d);
    }
    public static void AddComponentObject(this Entity entity, object obj)
    {
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entity, obj);
    }
    public static bool HasComponent<T>(this Entity entity) where T : struct, IComponentData
    {
        return World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<T>(entity);
    }
    public static bool HasBuffer<T>(this Entity entity) where T : struct, IBufferElementData
    {
        return World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<T>(entity);
    }

    public static void RemoveComponent<T>(this Entity entity) where T : struct, IComponentData
    {
        if (entity.HasComponent<T>())
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.RemoveComponent<T>(entity);
        }
    }
   



}

public static class MonoBehaviourExtensions
{

    public static List<T> DecodeCharSeparatedEnums<T>(this string charSeparatedEnum, char separator = ',')
    {
        List<T> enums = new List<T>();
        if (charSeparatedEnum.Length > 0)
        {
            foreach (string str in charSeparatedEnum.Split(separator))
            {
                bool enumFound = false;
                foreach (T e in Enum.GetValues(typeof(T)))
                {

                    if (e.ToString() == str)
                    {
                        //Debug.Log(e.ToString());
                        enums.Add(e);
                        enumFound = true;
                        break;
                    }
                }

                if (!enumFound) throw new NullReferenceException("There is no such name " + str + " inside " + typeof(T).ToString() + " enum");

            }
        }
        return enums;

    } 
    public static T DecodeCharSeparatedEnumsAndGetFirst<T>(this string charSeparatedEnum, char separator = ',')
    {
        List<T> enums = new List<T>();
        if (charSeparatedEnum.Length > 0)
        {
            foreach (string str in charSeparatedEnum.Split(separator))
            {
                bool enumFound = false;
                foreach (T e in Enum.GetValues(typeof(T)))
                {

                    if (e.ToString() == str)
                    {
                        //Debug.Log(e.ToString());
                        enums.Add(e);
                        enumFound = true;
                        break;
                    }
                }

                if (!enumFound) throw new NullReferenceException("There is no such name " + str + " inside " + typeof(T).ToString() + " enum");

            }
        }

        if (enums.Count > 0) return enums[0];
        else
        {
            return (T)Enum.GetValues(typeof(T)).GetValue(0);
        }


    }
    public static void Instantiate(this MonoBehaviour behaviour, GameObject gameObject, int2 position, Quaternion rotation)
    {

        MonoBehaviour.Instantiate(gameObject, new Vector3(position.x, position.y, position.y * 10), rotation);
    }
}
public static class ShuffleListExtensions
{

    /// <summary>
    /// Shuffle the list in place using the Fisher-Yates method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Return a random item from the list.
    /// Sampling with replacement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RandomItem<T>(this IList<T> list)
    {
        if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot select a random item from an empty list");
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    public static T RandomItem<T> (this NativeArray<T> list) where T : struct
    {
        if (list.Length == 0) throw new System.IndexOutOfRangeException("Cannot select a random item from an empty nativeArray");
        return list[UnityEngine.Random.Range(0, list.Length)];
    }
    /// <summary>
    /// Removes a random item from the list, returning that item.
    /// Sampling without replacement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RemoveRandom<T>(this IList<T> list)
    {
        if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
        int index = UnityEngine.Random.Range(0, list.Count);
        T item = list[index];
        list.RemoveAt(index);
        return item;
    }

    
}

static public class AnimationCurveExtensions
{
    public static void AddKey(this AnimationCurve curve, Vector2 key)
    {
        Keyframe keyframe = new Keyframe();
        keyframe.time = key.x;
        keyframe.value = key.y;
        curve.AddKey(keyframe);
    }
    public static void MoveKey(this AnimationCurve curve, int index, Vector2 key)
    {
        Keyframe keyframe = new Keyframe();
        keyframe.time = key.x;
        keyframe.value = key.y;
        curve.MoveKey(index, keyframe);
    }
    public static void Clear(this AnimationCurve curve)
    {

        if (curve.keys.Length > 0)
        {


            for (int i = 0; i < curve.keys.Length; i++)
            {
                curve.RemoveKey(i);
            }
        }
    }
}