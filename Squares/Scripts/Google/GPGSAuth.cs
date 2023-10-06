using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;

public class GPGSAuth : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate((result) =>
        {
            Debug.Log(result);
            if (result == GooglePlayGames.BasicApi.SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI();
            }
        });
    }
}