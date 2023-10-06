using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _maxHealth;
    private static GameField _gameField;
    private static bool _alive;
    public static GameField GameField => _gameField;

    public static event Action OnPlayerSpawned;

    public static event Action OnPlayerDied;

    public static Vector2 Position => instance.transform.position.ToVector2();
    public int CurrentHealth => _currentHealth;
    public static bool Alive => _alive;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _gameField = GetComponentInParent<GameField>();
        _alive = true;
        OnPlayerSpawned?.Invoke();
    }

    public static void ApplyDamage(int damage)
    {
        if (Alive)
        {
            instance._currentHealth -= damage;
            if (instance._currentHealth <= 0)
            {
                instance.Die();
            }
        }
    }

    private async void Die()
    {
        StartCoroutine(Death());
        IEnumerator Death()
        {
            _alive = false;
            Pooler.Take("DeathParticles", instance.transform.position);
            Debug.Log("Player died");
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(1);
            OnPlayerDied?.Invoke();
        }
    }
}