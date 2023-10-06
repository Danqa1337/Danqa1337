using System.Linq;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private Direction _direction;
    private SpriteFlicker _flicker;
    [SerializeField] private int _rightIntersectLimit;
    [SerializeField] private int _leftIntersectLimit;
    private SquareOrthogonal _square;
    private EdgeCollider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private LineCaster _lineCaster;
    public Direction Direction => _direction;
    private Bounds _squareBounds => _square.ShapeCollider.bounds;

    private void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        _flicker = GetComponent<SpriteFlicker>();
        _square = GetComponentInParent<SquareOrthogonal>();
        _lineCaster = GetComponentInParent<LineCaster>();
        _collider = GetComponent<EdgeCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }

    public void SetIntersectLimits(Vector2Int limits)
    {
        _rightIntersectLimit = limits.y;
        _leftIntersectLimit = limits.x;
    }

    public void UpdateGeometry()
    {
        var extends = GetExtends(_direction);
        var width = _lineCaster.MaxLineWidth;
        var minRaycastDistance = extends.x + width;
        var raycastPoistion = _square.transform.position.ToVector2() + Turn(_direction, new Vector2(0, extends.y + width / 2));
        var leftHandVector = Turn(_direction, Vector2.left);
        var rightHandVector = Turn(_direction, Vector2.right);
        var leftHandHits = Physics2D.RaycastAll(raycastPoistion, leftHandVector, _lineCaster.MaxLineLength, _lineCaster.CombinedMask);
        var rightHandHits = Physics2D.RaycastAll(raycastPoistion, rightHandVector, _lineCaster.MaxLineLength, _lineCaster.CombinedMask);
        var leftDst = GetDistance(leftHandHits, _leftIntersectLimit);
        var rightDst = GetDistance(rightHandHits, _rightIntersectLimit);
        var localPosition = Turn(_direction, new Vector2((rightDst - leftDst) / 2, extends.y));

        var size = new Vector2(leftDst + rightDst, width);

        var colliderPoint1 = new Vector2(-size.x / 2 + width, width * 0.5f);
        var colliderPoint2 = new Vector2(size.x / 2 - width, width * 0.5f);

        transform.localPosition = localPosition;
        _spriteRenderer.size = size;
        _collider.points = new Vector2[] { colliderPoint2, colliderPoint1 };

        Physics2D.SyncTransforms();

        float GetDistance(RaycastHit2D[] raycastHit2Ds, int maxIntersects)
        {
            if (maxIntersects == -1) return minRaycastDistance;
            var hits = raycastHit2Ds.Where(hit => hit.distance > minRaycastDistance && hit.collider.gameObject.transform.parent != gameObject.transform.parent).ToList();

            for (int i = 0; i < hits.Count; i++)
            {
                if (i == maxIntersects || _lineCaster.SquareLayerMask.Contains(hits[i].collider.gameObject.layer))
                {
                    return hits[i].distance;
                }
            }
            return minRaycastDistance;
        }
    }

    private Vector2 GetExtends(Direction side)
    {
        switch (side)
        {
            case Direction.Up:
                return new Vector2(_squareBounds.extents.x, _squareBounds.extents.y);

            case Direction.Down:
                return new Vector2(_squareBounds.extents.x, _squareBounds.extents.y);

            case Direction.Left:
                return new Vector2(_squareBounds.extents.y, _squareBounds.extents.x);

            case Direction.Right:
                return new Vector2(_squareBounds.extents.y, _squareBounds.extents.x);

            default:
                break;
        }
        throw new System.ArgumentOutOfRangeException();
    }

    private Vector2 Turn(Direction side, Vector2 vector)
    {
        return Utills.Turn(side, vector);
    }
}