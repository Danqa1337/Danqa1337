using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fossil : MonoBehaviour, ITouchable, IBurnable
{
    [SerializeField] private Sprite[] _sprites;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    public Rigidbody2D Rigidbody { get => _rigidbody; }

    private bool _isFree;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _spriteRenderer.sprite = _sprites.RandomItem();
    }

    public void Touch()
    {
        if (!_isFree)
        {
            _isFree = true;
            transform.SetParent(null);
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public float Burn()
    {
        return 50;
    }
}

public interface ITouchable
{
    public abstract void Touch();
}