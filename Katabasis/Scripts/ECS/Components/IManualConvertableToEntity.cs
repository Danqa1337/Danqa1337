using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public interface IManualConvertableToEntity
{
    abstract void Convert(Entity entity, GameObject gameObject);
}
