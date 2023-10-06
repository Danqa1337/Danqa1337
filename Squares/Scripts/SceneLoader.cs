using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static event Action OnLoadingStarted;

    public static async void LoadScene(int index)
    {
        OnLoadingStarted?.Invoke();
        await System.Threading.Tasks.Task.Delay(100);
        SceneManager.LoadSceneAsync(index);
    }
}