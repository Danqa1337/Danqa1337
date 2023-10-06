using Unity.Mathematics;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private int _vertsNumToDraw;
    [SerializeField] private float _randomDeviation;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            GetComponent<MeshFilter>().mesh = GenerateMesh(_vertsNumToDraw, _randomDeviation, 1);
        }
    }
    public static Mesh GenerateMesh(int sidesNum, float deviation, float size)
    {
        var mesh = new Mesh();

        mesh.vertices = GetVertices(sidesNum, deviation, size);
        mesh.triangles = GetTriangles(sidesNum);
        mesh.normals = GetNormals(sidesNum);
        mesh.uv = GetUVs(mesh.vertices);
        mesh.MarkModified();
        return mesh;
    }
    private static Vector3[] GetVertices(int sidesNum, float deviation, float size)
    {
        var angle = 2f * math.PI / sidesNum;
        var vertices = new Vector3[sidesNum + 1];

        
        for (int i = 1; i < sidesNum + 1; i++)
        {
            vertices[i].x = math.cos(angle * (i - 1) + UnityEngine.Random.Range(-deviation, deviation)) * size;
            vertices[i].y = math.sin(angle * (i - 1) + UnityEngine.Random.Range(-deviation, deviation)) * size;
        }
        
        return vertices;
    }
    private static int[] GetTriangles(int sidesNum)
    {
        var tris = new int[sidesNum * 3];
        for (int i = 0; i < sidesNum; i++)
        {
            tris[i * 3] = i + 1;
            tris[i * 3 + 1] = i != sidesNum -1 ? i + 2 : 1;
            tris[i * 3 + 2] = 0;
        }
        return tris;
    }
    private static Vector3[] GetNormals(int sidesNum)
    {
        var normals = new Vector3[sidesNum + 1];
        for (int i = 0; i < sidesNum + 1; i++)
        {
            normals[i] = Vector3.forward;
        }
        return normals;
    }

    private static Vector2[] GetUVs(Vector3[] vertices)
    {
        var UVs = new Vector2[vertices.Length];
        for (int i = 0; i < UVs.Length; i++)
        {
            UVs[i] = vertices[i];
        }
        return UVs;
    }

    private void OnDrawGizmos()
    {
        //GetComponent<MeshFilter>().mesh = GenerateMesh(_vertsNumToDraw, _randomDeviation);
        //var vertices = GetVertices(_vertsNumToDraw);
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    Gizmos.color = Color.Lerp(Color.green, Color.red, (float)i / (float)_vertsNumToDraw);
        //    Gizmos.DrawSphere(vertices[i], 0.1f);
        //}
    }
}
