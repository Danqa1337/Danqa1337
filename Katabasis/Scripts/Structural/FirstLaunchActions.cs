using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FirstLaunchActions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!Directory.Exists(SaveLoader.savesFolderPath))
        {
            Directory.CreateDirectory(SaveLoader.savesFolderPath);
        }
    }


}
