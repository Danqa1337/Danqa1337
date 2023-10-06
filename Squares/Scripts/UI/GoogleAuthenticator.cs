using UnityEngine;
using GooglePlayGames;
using System;
using GooglePlayGames.BasicApi;

public class GoogleAuthenticator : Singleton<GoogleAuthenticator>
{
    public static bool Authenticated { get => instance._authenticated; }
    private bool _authenticated;

    public static event Action OnAuthenticationStarted;

    public static event Action<bool> OnAuthenticationComplete;

    private void Start()
    {
        PlayGamesPlatform.Activate();

        if (!PlayGamesPlatform.Instance.IsAuthenticated())
        {
            OnAuthenticationStarted?.Invoke();
            PlayGamesPlatform.Instance.Authenticate((result) =>
            {
                bool success = false;
                switch (result)
                {
                    case GooglePlayGames.BasicApi.SignInStatus.Success:
                        Debug.Log("authenticate success");
                        success = true;
                        break;

                    case GooglePlayGames.BasicApi.SignInStatus.InternalError:
                        Debug.Log("authenticate error");
                        success = false;
                        break;

                    case GooglePlayGames.BasicApi.SignInStatus.Canceled:
                        Debug.Log("authenticate canceled");
                        success = false;
                        break;
                }

                _authenticated = success;
                OnAuthenticationComplete?.Invoke(success);
            });
        }
    }
}