using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : BaseMethodsClass where T : BaseMethodsClass
{
    private static T _i;
    public static T i
    { 
        get 
        { 
            if(_i == null)
            {
                _i = FindObjectOfType<T>();
            }
            return _i;
        }

    }


}
