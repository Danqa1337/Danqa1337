using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TermometerView : MonoBehaviour
{
    [SerializeField] private float _maxArrowAngle;
    [SerializeField] private Transform _arrow;
    [SerializeField] private Label _label;
    private Quaternion _minRotation => Quaternion.Euler(new Vector3(0, 0, -_maxArrowAngle));
    private Quaternion _maxRotation => Quaternion.Euler(new Vector3(0, 0, _maxArrowAngle));

    private void Start()
    {
        _arrow.localRotation = _minRotation;
    }

    private void Update()
    {
        _label.SetValue(Termometer.Temperature);
        _arrow.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-_maxArrowAngle, _maxArrowAngle, Termometer.Temperature / Termometer.MaxTemperature));
    }
}