using TMPro;
using UnityEngine;

public class DistanceMeterView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _numbersTexts;

    private void OnEnable()
    {
        DistanceMeter.OnDistanceUpdated += OnDistanceUpdated;
    }

    private void OnDisable()
    {
        DistanceMeter.OnDistanceUpdated -= OnDistanceUpdated;
    }

    private void OnDistanceUpdated(float distance)
    {
        var dstString = ((int)distance).ToString("D5");
        for (int i = 0; i < _numbersTexts.Length; i++)
        {
            _numbersTexts[i].text = dstString[i].ToString();
        }
    }
}