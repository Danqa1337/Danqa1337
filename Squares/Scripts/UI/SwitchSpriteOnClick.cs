using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class SwitchSpriteOnClick : MonoBehaviour
{
    [SerializeField] private Sprite _firstSprite;
    [SerializeField] private Sprite _secondSprite;
    [SerializeField] private Image _image;
    private bool _switched;
    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(Switch);
    }
    public void Switch()
    {
        if(_switched)
        {
            _image.sprite = _firstSprite;
            _switched = false;
        }
        else
        {
            _image.sprite = _secondSprite;
            _switched = true;
        }
    }
}
