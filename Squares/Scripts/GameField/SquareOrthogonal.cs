using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class SquareOrthogonal : Square
{
    [HideInInspector] public int lineLimitTop = int.MaxValue;
    [HideInInspector] public int lineLimitBottom = int.MaxValue;
    [HideInInspector] public int lineLimitRight = int.MaxValue;
    [HideInInspector] public int lineLimitLeft = int.MaxValue;

    [Header("Shape")]
    [SerializeField] private bool _transformationEnabled;

    [SerializeField] private float _damageColliderOffset;
    [SerializeField] private float _interactionColliderOffset;
    [SerializeField] private float _minSize;
    [SerializeField] private float _maxSize;

    [Min(0.001f)]
    [SerializeField] private float _transformationTime;

    [SerializeField] private LayerMask _groundLayerMask;

    [Min(1)]
    [SerializeField] private int _detectorRaysPerUnit = 1;

    [SerializeField] private float _detectionRayCastDST = 0.2f;

    [Header("Movement")]
    [SerializeField] private bool _movementEnabled;

    [SerializeField] private float _maxMovementCooldown;
    [SerializeField] private float _maxTransformationCooldown;
    [SerializeField] private float _maxMoveSpeed;
    [SerializeField] private float _minMoveSpeed;
    [SerializeField] private AxisRestriction _movementAxisRestriction;

    [Header("Colors")]
    [SerializeField] private bool _changingColorsEnabled;

    [SerializeField] private Color _yellowColor;
    [SerializeField] private Color _redColor;
    [SerializeField] private Color _blueColor;
    [SerializeField] private Color _blackColor;
    [SerializeField] private SquareColor _currentSquareColor;
    [SerializeField] private BoxCollider2D _areaCollider;

    private SpriteRenderer _spriteRenderer;
    private LineCaster _lineCaster;
    private TrailRenderer _trail;
    private GameField _gameField;

    private Direction _currentMovementDirection;

    private float _currentDirectionChangeCooldown;
    private float _currentTransformationCooldown;
    private float _currentMoveSpeed;

    private bool _hasSurfaceUp;
    private bool _hasSurfaceDown;
    private bool _hasSurfaceLeft;
    private bool _hasSurfaceRight;
    private bool _isTransforming;

    public UnityEvent<Direction> SideInteractionEvent;

    public enum SquareColor
    {
        Red,
        Yellow,
        Blue,
        Black,
    }

    public bool IsTransforming => _isTransforming;
    public SquareColor CurrentSquareColor => _currentSquareColor;
    public LineCaster LineCaster => _lineCaster;
    public bool CanChangeColors => _changingColorsEnabled;
    private bool _anySurface => _hasSurfaceUp || _hasSurfaceDown || _hasSurfaceLeft || _hasSurfaceRight;

    protected virtual void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        _lineCaster = GetComponentInChildren<LineCaster>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trail = GetComponentInChildren<TrailRenderer>();
        _gameField = GetComponentInParent<GameField>();
    }

    private void OnEnable()
    {
        if (_gameField != null)
        {
            _gameField.OnMovementUpdate += UpdateMovement;
            _gameField.Subscribe(this);
        }
        else
        {
            throw new System.Exception("Square needs a gamefield");
        }
    }
    

    private void Start()
    {
        AdjustColliders();
        SetColor(GetRandomSquareColor());
        AdjustTrailPosition();
        AdjustSpeed();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _lineCaster.StopAllCoroutines();
        if (_gameField != null)
        {
            _gameField.OnMovementUpdate -= UpdateMovement;
            _gameField.UnSubscribe(this);
        }
        else
        {
            throw new System.Exception("Square needs a gamefield");
        }
    }

    public void AdjustAllSizes()
    {
        if (!Application.isPlaying)
        {
            SetUp();
            LineCaster.SetUp();
            AdjustColliders();
            foreach (var item in GetComponentsInChildren<Line>())
            {
                item.SetUp();
            }
            LineCaster.UpdateLinesHorizontal();
            LineCaster.UpdateLinesVertical();
            var canvas = GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                canvas.GetComponent<RectTransform>().sizeDelta = _spriteRenderer.size;
            }
        }
    }

    private void AdjustSpeed()
    {
        var newSurface = _spriteRenderer.size.x * _spriteRenderer.size.y;
        var maxSurface = _maxSize * _maxSize;
        var minSurface = _minSize * _minSize;

        var surfaceCof = Mathf.InverseLerp(minSurface, maxSurface, newSurface);
        _currentMoveSpeed = Mathf.Lerp(_minMoveSpeed, _maxMoveSpeed, 1 - surfaceCof);
    }

    public void RegisterSideInteraction(Direction direction)
    {
        SideInteractionEvent.Invoke(direction);
    }

    public void UpdateMovement()
    {
        UpdateCooldowns();
        CheckSurfaces();

        if (_currentTransformationCooldown == 0 && _gameField.IsTransformationAlowed())
        {
            DoTransformation();
        }
        else
        {
            if (!_isTransforming)
            {
                if (_currentDirectionChangeCooldown == 0)
                {
                    SetRandomDirection();
                }

                if (CanMoveInDirection(_currentMovementDirection))
                {
                    MovePosition();
                }
                else
                {
                    SetRandomDirection();
                }
            }
        }
        Physics2D.SyncTransforms();
    }

    private bool CanMoveInDirection(Direction direction)
    {
        if (direction != Direction.Null && !HasSurface(direction))
        {
            var hasSurfaceToTheLeft = HasSurface(Utills.Turn(direction, Direction.Left));
            var hasSurfaceToTheRight = HasSurface(Utills.Turn(direction, Direction.Right));

            return !hasSurfaceToTheLeft && !hasSurfaceToTheRight;
        }
        return false;
    }

    private void UpdateCooldowns()
    {
        _currentDirectionChangeCooldown = math.max(0, _currentDirectionChangeCooldown - Time.fixedDeltaTime);
        _currentTransformationCooldown = math.max(0, _currentTransformationCooldown - Time.fixedDeltaTime);
    }

    public void AdjustColliders()
    {
        var size = _spriteRenderer.size;

        ShapeCollider.size = size;
    }

    private void CheckSurfaces(float lengthMultipler = 1)
    {
        _hasSurfaceUp = CheckSurface(Direction.Up, lengthMultipler);
        _hasSurfaceDown = CheckSurface(Direction.Down, lengthMultipler);
        _hasSurfaceLeft = CheckSurface(Direction.Left, lengthMultipler);
        _hasSurfaceRight = CheckSurface(Direction.Right, lengthMultipler);
    }

    private void MovePosition()
    {
        if (!_movementEnabled) return;
        transform.position += (Utills.DirectionToVector(_currentMovementDirection) * Time.fixedDeltaTime * _currentMoveSpeed * SquareSpeedController.SpeedMultipler).ToVector3();
        Physics2D.SyncTransforms();
    }

    private void DoTransformation()
    {
        _currentTransformationCooldown = _maxTransformationCooldown * UnityEngine.Random.Range(0.8f, 1.2f);
        if (_transformationEnabled && !_isTransforming)
        {
            StartCoroutine(ChangeShape());
        }

        IEnumerator ChangeShape()
        {
            _isTransforming = true;
            var targetSize = new Vector2(UnityEngine.Random.Range(_minSize, _maxSize), UnityEngine.Random.Range(_minSize, _maxSize));
            var oldSize = _spriteRenderer.size;
            var oldColor = _spriteRenderer.color;
            var isExpandingX = targetSize.x > oldSize.x;
            var isExpandingY = targetSize.y > oldSize.y;
            _areaCollider.size = targetSize;

            _currentSquareColor = GetRandomSquareColor();
            var newColor = GetColor(_currentSquareColor);
            var frameCount = 25f;
            var canChangeShapeX = true;
            var canChangeShapeY = true;

            for (int i = 0; i < frameCount; i++)
            {
                if (canChangeShapeX)
                {
                    CheckSurfaces(3);
                    canChangeShapeX = !isExpandingX || (!_hasSurfaceLeft && !_hasSurfaceRight);

                    if (canChangeShapeX)
                    {
                        _spriteRenderer.size = new Vector2(math.lerp(oldSize.x, targetSize.x, i / frameCount), _spriteRenderer.size.y);

                        AdjustColliders();
                        _lineCaster.UpdateLinesVertical();
                    }
                }

                if (canChangeShapeY)
                {
                    canChangeShapeY = !isExpandingY || (!_hasSurfaceUp && !_hasSurfaceDown);

                    if (canChangeShapeY)
                    {
                        _spriteRenderer.size = new Vector2(_spriteRenderer.size.x, math.lerp(oldSize.y, targetSize.y, i / frameCount));

                        AdjustColliders();
                        _lineCaster.UpdateLinesHorizontal();
                    }
                }

                if (oldColor != newColor)
                {
                    _spriteRenderer.color = Color.Lerp(oldColor, newColor, i / frameCount);
                }
                yield return new WaitForSeconds(_transformationTime / frameCount);
            }
            _areaCollider.size = Vector2.zero;
            _spriteRenderer.color = newColor;
            AdjustTrailPosition();
            AdjustSpeed();
            _lineCaster.SetRandomIntersectsCount();
            _isTransforming = false;
        }
    }

    private bool HasSurface(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return _hasSurfaceUp;

            case Direction.Down:
                return _hasSurfaceDown;

            case Direction.Left:
                return _hasSurfaceLeft;

            case Direction.Right:
                return _hasSurfaceRight;

            default:
                break;
        }
        return false;
    }

    private bool CheckSurface(Direction direction, float lengthMultipler = 1)
    {
        var origin = transform.position.ToVector2() + Utills.DirectionToVector(direction) * _detectionRayCastDST * lengthMultipler;
        var colliders = Physics2D.OverlapBoxAll(origin, _shapeCollider.size, 0, _groundLayerMask);
        foreach (var item in colliders)
        {
            if(item != _shapeCollider && item != _areaCollider)
            {
                return true;
            }
        }
        return false;
    }

    private void SetRandomDirection()
    {
        var direction = Direction.Null;
        var directions = new Direction[]
        {
            Direction.Up,
            Direction.Down,
            Direction.Left,
            Direction.Right,
        };
        var freeDirections = directions.Where(d => !HasSurface(d) && Utills.AxisRestrictionAlowsDirection(_movementAxisRestriction, d)).ToList();

        if (freeDirections.Count > 0)
        {
            var playerPos = Player.instance != null ? Player.Position : UnityEngine.Random.insideUnitCircle * 10;
            var directionToPlayer = Utills.VectorToDirection((playerPos - transform.position.ToVector2()).normalized);
            if (freeDirections.Contains(directionToPlayer))
            {
                direction = directionToPlayer;
            }
            else
            {
                direction = freeDirections.RandomItem();
            }
        }
        else
        {
            Debug.Log("Stuck");
        }

        _currentMovementDirection = direction;
        _currentDirectionChangeCooldown = _maxMovementCooldown;
        AdjustTrailPosition();
    }

    private void AdjustTrailPosition()
    {
        AdjustColliders();

        switch (_currentMovementDirection)
        {
            case Direction.Null:
                _trail.transform.localPosition = Vector3.zero;
                break;

            case Direction.Up:
                _trail.transform.localPosition = -ShapeCollider.bounds.extents.ToVector2Y();
                break;

            case Direction.Down:
                _trail.transform.localPosition = ShapeCollider.bounds.extents.ToVector2Y();
                break;

            case Direction.Left:
                _trail.transform.localPosition = ShapeCollider.bounds.extents.ToVector2X();

                break;

            case Direction.Right:
                _trail.transform.localPosition = -ShapeCollider.bounds.extents.ToVector2X();

                break;

            default:
                break;
        }
        var trailWidth = _spriteRenderer.size.y;

        if (_currentMovementDirection == Direction.Up || _currentMovementDirection == Direction.Down)
        {
            trailWidth = _spriteRenderer.size.x;
        }

        if (_currentMovementDirection == Direction.Null) trailWidth = 0;
        _trail.startColor = _spriteRenderer.color;
        _trail.startWidth = trailWidth + _lineCaster.CurrentLineWidth * 2;
        _trail.endWidth = _trail.startWidth;
        _trail.Clear();
    }

    private void ReverseMovementDirection()
    {
        _currentMovementDirection = Utills.ReverseDirection(_currentMovementDirection);
    }
    private void SetColor(SquareColor squareColor)
    {
        _currentSquareColor = squareColor;
        _spriteRenderer.color = GetColor(squareColor);
    }

    private SquareColor GetRandomSquareColor()
    {
        if (_changingColorsEnabled)
        {
            if (_gameField.CanChangeColor(_currentSquareColor))
            {
                var colors = new SquareColor[]
                {
                SquareColor.Red,
                SquareColor.Yellow,
                SquareColor.Blue,
                SquareColor.Black,
                }.Where(c => c != _currentSquareColor).ToList();

                var color = colors.RandomItem();
                foreach (var item in colors)
                {
                    if (_gameField.IsColorMissing(item))
                    {
                        color = item;
                        break;
                    }
                }

                return color;
            }
        }
        return _currentSquareColor;
    }

    private Color GetColor(SquareColor squareColor)
    {
        switch (squareColor)
        {
            case SquareColor.Red:
                return _redColor;

            case SquareColor.Yellow:
                return _yellowColor;

            case SquareColor.Blue:
                return _blueColor;

            case SquareColor.Black:
                return _blackColor;
        }
        throw new System.ArgumentOutOfRangeException();
    }

    private void OnDrawGizmos()
    {
        SetUp();
        Gizmos.color = Color.blue;
        foreach (var direction in Utills.Directions)
        {
            Gizmos.DrawWireCube(transform.position.ToVector2() + Utills.DirectionToVector(direction) * _detectionRayCastDST, _shapeCollider.size);
        }
    }
}