using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkFactory : Singleton<ChunkFactory>
{
    [SerializeField] private GameObject[] _chunkPrefabs;
    public static GameObject ChunkPrefab => instance._chunkPrefabs[0];

    public static Chunk CreateChunk(int sidesNum, float deviation, float size)
    {
        var newChunk = Instantiate(instance._chunkPrefabs.RandomItem(), new Vector3(-1000, -1000), Quaternion.identity).GetComponent<Chunk>();
        newChunk.GetComponent<MeshFilter>().sharedMesh = MeshGenerator.GenerateMesh(sidesNum, deviation, size);
        newChunk.AdjustMesh();
        return newChunk;
    }
}