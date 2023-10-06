using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStarter : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        AudioDatabase.StartUp();
    }
}