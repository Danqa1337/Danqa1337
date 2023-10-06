using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingletonScriptableObject<T> : ScriptableObject where T :ScriptableObject
{

    private static T _i;



    public static T i
    {
        
        get
        {
            
            if(_i == null )
            {
      
                _i = Resources.FindObjectsOfTypeAll<T>()[0];

            }
            return _i;
        }
    }

}
