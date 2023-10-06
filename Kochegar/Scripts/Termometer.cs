using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Termometer : Singleton<Termometer>
{
    [SerializeField] private float _coolingPerTick;
    [SerializeField] private float _maxTemp = 10000;
    public static float Temperature => instance._temperature;
    public static float MaxTemperature => instance._maxTemp;

    public static float NormalizedTemperature => Temperature / MaxTemperature;
    private float _temperature;

    private void OnEnable()
    {
        Burner.OnChunkBurned += IncreaseTemperature;
    }

    private void IncreaseTemperature(float t)
    {
        _temperature = Mathf.Min(_temperature + t, _maxTemp);
    }

    private void Update()
    {
        _temperature = Mathf.Max(0, _temperature - _coolingPerTick);
    }
}