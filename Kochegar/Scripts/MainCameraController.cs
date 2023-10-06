using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCameraController : Singleton<MainCameraController>
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector2 _playerWindowSize;
    [SerializeField] private Vector2 _playerWindowOffset;
    [SerializeField] private Transform _cameraFolowPoint;
    private Camera _camera;
    public static Camera Camera => instance._camera;

    [SerializeField] private bool _flow;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (_cameraFolowPoint != null)
        {
            Gizmos.DrawWireCube(_cameraFolowPoint.position.ToVector2() + _playerWindowOffset, _playerWindowSize.ToVector3());
        }
    }

    private void StartFollow()
    {
        _flow = true;
    }

    private void StopFollow()
    {
        _flow = false;
    }

    private void Update()
    {
        if (_flow && _cameraFolowPoint != null)
        {
            var flowPointPosition = _cameraFolowPoint.transform.position.ToVector2();
            var camerPosition = transform.position.ToVector2();

            var window = new Rect(flowPointPosition + _playerWindowOffset - _playerWindowSize, _playerWindowSize * 2);
            if (!window.Contains(camerPosition))
            {
                var newPosition = camerPosition + (flowPointPosition - camerPosition + _playerWindowOffset).normalized * Time.deltaTime * _moveSpeed;
                transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            }
        }
    }
}