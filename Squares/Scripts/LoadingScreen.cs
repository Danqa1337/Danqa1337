using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    private Canvas _canvas;

    private void OnEnable()
    {
        SceneLoader.OnLoadingStarted += Show;
        GoogleAuthenticator.OnAuthenticationStarted += Show;
        GoogleAuthenticator.OnAuthenticationComplete += OnAuthComplete;
    }

    private void OnDisable()
    {
        SceneLoader.OnLoadingStarted -= Show;
        GoogleAuthenticator.OnAuthenticationStarted -= Show;
        GoogleAuthenticator.OnAuthenticationComplete -= OnAuthComplete;
    }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        Hide();
    }

    public void Show()
    {
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.enabled = true;
    }

    public void OnAuthComplete(bool result)
    {
        Hide();
    }

    public void Hide()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.enabled = false;
    }
}