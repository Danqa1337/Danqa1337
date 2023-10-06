using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSounds : MonoBehaviour
{
    [Min(0.01f)]
    [SerializeField] private float _speed;

    private void Start()
    {
        StartCoroutine(PlaySounds());
        IEnumerator PlaySounds()
        {
            while (true)
            {
                if (Termometer.NormalizedTemperature > 0)
                {
                    AudioManager.PlayEvent(SoundName.Bup);
                    yield return new WaitForSeconds(Mathf.Min(3, 1f / _speed / Termometer.NormalizedTemperature));
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }
}