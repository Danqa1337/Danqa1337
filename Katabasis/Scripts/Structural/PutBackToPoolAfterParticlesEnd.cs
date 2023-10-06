using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PutBackToPoolAfterParticlesEnd : MonoBehaviour
{

    private async void OnEnable()
    {

        var systems = GetComponentsInChildren<ParticleSystem>().ToList();
        systems.Add(GetComponent<ParticleSystem>());
        foreach (var system in systems)
        {
            system.Play();
        }

        StartCoroutine(wait());
        IEnumerator wait()
        {
            var delay = systems.Max(s => s.main.duration);
            yield return new WaitForSeconds(delay + 0.2f);

            Pooler.PutObjectBackToPool(gameObject);
        }
    
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
