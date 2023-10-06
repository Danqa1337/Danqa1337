using UnityEngine;

public class CircleImpactor : MonoBehaviour
{
    [SerializeField] private float _sliceNormalDeviation = 0.1f;
    [SerializeField] private bool _debug;
    [SerializeField] private float _radius = 0.5f;

    public void Impact()
    {
        var overlapColliders = Physics2D.OverlapCircleAll(transform.position, _radius);

        if (overlapColliders.Length > 0)
        {
            foreach (var overlapCollider in overlapColliders)
            {
                var sliceable = overlapCollider.GetComponent<ISliceble>();
                if (sliceable != null)
                {
                    var sliceNormal = UnityEngine.Random.insideUnitCircle;
                    var pices1 = sliceable.Slice(sliceable.transform.GetComponent<Renderer>().bounds.center, sliceNormal.ToVector3());
                    if (pices1 != null)
                    {
                        foreach (var piece1 in pices1)
                        {
                            var pieces2 = piece1.GetComponent<ISliceble>().Slice(piece1.transform.GetComponent<Renderer>().bounds.center, Random.insideUnitCircle);
                        }
                    }
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