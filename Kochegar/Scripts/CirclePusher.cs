using UnityEngine;

public class CirclePusher : MonoBehaviour
{
    [SerializeField] private float _impactForce = 200;
    [SerializeField] private float _radius = 0.5f;

    public void Push()
    {
        var overlapColliders = Physics2D.OverlapCircleAll(transform.position, _radius);

        if (overlapColliders.Length > 0)
        {
            foreach (var overlapCollider in overlapColliders)
            {
                var rb = overlapCollider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce((overlapCollider.transform.position - transform.position).normalized * _impactForce);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}