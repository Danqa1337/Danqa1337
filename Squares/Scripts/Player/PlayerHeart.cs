using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeart : MonoBehaviour
{
    [SerializeField] private Collider2D _rightCollider2D;
    [SerializeField] private Collider2D _leftCollider;
    [SerializeField] private Collider2D _topCollider;
    [SerializeField] private Collider2D _bottomCollider;

    private void Update()
    {
        if(_rightCollider2D.IsTouchingLayers() && _leftCollider.IsTouchingLayers())
        {
            Player.ApplyDamage(100);
        }
        if(_topCollider.IsTouchingLayers() && _bottomCollider.IsTouchingLayers())
        {
            Player.ApplyDamage(100);
        }
    }


}
