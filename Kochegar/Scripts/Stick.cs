using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    [SerializeField] private float _strikeSpeed = 250;
    [SerializeField] private float _strikeEndDelaySeconds = 0.2f;
    [SerializeField] private float _timeSinceLastImpact;
    [SerializeField] private float _cooldown = 1;
    [SerializeField] private Toucher _toucher;
    [SerializeField] private CirclePusher _pointPusher;
    public bool Ready => _timeSinceLastImpact >= _cooldown && !_operating;
    private bool _operating;

    private void Update()
    {
        _timeSinceLastImpact += Time.deltaTime;
    }

    public void Strike(Vector2 touchPosition)
    {
        if (Ready)
        {
            _timeSinceLastImpact = 0;
            StartCoroutine(Strike());
        }

        IEnumerator Strike()
        {
            _operating = true;
            Debug.Log("strike");
            AudioManager.PlayEvent(SoundName.Piston);

            var startPosition = transform.position.ToVector2();

            var diff = (touchPosition - startPosition).normalized;
            var rotationZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg + 90;
            transform.position = startPosition;
            transform.rotation = Quaternion.Euler(0, 0, rotationZ);

            var frameCount = (touchPosition - startPosition).magnitude;
            for (float i = 0; i < frameCount; i++)
            {
                transform.position = Vector2.Lerp(startPosition, touchPosition, i / frameCount);
                yield return new WaitForSeconds(1f / _strikeSpeed);
            }
            transform.position = touchPosition;
            _toucher.Touch();
            _pointPusher.Push();
            yield return new WaitForSeconds(_strikeEndDelaySeconds);

            for (float i = 0; i < frameCount; i++)
            {
                transform.position = Vector2.Lerp(touchPosition, startPosition, i / frameCount);
                yield return new WaitForSeconds(1f / _strikeSpeed);
            }
            transform.position = startPosition;
            _operating = false;
        }
    }
}