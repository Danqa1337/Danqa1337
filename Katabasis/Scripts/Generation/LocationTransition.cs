using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
[System.Serializable]
public class LocationTransition
{
    public int id;
    
    public int LocationFromID=-1;
    
    public int LocationToID=-1;
    public bool exposed;
    public bool toOpened;
    public bool fromOpened;
    public float2 positionFrom;
    public float2 positionTo;
    public LocationTransition( int locationFromId, int locationToId, bool _exposed= true, bool _toOpened = true, bool _fromOpened = true)
    {
        
        LocationFromID = locationFromId;
        LocationToID = locationToId;
        exposed = _exposed;
        toOpened = _toOpened;
        fromOpened = _fromOpened;
        
    }
}
