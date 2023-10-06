using System;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class DeathScreen : Singleton<DeathScreen>
{
    [SerializeField] private Image _recordIcon;
    [SerializeField] private Label _recordLabel;

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SceneLoader.LoadScene(0);
    }

    private void OnEnable()
    {
        Player.OnPlayerDied += Show;
        RuntimeScoreCounter.OnScoreWriten += WriteDeathMessage;
    }

    private void OnDisable()
    {
        Player.OnPlayerDied -= Show;
        RuntimeScoreCounter.OnScoreWriten -= WriteDeathMessage;
    }

    private void Show()
    {
        UiManager.SwichToUi(UiManager.UIName.Death);
    }

    public void WriteDeathMessage(int score, bool isARecord)
    {
        _recordLabel.SetValue(score);
        _recordIcon.enabled = isARecord;
    }
}