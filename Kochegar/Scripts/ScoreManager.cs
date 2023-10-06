using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Label _scoreLabel;
    private float _score;

    private void OnEnable()
    {
        Burner.OnChunkBurned += OnSmthBurned;
    }

    private void OnDisable()
    {
        Burner.OnChunkBurned -= OnSmthBurned;
    }

    private void Start()
    {
        _scoreLabel.SetValue(_score);
    }

    private void OnSmthBurned(float score)
    {
        AddScore(Mathf.Max(0, score));
    }

    private void AddScore(float score)
    {
        _score += score;
        _scoreLabel.SetValue(_score);
    }
}