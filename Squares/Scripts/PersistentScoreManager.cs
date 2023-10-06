using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using GooglePlayGames.BasicApi;

public class PersistentScoreManager : MonoBehaviour
{
    public static void GetRecord(Action<int> onRecord)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            onRecord?.Invoke(0);
        }
        else
        {
            onRecord?.Invoke(int.MaxValue);
        }
    }

    public static void WriteScore(int score)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_leaderboard, (result) =>
            {
                Debug.Log("Record set " + result);
            });
        }
    }
}