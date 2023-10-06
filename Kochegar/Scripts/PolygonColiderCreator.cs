using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolygonColiderCreator : MonoBehaviour
{
    public static void SetColliderPoints(Mesh mesh, PolygonCollider2D polygonCollider)
    {
        // Get triangles and vertices from mesh
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        if (vertices.Length < 3)
        {
            throw new System.ArgumentOutOfRangeException("mesh has less than 3 verts!");
        }

        // Get just the outer edges from the mesh's triangles (ignore or remove any shared edges)
        Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int e = 0; e < 3; e++)
            {
                int vert1 = triangles[i + e];
                int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                string edgeKey = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
                if (edges.ContainsKey(edgeKey))
                {
                    edges.Remove(edgeKey);
                }
                else
                {
                    edges.Add(edgeKey, new KeyValuePair<int, int>(vert1, vert2));
                }
            }
        }

        // Create edge lookup (Key is first vertex, Value is second vertex, of each edge)
        Dictionary<int, int> lookup = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> edge in edges.Values)
        {
            if (lookup.ContainsKey(edge.Key) == false)
            {
                lookup.Add(edge.Key, edge.Value);
            }
        }

        // Create empty polygon collider
        polygonCollider.pathCount = 0;

        // Loop through edge vertices in order
        int startVert = lookup.Keys.ToList()[0];
        int nextVert = startVert;
        int highestVert = startVert;
        List<Vector2> colliderPath = new List<Vector2>();
        var tries = 0;
        while (tries < 200)
        {
            tries++;
            // Add vertex to collider path
            colliderPath.Add(vertices[nextVert]);

            // Get next vertex
            nextVert = lookup[nextVert];

            // Store highest vertex (to know what shape to move to next)
            if (nextVert > highestVert)
            {
                highestVert = nextVert;
            }

            // Shape complete
            if (nextVert == startVert)
            {
                // Add path to polygon collider

                var firstAngle = KatabasisUtillsClass.AngleBetween(colliderPath[1] - colliderPath[0], colliderPath[2] - colliderPath[0]);
                if (Mathf.Abs(firstAngle) > 0.1f && Mathf.Abs(firstAngle) < 179.9f)
                {
                    polygonCollider.pathCount++;
                    polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
                }

                colliderPath.Clear();

                // Go to next shape if one exists
                if (lookup.ContainsKey(highestVert + 1))
                {
                    // Set starting and next vertices
                    startVert = highestVert + 1;
                    nextVert = startVert;

                    // Continue to next loop
                    continue;
                }

                // No more verts
                break;
            }
        }
        if (tries == 200)
        {
            Debug.Log("Mesh calculation filed");
        }
    }
}