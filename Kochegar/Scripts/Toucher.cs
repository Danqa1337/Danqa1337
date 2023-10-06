using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Toucher : MonoBehaviour
{
    [SerializeField] private bool _debug;
    [SerializeField] private float _radius = 0.5f;

    public void Touch()
    {
        var overlapColliders = Physics2D.OverlapCircleAll(transform.position, _radius);

        if (overlapColliders.Length > 0)
        {
            foreach (var overlapCollider in overlapColliders)
            {
                var touchable = overlapCollider.GetComponent<ITouchable>();
                if (touchable != null)
                {
                    touchable.Touch();
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}