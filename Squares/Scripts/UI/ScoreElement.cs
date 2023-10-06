using UnityEngine;

public class ScoreElement : MonoBehaviour
{
    [SerializeField] private Label _tablePositionLabel;
    [SerializeField] private Label _usernameLabel;
    [SerializeField] private Label _scoreLabel;

    public void WrightScore(int tablePosition, string username, int score)
    {
        _tablePositionLabel.SetText(tablePosition.ToString());
        _usernameLabel.SetText(username);
        _scoreLabel.SetText(score.ToString());
    }
}