using UnityEngine;

public static class Utills
{
    public static Direction[] Directions => new Direction[]
    {
        Direction.Up,
        Direction.Down,
        Direction.Left,
        Direction.Right,
    };

    public static bool AxisRestrictionAlowsDirection(AxisRestriction axisRestriction, Direction direction)
    {
        if (axisRestriction == AxisRestriction.None || direction == Direction.Null)
        {
            return true;
        }
        else
        {
            if (axisRestriction == AxisRestriction.Vertical)
            {
                return direction == Direction.Up || direction == Direction.Down;
            }
            else
            {
                return direction == Direction.Left || direction == Direction.Right;
            }
        }
    }

    public static bool CheckSurface(Vector2 position, Vector2 direction, float dst, LayerMask layerMask, GameObject excludedRoot)
    {
        var end = position + direction.normalized * dst;
        var result = Physics2D.LinecastAll(position, end);
        foreach (var item in result)
        {
            if (item.collider.transform.root != excludedRoot.transform.root)
            {
                if ((layerMask.value & (1 << item.collider.gameObject.layer)) != 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static Vector2 Turn(Direction direction, Vector2 vector)
    {
        switch (direction)
        {
            case Direction.Up:
                return new Vector2(vector.x, vector.y);

            case Direction.Down:
                return new Vector2(vector.x * -1, vector.y * -1);

            case Direction.Left:
                return new Vector2(vector.y * -1, vector.x);

            case Direction.Right:
                return new Vector2(vector.y, vector.x * -1);

            default:
                break;
        }

        throw new System.ArgumentOutOfRangeException();
    }

    public static Direction Turn(Direction direction, Direction directionToTurn)
    {
        return VectorToDirection(Turn(direction, DirectionToVector(directionToTurn)));
    }

    public static Direction VectorToDirection(Vector2 vector)
    {
        if (vector == Vector2.up) return Direction.Up;
        if (vector == Vector2.down) return Direction.Down;
        if (vector == Vector2.left) return Direction.Left;
        if (vector == Vector2.right) return Direction.Right;
        if (vector == Vector2.zero) return Direction.Null;

        float angle = Vector2.SignedAngle(vector, new Vector2(-1, 1));

        if (angle >= 0 && angle <= 90)
        {
            return Direction.Up;
        }
        if (angle > 90)
        {
            return Direction.Right;
        }
        if (angle < 0 && angle > -90)
        {
            return Direction.Left;
        }
        if (angle <= -90)
        {
            return Direction.Down;
        }

        throw new System.ArgumentOutOfRangeException();
    }

    public static Vector2 DirectionToVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector2.up;

            case Direction.Down:
                return Vector2.down;

            case Direction.Left:
                return Vector2.left;

            case Direction.Right:
                return Vector2.right;

            default:
                break;
        }

        throw new System.ArgumentOutOfRangeException();
    }

    public static Direction ReverseDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;

            case Direction.Down:
                return Direction.Up;

            case Direction.Left:
                return Direction.Right;

            case Direction.Right:
                return Direction.Left;

            default: return Direction.Null;
        }

        throw new System.ArgumentOutOfRangeException();
    }
}