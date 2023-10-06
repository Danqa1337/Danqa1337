using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] protected BoxCollider2D _shapeCollider;
    public BoxCollider2D ShapeCollider => _shapeCollider;
}