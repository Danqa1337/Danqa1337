using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeter : Singleton<DistanceMeter>
{
    [SerializeField] private float _scale;

    public static event Action<float> OnDistanceUpdated;

    private float _distance;

    public static float Distance { get => instance._distance; }

    private void Update()
    {
        _distance += Termometer.Temperature * Time.deltaTime * _scale;
        OnDistanceUpdated?.Invoke(_distance);
    }
}