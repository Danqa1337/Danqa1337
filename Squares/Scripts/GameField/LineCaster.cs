using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineCaster : MonoBehaviour
{
    [SerializeField] private Color _lineColor;
    [SerializeField] private float _maxLineLength;
    [SerializeField] private float _minLineWidth;
    [SerializeField] private float _maxLineWidth;
    [SerializeField] private int _maxIntersections;
    [SerializeField] private int _minIntersections;
    [SerializeField] private LayerMask _squareLayerMask;
    [SerializeField] private LayerMask _combinedMask;

    private Line _topLine;
    private Line _bottomLine;
    private Line _leftLine;
    private Line _rightLine;
    public float CurrentLineWidth => _minLineWidth;
    private Direction[] Directions => Utills.Directions;
    private Bounds _squareBounds => _square.ShapeCollider.bounds;
    private Vector2 _squarePosition => _square.transform.position;
    public LayerMask SquareLayerMask => _squareLayerMask;
    public LayerMask CombinedMask => _combinedMask;

    public float MaxLineLength => _maxLineLength;
    public float MinLineWidth => _minLineWidth;
    public float MaxLineWidth => _maxLineWidth;

    private Line[] _lines => new Line[] { _topLine, _bottomLine, _leftLine, _rightLine, };
    public IEnumerable<Line> Lines => _lines;

    private SquareOrthogonal _square;
    private GameField _gameField;

    private void OnEnable()
    {
        _gameField.OnHorizontalLinesUpdate += UpdateLinesHorizontal;
        _gameField.OnVerticalLinesUpdate += UpdateLinesVertical;
    }

    private void OnDisable()
    {
        _gameField.OnVerticalLinesUpdate -= UpdateLinesHorizontal;
        _gameField.OnVerticalLinesUpdate -= UpdateLinesVertical;
    }

    private void OnValidate()
    {
        SetUp();
        foreach (var item in Lines)
        {
            item.SetColor(_lineColor);
        }
    }

    private void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        _gameField = GetComponentInParent<GameField>();
        _square = GetComponentInParent<SquareOrthogonal>();

        var lines = GetComponentsInChildren<Line>();
        if (lines.Length != 4)
        {
            throw new System.Exception("number of lines must be 4");
        }

        _topLine = lines.FirstOrDefault(l => l.Direction == Direction.Up);
        if (_topLine == null) throw new System.Exception("top line is missing");

        _bottomLine = lines.FirstOrDefault(l => l.Direction == Direction.Down);
        if (_bottomLine == null) throw new System.Exception("bottom line is missing");

        _leftLine = lines.FirstOrDefault(l => l.Direction == Direction.Left);
        if (_leftLine == null) throw new System.Exception("left line is missing");

        _rightLine = lines.FirstOrDefault(l => l.Direction == Direction.Right);
        if (_rightLine == null) throw new System.Exception("right line is missing");

        foreach (var item in Lines)
        {
            item.SetUp();
        }
    }

    public void SetRandomIntersectsCount()
    {
        for (int i = 0; i < 4; i++)
        {
            var leftValue = GetRandomValue();
            var rightValue = GetRandomValue();

            _lines[i].SetIntersectLimits(new Vector2Int(leftValue, rightValue));
        }

        int GetRandomValue()
        {
            return KatabasisUtillsClass.Chance(40) ? _minIntersections : (int)Random.Range(_minIntersections, _maxIntersections + 1);
        }
    }

    public void UpdateLinesVertical()
    {
        _leftLine.UpdateGeometry();
        _rightLine.UpdateGeometry();
    }

    public void UpdateLinesHorizontal()
    {
        _topLine.UpdateGeometry();
        _bottomLine.UpdateGeometry();
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

    private void OnDrawGizmos()
    {
        if (_square != null)
        {
            for (int i = 0; i < 4; i++)
            {
                Gizmos.color = Color.blue;
                var direction = Directions[i];

                var extends = GetExtends(direction);
                var width = _maxLineWidth;
                var worldPosition = _squarePosition + Utills.Turn(direction, new Vector2(0, extends.y + width / 2));

                Gizmos.DrawSphere(worldPosition, 0.03f);
            }
        }
    }
}