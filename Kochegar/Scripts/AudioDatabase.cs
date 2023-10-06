using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum SoundName
{
    Null,
    Piston,
    MainTheme,
    Ambient,
    Tick1,
    Tick2,
    Bup,
    Steam,
    Crack,
}

[CreateAssetMenu(fileName = "New Audio Database", menuName = "Audio Database")]
public class AudioDatabase : SingletonScriptableObject<AudioDatabase>
{
    [SerializeField] private EventReference _eventNotFound;
    [SerializeField] private EventReference _piston;
    [SerializeField] private EventReference _mainTheme;
    [SerializeField] private EventReference _ambient;
    [SerializeField] private EventReference _tick1;
    [SerializeField] private EventReference _tick2;
    [SerializeField] private EventReference _bup;
    [SerializeField] private EventReference _steam;
    [SerializeField] private EventReference _crack;

    private Dictionary<SoundName, EventReference> eventsByName;

    public static void StartUp()
    {
        instance.eventsByName = new Dictionary<SoundName, EventReference>();
        var eventRefsFI = instance.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var soundNameValues = Enum.GetValues(typeof(SoundName));

        instance.eventsByName.Add(SoundName.Null, instance._eventNotFound);
        foreach (var item in soundNameValues)
        {
            bool found = false;
            foreach (var reference in eventRefsFI)
            {
                if (reference.Name.ToLower() == "_" + item.ToString().ToLower())
                {
                    found = true;
                    instance.eventsByName.Add((SoundName)item, (EventReference)reference.GetValue(instance));
                    break;
                }
            }
            if (!found)
            {
                UnityEngine.Debug.Log("Audio event not found for " + item.ToString());
            }
        }
    }

    public static EventReference GetAudioEvent(SoundName soundName)
    {
        if (instance.eventsByName.ContainsKey(soundName))
        {
            return instance.eventsByName[soundName];
        }
        else
        {
            UnityEngine.Debug.Log("AudioEvent for " + soundName + " not found");
            return instance._eventNotFound;
        }
    }
}