using GooglePlayGames;
using System;
using UnityEngine;

public class RuntimeScoreCounter : Singleton<RuntimeScoreCounter>
{
    private int _currentScore;
    public static int CurrentScore => instance._currentScore;

    public static event Action<int, bool> OnScoreWriten;

    public static event Action<int> OnScoreChanged;

    private void OnEnable()
    {
        Player.OnPlayerDied += WrightScoreToTable;
    }

    private void OnDisable()
    {
        Player.OnPlayerDied -= WrightScoreToTable;
    }

    public static void IncreaseScore()
    {
        instance._currentScore++;
        OnScoreChanged?.Invoke(instance._currentScore);
    }

    private void WrightScoreToTable()
    {
        PersistentScoreManager.GetRecord((result) =>
        {
            var isARecord = result == 0 || CurrentScore > result;
            OnScoreWriten?.Invoke(CurrentScore, isARecord);
            if (isARecord)
            {
                PersistentScoreManager.WriteScore(CurrentScore);
            }
        });
    }
}