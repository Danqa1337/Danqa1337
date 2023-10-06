using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ControllsHint : MonoBehaviour
{
    [SerializeField] private float _delaySeconds;
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        StartCountdown.OnCountdownOver += Hide;
    }

    private void OnDisable()
    {
        StartCountdown.OnCountdownOver -= Hide;
    }

    private void Hide()
    {
        StartCoroutine(HideWithDelay());
        IEnumerator HideWithDelay()
        {
            yield return new WaitForSeconds(_delaySeconds);
            _canvas.enabled = false;
        }
    }
}