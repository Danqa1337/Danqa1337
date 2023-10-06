using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MachineShake : MonoBehaviour
{
    [Min(0.01f)]
    [SerializeField] private float _speed;

    [SerializeField] private float _magnitude = 1;
    [SerializeField] private Transform _machine;

    private void Update()
    {
        if (KatabasisUtillsClass.Chance(_speed * Time.deltaTime * Termometer.NormalizedTemperature))
        {
            if (DOTween.TweensByTarget(_machine) == null)
            {
                _machine.DOShakePosition(2, _magnitude, 1);
            }
        }
    }
}