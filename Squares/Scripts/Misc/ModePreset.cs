using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;
[CreateAssetMenu(menuName = "New Mode Preset")]
public class ModePreset : ScriptableObject
{
    [SerializeField] private SceneReference _scene;
    [SerializeField] private int _numberOfSquares;
    [SerializeField] private PresetScoreMode _scoreMode;
    [SerializeField] private string _name;
    [SerializeField] private string _description;

    public SceneReference Scene { get => _scene;}
    public int NumberOfSquares { get => _numberOfSquares;}
    public PresetScoreMode ScoreMode { get => _scoreMode;}
    public string Name { get => _name;}
    public string Description { get => _description;}

    public enum PresetScoreMode
    {
        Coin,
        Time,
    }
}
