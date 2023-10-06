using EzySlice;
using UnityEngine;

public static class ChunkSlicer
{
    public static GameObject[] Slice(Chunk chunk, Vector3 position, Vector3 normal)
    {
        var crossectionMaterial = chunk.GetComponent<MeshRenderer>().material;

        var pices = chunk.gameObject.SliceInstantiate(position, normal, new TextureRegion(0, 0, 1, 1), crossectionMaterial, ChunkFactory.ChunkPrefab);
        if (pices != null && pices.Length > 1)
        {
            for (int i = 0; i < pices.Length; i++)
            {
                Physics2D.SyncTransforms();
                var piece = pices[i];
                piece.layer = chunk.gameObject.layer;
                piece.name = chunk.gameObject.name + " piece";
                piece.GetComponent<Chunk>().AdjustMesh();
            }
            MonoBehaviour.Destroy(chunk.gameObject);
        }
        return pices;
    }
}