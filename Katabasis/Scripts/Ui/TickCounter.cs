using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TickCounter : Singleton<TickCounter>
{
    [SerializeField] private Counter counter;

    public void SetValue(int value)
    {
        counter.SetValue(value, "t");
    }
}
