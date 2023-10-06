using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Burner : MonoBehaviour
{
    public static event Action<float> OnChunkBurned;

    [SerializeField] private ParticleSystem _smokeParticles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<IBurnable>() != null)
        {
            OnChunkBurned?.Invoke(collision.GetComponent<IBurnable>().Burn());
            Destroy(collision.gameObject);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        OnChunkBurned?.Invoke(0.1f);
    }
}