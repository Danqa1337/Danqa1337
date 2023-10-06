using System;
using UnityEngine;

public class AdsTimer : MonoBehaviour
{
    public static event Action OnTimerZero;

    [SerializeField] private int _maxTimer;
    private const string _playerPrefsKey = "AdsTimer";

    private void OnEnable()
    {
        Player.OnPlayerDied += DecreaseTimer;
        //AdsInterstitial.OnAddShownAction += ResetTimer;
    }

    private void OnDisable()
    {
        Player.OnPlayerDied -= DecreaseTimer;
        //// AdsInterstitial.OnAddShownAction -= ResetTimer;
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(_playerPrefsKey))
        {
            ResetTimer();
        }
    }

    public static bool IsItTimetoShowAd()
    {
        if (PlayerPrefs.HasKey(_playerPrefsKey))
        {
            return PlayerPrefs.GetInt(_playerPrefsKey) == 0;
        }
        return false;
    }

    private void DecreaseTimer()
    {
        var currentTimer = Mathf.Max(0, PlayerPrefs.GetInt(_playerPrefsKey) - 1);

        if (currentTimer == 0)
        {
            Debug.Log("Ad time zero");
            OnTimerZero?.Invoke();
            ResetTimer();
        }
        else
        {
            PlayerPrefs.SetInt(_playerPrefsKey, currentTimer);
            PlayerPrefs.Save();
        }
    }

    private void ResetTimer()
    {
        PlayerPrefs.SetInt(_playerPrefsKey, _maxTimer);
        PlayerPrefs.Save();
    }
}