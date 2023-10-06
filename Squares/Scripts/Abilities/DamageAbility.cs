using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAbility : MonoBehaviour, IAbilityOnTarget
{
    [SerializeField] private int _damage;
    public void Apply(GameObject target)
    {
        if (target.GetComponent<Player>() != null)
        {
            Player.ApplyDamage(_damage);
        }
    }
}
