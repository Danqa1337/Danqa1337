using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landscape : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _sectionSize;
    [SerializeField] private GameObject[] _sections;

    private void Update()
    {
        foreach (var item in _sections)
        {
            item.transform.localPosition += Vector3.left * _speed * Termometer.NormalizedTemperature * Time.fixedDeltaTime;
            if (item.transform.localPosition.x <= -(_sectionSize * (_sections.Length / 2f)))
            {
                item.transform.localPosition = new Vector3(_sectionSize * (_sections.Length / 2f), 0, 0);
            }
        }
    }
}