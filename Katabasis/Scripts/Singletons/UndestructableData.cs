using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UndestructableData : MonoBehaviour
{
    public Biome presetToLoad;

    private void Start()
    {

        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);

    }

}
