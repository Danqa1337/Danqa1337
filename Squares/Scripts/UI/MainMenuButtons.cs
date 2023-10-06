using GooglePlayGames;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    private void Start()
    {
        UiManager.SwichToUi(UiManager.UIName.MainMenu);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneLoader.LoadScene(1);
    }

    public void ShowScore()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else
        {
            Debug.Log("Can't show score. You are not authed");
        }
    }
}