using System.Collections;
using UnityEngine;

public class TeleportToRandomPointOnFieldAbility : MonoBehaviour, IAbilityOnTarget
{
    [SerializeField] private float _dstFromEdge;
    [SerializeField] private float _dstFromPlayer;
    [SerializeField] private float _dstFromPosition;
    [SerializeField] private float _moveSpeed;
    private Collider2D _collider2D;
    private Vector2 _targetPosition;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
    }

    public void Apply(GameObject gameObject)
    {
        if (gameObject.GetComponent<Player>() != null)
        {
            var field = GetComponentInParent<GameField>();
            for (int i = 0; i < 100; i++)
            {
                var newPosition = field.GetRandomPointOnField(_dstFromEdge);
                var sqrDistanceFromPosition = (newPosition - transform.position.ToVector2()).sqrMagnitude;
                if (sqrDistanceFromPosition > Mathf.Pow(_dstFromPosition, 2))
                {
                    var sqrDistanceFromPlayer = (newPosition - Player.Position).sqrMagnitude;

                    if (sqrDistanceFromPlayer > Mathf.Pow(_dstFromPlayer, 2))
                    {
                        StartCoroutine(MoveToNewPosition(newPosition));
                        return;
                    }
                }
            }
            throw new System.Exception("Can not find any good position");
        }
    }

    private void Teleport(Vector2 vector)
    {
        Debug.Log("teleported");
        transform.localPosition = vector.ToVector3();
    }

    private IEnumerator MoveToNewPosition(Vector2 newPosition)
    {
        _targetPosition = newPosition;
        _collider2D.enabled = false;
        var distance = (newPosition - transform.position.ToVector2()).magnitude;
        var direction = (newPosition - transform.position.ToVector2()).normalized;
        var stepCount = (distance / _moveSpeed) * 10f;

        for (int i = 0; i < stepCount; i++)
        {
            transform.position += (direction * (distance / stepCount)).ToVector3();
            yield return null;
        }
        _collider2D.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_targetPosition, 0.2f);
    }
}