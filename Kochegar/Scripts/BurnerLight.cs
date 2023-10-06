using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BurnerLight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _glow;

    private void Update()
    {
        _glow.size = new Vector2(_glow.size.x, Mathf.Lerp(2f, 6.5f, Termometer.NormalizedTemperature));
    }
}