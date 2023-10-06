using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SeamlessTilesetCreater : MonoBehaviour
{
    public Texture2D partsTexture;
    public Texture2D tilesetTexture;
    public string path;

    [ContextMenu("Create tileset")]

    public void CreateTileset()
    {
        List<Color[]> parts = new List<Color[]>();

        int[][] codes = new[]
        {
            new int[] { 4, 9, 14,  9, 14,  9, 14,  9, 14,  9, 14, 19, 20, 20,  4, 19},
            new int[] { 2, 8, 11,  6, 13,  8, 11,  6, 11,  6, 13, 18, 20, 20,  2, 18},
            new int[] { 3, 17, 4, 14, 12,  7, 14,  9, 14,  9, 12,  7, 14,  9, 12, 17},
            new int[] { 2, 18, 2,  0,  5,  8, 13,  0,  5,  0,  5,  0,  5,  0,  5, 18},
            new int[] { 3, 7, 12, 10, 15, 17,  3, 10, 15, 10, 15, 10, 15, 10, 15, 17},
            new int[] { 2, 8, 13,  8, 11, 16,  2,  8,  6, 11, 13,  0,  5,  0,  5, 18},
            new int[] { 3, 17, 3,  7, 14,  9, 12,  7, 14, 19,  3, 10, 15, 10, 15, 17},
            new int[] { 2, 18, 2,  0,  5,  8, 13,  0,  5, 18,  2,  0,  5,  0,  5, 18},
            new int[] { 3, 17, 3, 10, 15, 17,  3, 10, 15,  7, 12, 10, 15, 10, 15, 17},
            new int[] { 2, 18, 2,  0,  5, 18,  1, 11, 13,  0,  5,  0,  5,  8, 13, 18},
            new int[] { 3, 7, 12, 10, 15,  7,  9, 14, 12, 10, 15, 10, 15,  7, 12, 17},
            new int[] { 1, 6, 13,  0,  5,  0,  5,  0,  5,  8, 13,  8, 13,  8, 13, 18},
            new int[] {20,20,  3, 10, 15, 10, 15, 10, 15,  7, 12,  7, 12, 17,  3, 17},
            new int[] {20,20,  2,  0,  5,  0,  5,  0,  5,  8, 13,  8, 11, 16,  1, 16},
            new int[] { 4, 9, 12, 10, 15, 10, 15, 10, 15,  7, 12,  7, 14, 19,  4, 19},
            new int[] { 1, 6, 11,  6, 11,  6, 11,  6, 11,  6, 11,  6, 11, 16,  1, 16},
        };

        codes = codes.Reverse().ToArray();
        for (int x = 0; x < 80; x += 16)
        {
            for (int y = 0; y < 80; y += 16)
            {
                parts.Add(partsTexture.GetPixels(x,y,16,16));
            }
        }

        

        for (int x = 0; x < 32*8; x+= 16)
        {
            for (int y = 0; y < 32*8; y+= 16)
            {
                tilesetTexture.SetPixels(x,y,16,16, parts[codes[(int)(y/16f)][(int)(x/16f)]]);
            }
        }
        tilesetTexture.Apply();
        
       

        byte[] bytes = tilesetTexture.EncodeToPNG();
        File.Delete(path);
        File.WriteAllBytes(path, bytes);
    }
}
