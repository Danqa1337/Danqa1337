using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSpeedController : Singleton<SquareSpeedController>
{
    private int _startTimeSeconds;
    [SerializeField] private float _minSpeedMult = 1;
    [SerializeField] private float _maxSpeedMult = 2;
    [SerializeField] private float _maxSpeedTimeSeconds = 100;

    public static float SpeedMultipler
    {
        get
        {
            var timeSinceStart = DateTime.Now.Second - instance._startTimeSeconds;
            var multipler = Mathf.Lerp(instance._minSpeedMult, instance._maxSpeedMult, (float)timeSinceStart / instance._maxSpeedTimeSeconds);
            return multipler;
        }
    }
    private void Start()
    {
        _startTimeSeconds = DateTime.Now.Second;
    }
    private void Update()
    {
    }
}
