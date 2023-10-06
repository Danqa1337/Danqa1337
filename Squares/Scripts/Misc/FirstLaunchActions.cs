using System;
using UnityEngine;

public class FirstLaunchActions : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetString("LastLaunchTime", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    [ContextMenu("resetPreffs")]
    public void ResetLastLaunchTime()
    {
        PlayerPrefs.DeleteKey("LastLaunchTime");
        PlayerPrefs.Save();
    }

    private bool IsItFirstLaunch()
    {
        return !PlayerPrefs.HasKey("LastLaunchTime");
    }
}