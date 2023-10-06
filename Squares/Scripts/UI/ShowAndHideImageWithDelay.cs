using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class ShowAndHideImageWithDelay : MonoBehaviour
{
    [SerializeField] private float _delaySeconds = 1f;
    private Image _image;
    private Color _defaultImageColor;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        StopAllCoroutines();
        _image.color = _defaultImageColor;
        Hide();
    }

    private void Hide()
    {
        StartCoroutine(HideWithDelay());
        IEnumerator HideWithDelay()
        {
            yield return new WaitForSeconds(_delaySeconds);
            _defaultImageColor = _image.color;
            _image.color = Color.clear;
        }
    }
}