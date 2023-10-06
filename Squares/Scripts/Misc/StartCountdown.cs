using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCountdown : Singleton<StartCountdown>
{
    public static bool TimerIsOver { get => instance != null ? instance._timerIsOver : true; }
    private bool _timerIsOver = false;
    [SerializeField] private float _squaresMoveSpeed = 40;

    public static event Action OnCountdownOver;

    private void Start()
    {
        DoCountdown();
    }

    private void DoCountdown()
    {
        StartCoroutine(IDoCountdown());
    }

    private IEnumerator IDoCountdown()
    {
        var field = Player.GameField;
        var squares = field.Squares;
        var squaresAndPositions = new Dictionary<Square, Vector3>();

        foreach (var square in squares)
        {
            var currentPosition = square.transform.localPosition;
            squaresAndPositions.Add(square, currentPosition);
            var startPosition = currentPosition.x >= 0 ? new Vector2(20, currentPosition.y) : new Vector2(-20, currentPosition.y);
            square.transform.localPosition = startPosition;
        }
        foreach (var square in squares)
        {
            var targetPosition = squaresAndPositions[square];
            var startPosition = square.transform.localPosition;
            var frameCount = 25f;
            for (float i = 0; i < frameCount; i++)
            {
                square.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, i / frameCount);
                field.UpdateLines();
                yield return new WaitForSeconds(1f / _squaresMoveSpeed);
            }
        }
        _timerIsOver = true;
        OnCountdownOver?.Invoke();
    }
}