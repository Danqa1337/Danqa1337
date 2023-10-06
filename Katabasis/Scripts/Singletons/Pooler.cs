using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : Singleton<Pooler>
{
    private Dictionary<string, Queue<GameObject>> poolsDict;

    public List<Pool> pools;
    [System.Serializable] public class Pool
    {
        public GameObject Object;
        public int size;
    }
    public bool PoolContainsObject(string name)
    {
        if (poolsDict == null) RecreatePools();
        if(poolsDict.ContainsKey(name))
        {
            return true;
        }
        return false;
    }
    [ContextMenu("Clear")]
    
    public void Clear()
    {
       while (transform.childCount > 0)
       {
            DestroyImmediate(transform.GetChild(0).gameObject);
       }
       Debug.Log("Pool cleared");
    }
    [ContextMenu("Recreate Pools")]
    public void RecreatePools()
    {
        StartTest();
        MonoBehaviour[] allobjects = FindObjectsOfType<MonoBehaviour>(true);
        
        poolsDict = new Dictionary<string, Queue<GameObject>>();
        foreach (var pool in pools)
        {
            StartTest();
            Queue<GameObject> que = new Queue<GameObject>();
            foreach (var item in allobjects)
            {
                if(que.Count < pool.size && item.name == pool.Object.name)
                {
                    que.Enqueue(item.gameObject);
                    item.gameObject.SetActive(false);
                }
            }
            

            for (int i = que.Count; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.Object, transform);
                obj.SetActive(false);
                que.Enqueue(obj);
            } 
            poolsDict.Add(pool.Object.name, que);
        }
        EndTest("Pool updating took ");
    }
    public static void PutObjectBackToPool(GameObject obj)
    {
        var authoring = obj.GetComponent<EntityAuthoring>();
        //authoring.bodyRenderer.spriteRenderer.material.SetFloat("HideWall", 0);

        if (authoring != null)
        {
            authoring.bodyRenderer.Show();
            var children = authoring.partsHolder.transform.GetComponentsInChildren<EntityAuthoring>();
            foreach (var child in children)
            {
                PutObjectBackToPool(child.gameObject);
            }

        }
        obj.transform.rotation = Quaternion.Euler(0,0,0);
        obj.SetActive(false);
    }
    public static GameObject Take(string name)
    {
        if (i.poolsDict == null) i.RecreatePools();
        GameObject thing = i.poolsDict[name].Dequeue();
        thing.gameObject.SetActive(true);
        i.poolsDict[name].Enqueue(thing);
        return thing;
    }


}
