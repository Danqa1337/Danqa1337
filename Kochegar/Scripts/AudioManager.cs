using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public enum BusName
    {
        SFX,
        Music,
    }

    [SerializeField] private bool _debug;
    private Bus _ambientBus;
    private Bus _musicBus;

    public void Awake()
    {
        AudioDatabase.StartUp();
        RuntimeManager.CoreSystem.mixerSuspend();
        RuntimeManager.CoreSystem.mixerResume();
    }

    private void Start()
    {
        StartCoroutine(Wait());

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(5);

            if (!RuntimeManager.IsInitialized) throw new System.Exception("not initialized");
            if (!RuntimeManager.HaveAllBanksLoaded) throw new System.Exception("not loaded");
        }
    }

    public static void PlayEvent(EventReference eventReference)
    {
        RuntimeManager.PlayOneShot(eventReference);
    }

    public static void StopAllImmediate()
    {
        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public static void StopAllFadeOut()
    {
        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public static void PlayEvent(SoundName soundName)
    {
        RuntimeManager.PlayOneShot(AudioDatabase.GetAudioEvent(soundName));
        if (instance._debug) UnityEngine.Debug.Log("Playing " + soundName);
    }

    public static void ChangeBusVolume(BusName busName, float value)
    {
        switch (busName)
        {
            case BusName.SFX:
                instance._ambientBus.setVolume(value);
                break;

            case BusName.Music:
                instance._musicBus.setVolume(value);
                break;

            default:
                break;
        }
    }
}