using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableLineOnSideInteraction : MonoBehaviour
{
    private float topPoints, bottomPoints, leftPoints, rightPoints;
    [SerializeField] private float _interactionTime = 1;

    private SquareOrthogonal _squareOrthogonal;

    private void Awake()
    {
        _squareOrthogonal = GetComponentInParent<SquareOrthogonal>();
        topPoints = _interactionTime;
        bottomPoints = _interactionTime;
        leftPoints = _interactionTime;
        rightPoints = _interactionTime;
    }
    public void ProcessInteraction(Direction direction)
    {
        switch (direction)
        {
            case Direction.Null:
                break;
            case Direction.Up:
                topPoints -= Time.fixedDeltaTime;
                if (topPoints < 0) DisableLine(direction);
                break;
            case Direction.Down:
                bottomPoints -= Time.fixedDeltaTime;
                if (bottomPoints < 0) DisableLine(direction);

                break;
            case Direction.Left:
                leftPoints -= Time.fixedDeltaTime;
                if (leftPoints < 0) DisableLine(direction);

                break;
            case Direction.Right:
                rightPoints -= Time.fixedDeltaTime;
                if (rightPoints < 0) DisableLine(direction);

                break;
            default:
                break; 
        }
    }
    private void DisableLine(Direction direction)
    {
        Debug.Log(direction + " line disabled");
        switch (direction)
        {
            case Direction.Null: 
                break;
            case Direction.Up:
                _squareOrthogonal.lineLimitTop = -1;
                break;
            case Direction.Down:
                _squareOrthogonal.lineLimitBottom = -1;
                break;
            case Direction.Left:
                _squareOrthogonal.lineLimitLeft = -1;
                break;
            case Direction.Right:
                _squareOrthogonal.lineLimitRight = -1;
                break;
            default:
                break;
        }
    }
}
