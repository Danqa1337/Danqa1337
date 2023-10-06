using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Describer : BaseMethodsClass
{
    protected float RoundFloat(float f)
    {
        return Mathf.Round(f *10) *0.1f;
    }
}
