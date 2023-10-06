using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameField : MonoBehaviour
{
    public static GameField GetActiveField
    {
        get
        {
            var fields = FindObjectsOfType<GameField>();
            return fields.FirstOrDefault(f => f.IsActive);
        }
    }

    private BoxCollider2D _collider2D;
    private List<SquareOrthogonal> _squares = new List<SquareOrthogonal>();
    [SerializeField] private bool _isActive;
    public bool IsActive => _isActive;
    public List<SquareOrthogonal> Squares => _squares;
    public Bounds Bounds => _collider2D.bounds;
    public Action OnMovementUpdate;
    public Action OnVerticalLinesUpdate;
    public Action OnHorizontalLinesUpdate;

    private void Awake()
    {
        _collider2D = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (IsActive)
        {
            if (StartCountdown.TimerIsOver)
            {
                OnMovementUpdate?.Invoke();
            }
            UpdateLines();
        }
    }

    public void UpdateLines()
    {
        Physics2D.SyncTransforms();
        OnVerticalLinesUpdate?.Invoke();
        Physics2D.SyncTransforms();
        OnHorizontalLinesUpdate?.Invoke();
    }

    public void Subscribe(SquareOrthogonal squareOrthogonal)
    {
        if (!_squares.Contains(squareOrthogonal))
        {
            _squares.Add(squareOrthogonal);
        }
        else
        {
            throw new System.Exception("this Square allready subscribed");
        }
    }

    public void UnSubscribe(SquareOrthogonal squareOrthogonal)
    {
        _squares.Remove(squareOrthogonal);
    }

    public void Activate()
    {
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
    }

    public bool CanChangeColor(SquareOrthogonal.SquareColor currentColor)
    {
        if (_squares.Count < 3) return true;
        return _squares.Where(s => s.CurrentSquareColor == currentColor).ToList().Count >= 2;
    }

    public bool IsColorMissing(SquareOrthogonal.SquareColor color)
    {
        return _squares.Where(s => s.CurrentSquareColor == color && s.CanChangeColors).ToList().Count == 0;
    }

    public bool IsTransformationAlowed()
    {
        return StartCountdown.TimerIsOver && _squares.All(s => !s.IsTransforming);
    }

    public Vector2 GetRandomPointOnField(float distanceFromEdge)
    {
        var positionX = UnityEngine.Random.Range(-Bounds.extents.x + distanceFromEdge, Bounds.extents.x - distanceFromEdge);
        var positionY = UnityEngine.Random.Range(-Bounds.extents.y + distanceFromEdge, Bounds.extents.y - distanceFromEdge);
        return transform.position.ToVector2() + new Vector2(positionX, positionY);
    }
}