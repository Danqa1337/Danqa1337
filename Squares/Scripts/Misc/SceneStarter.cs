using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStarter : Singleton<SceneStarter>
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            UiManager.SwichToUi(UiManager.UIName.MainMenu);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            UiManager.SwichToUi(UiManager.UIName.Hud);
            if (AdsTimer.IsItTimetoShowAd())
            {
            }
        }
    }
}