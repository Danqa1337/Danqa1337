using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SticksManager : MonoBehaviour
{
    [SerializeField] private Stick[] _sticks;

    private void OnEnable()
    {
        TouchController.OnTap += OnTap;
    }

    private void OnDisable()
    {
        TouchController.OnTap -= OnTap;
    }

    private void OnTap(Vector2 touchPosition)
    {
        var readySticks = _sticks.Where(s => s.Ready).ToArray();
        if (readySticks.Length > 0)
        {
            var stick = readySticks.RandomItem();
            stick.Strike(touchPosition);
        }
    }
}