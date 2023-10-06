using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cog : MonoBehaviour
{
    [SerializeField] private float _revolutionsPerMinute = 1;
    [SerializeField] private bool _startActive;
    [SerializeField] private bool _dependOnTemperture = true;
    private bool _isActive;

    private void Start()
    {
        if (_startActive)
        {
            Run();
        }
    }

    private void Update()
    {
        if (_isActive)
        {
            var tempMult = _dependOnTemperture ? Termometer.NormalizedTemperature : 1;
            transform.Rotate(0, 0, 6f * _revolutionsPerMinute * tempMult * Time.deltaTime);
        }
    }

    public void SetSpeed(float speed)
    {
        _revolutionsPerMinute = speed;
    }

    public void Run()
    {
        _isActive = true;
    }

    public void Stop()
    {
        _isActive = false;
    }
}