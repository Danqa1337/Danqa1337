using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextToTextmeshProConverter : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

    // Update is called once per frame
    void Update()
    {
        _textMeshProUGUI.text = _text.text;
    }
}
