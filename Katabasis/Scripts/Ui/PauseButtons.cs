using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour
{
    public void Options()
    {

    }

    public void SaveAndQuit()
    {
        SaveLoader.Save(DungeonStructure.CurrentLocation);
        Engine.i.Dispose();
        SceneManager.LoadSceneAsync(0);
    }

    public void GiveUp()
    {
        SaveLoader.DeleteSaves();
        Engine.i.Dispose();
        SceneManager.LoadSceneAsync(0);
    }
}
