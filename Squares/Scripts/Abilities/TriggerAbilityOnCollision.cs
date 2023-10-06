using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerAbilityOnCollision : MonoBehaviour
{
    [SerializeField] private float _minTriggerStayTime;
    private bool _isTriggetStillActive;
    [SerializeField] private UnityEvent<GameObject> OnCollisonEvent;
    private void OnCollisionEnter2D(Collision2D collision)
    {
       OnCollisonEvent.Invoke(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isTriggetStillActive = true;
        StartCoroutine(WaitAndInvokeCollisionEvent(collision));
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _isTriggetStillActive = false;
    }
    IEnumerator WaitAndInvokeCollisionEvent(Collider2D collision)
    {
        yield return new WaitForSeconds(_minTriggerStayTime);
        if (_isTriggetStillActive)
        {
            OnCollisonEvent.Invoke(collision.gameObject);
        }
    }
}
