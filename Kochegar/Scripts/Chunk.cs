using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Chunk : MonoBehaviour, IBurnable, ISliceble, ITouchable
{
    [SerializeField] private float _destroyChunkMaxMass = 0.1f;
    [SerializeField] private ParticleSystem _onDestroyParticles;
    [SerializeField] private ParticleSystem _onBreakParticles;
    [SerializeField] private ParticleSystem _onBurnParticles;

    private Rigidbody2D _rigidbody2D;
    private PolygonCollider2D _collider2D;
    private MeshFilter _meshFilter;

    public Rigidbody2D Rigidbody => _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _meshFilter = GetComponent<MeshFilter>();
        _collider2D = GetComponent<PolygonCollider2D>();
    }

    public void AdjustMesh()
    {
        var mesh = _meshFilter.mesh;
        PolygonColiderCreator.SetColliderPoints(mesh, _collider2D);
    }

    public void Destroy()
    {
        Instantiate(_onDestroyParticles.gameObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public GameObject[] Slice(Vector2 position, Vector2 normal)
    {
        AudioManager.PlayEvent(SoundName.Crack);
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        if (_rigidbody2D.mass < _destroyChunkMaxMass)
        {
            Destroy();
            return null;
        }
        else
        {
            Instantiate(_onBreakParticles.gameObject, transform.position, Quaternion.identity);
            return ChunkSlicer.Slice(this, position, normal);
        }
    }

    public float Burn()
    {
        Instantiate(_onBurnParticles.gameObject, transform.position, Quaternion.identity);

        return _rigidbody2D.mass * 100;
    }

    public void Touch()
    {
        var sliceNormal = UnityEngine.Random.insideUnitCircle;
        var pices1 = Slice(transform.GetComponent<Renderer>().bounds.center, sliceNormal.ToVector3());
        if (pices1 != null)
        {
            foreach (var piece1 in pices1)
            {
                var pieces2 = piece1.GetComponent<ISliceble>().Slice(piece1.transform.GetComponent<Renderer>().bounds.center, Random.insideUnitCircle);
            }
        }
    }
}

public interface IBurnable
{
    public float Burn();
}

public interface ISliceble
{
    public GameObject gameObject { get; }
    public Transform transform { get; }

    public GameObject[] Slice(Vector2 position, Vector2 normal);
}