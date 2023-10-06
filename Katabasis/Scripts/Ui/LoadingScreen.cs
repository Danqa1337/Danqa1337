using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : Singleton<LoadingScreen>
{
    //public Canvas canvas;
    public Camera cam;
    public Bar progressBar;

    private void Start()
    {
        Hide();
    }

    public async Task Show()
    {   
        cam.enabled = true;
       // canvas.enabled = true;
        Camera.main.Render();
        await Task.Delay(1);
    }
    public async void StartLoading(int sceneNum)
    {
        var operation = SceneManager.LoadSceneAsync(sceneNum);
        Show();
        while (!operation.isDone)
        {
            progressBar.SetNewValue(operation.progress, 1);
            await Task.Delay(10);
        }
        
    }
    
    public void EndLoading()
    {
       Hide();
    }
    public void Hide()
    {
        cam.enabled = false;
       // canvas.enabled = false;

    }

}
