using System.Collections;
using UnityEngine;

public class PlayersHud : MonoBehaviour
{
    [SerializeField] private Label _scoreLabel;

    private void Start()
    {
        _scoreLabel.SetValue(0);
    }

    private void OnEnable()
    {
        RuntimeScoreCounter.OnScoreChanged += _scoreLabel.SetValue;
    }

    private void OnDisable()
    {
        RuntimeScoreCounter.OnScoreChanged -= _scoreLabel.SetValue;
    }

    public void Menu()
    {
        SceneLoader.LoadScene(0);
    }
}