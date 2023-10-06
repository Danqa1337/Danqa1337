using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// Right now it only contains movement and jumping, but it should be pretty easy to expand... I may even do it myself
    /// if there's enough interest. You can play and compete for best times here: https://tarodev.itch.io/
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/GqeHHnhHpz
    /// </summary>
    public class PlayerMovement : MonoBehaviour, IPlayerController
    {
        // Public for external hooks
        public Vector3 Velocity { get; private set; }

        public FrameInput Input { get; private set; }
        public bool JumpingThisFrame { get; private set; }
        public bool LandingThisFrame { get; private set; }
        public Vector3 RawMovement { get; private set; }
        public bool Grounded => _colDown;

        [Header("COLLISION")]
        public Bounds _characterBounds;

        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private int _detectorCount = 3;
        [SerializeField] private float _detectionRayLength = 0.1f;
        [SerializeField][Range(0.01f, 0.3f)] private float _sideRayBuffer = 0.1f; // Prevents side detectors hitting the ground
        [SerializeField][Range(0.01f, 0.3f)] private float _topRayBuffer = 0.1f; // Prevents side detectors hitting the ground
        [SerializeField][Range(0.01f, 0.3f)] private float _bottomRayBuffer = 0.1f; // Prevents side detectors hitting the ground

        [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
        private int _freeColliderIterations = 10;

        private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
        private bool _colUp, _colRight, _colDown, _colLeft;

        private float _timeLeftGrounded;

        [Header("WALKING")]
        [SerializeField] private float _acceleration = 90;

        [SerializeField] private float _moveClamp = 13;
        [SerializeField] private float _deAcceleration = 60f;
        [SerializeField] private float _apexBonus = 2;

        [Header("GRAVITY")]
        [SerializeField] private float _fallClamp = -40f;

        [SerializeField] private float _minFallSpeed = 80f;
        [SerializeField] private float _maxFallSpeed = 120f;
        [SerializeField] private float _slideMultipler = 0.2f;
        private float _fallSpeed;

        [Header("JUMPING")][SerializeField] private float _jumpHeight = 30;
        [SerializeField] private float _jumpApexThreshold = 10f;
        [SerializeField] private float _coyoteTimeThreshold = 0.1f;
        [SerializeField] private float _jumpBuffer = 0.1f;
        [SerializeField] private float _jumpEndEarlyGravityModifier = 3;
        [SerializeField] private float _jumpOfWallMultipler = 0.5f;
        [SerializeField] private float _squizeSpeed;

        private bool _coyoteUsable;
        private bool _endedJumpEarly = true;
        private float _apexPoint; // Becomes 1 at the apex of a jump
        private float _lastJumpPressed;
        private bool CanUseCoyote => _coyoteUsable && !_colDown && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
        private bool HasBufferedJump => (_colDown || _colRight || _colLeft) && _lastJumpPressed + _jumpBuffer > Time.time;

        private Vector3 _lastPosition;
        private float _currentHorizontalSpeed, _currentVerticalSpeed;
        private Rigidbody2D _rigidbody;
        private ParticleSystem _particleSystem;
        private TrailRenderer _trailRenderer;
        private Animator _animator;
        private AudioSource _jumpAudioSource;

        // This is horrible, but for some reason colliders are not fully established when update starts...
        private bool _active;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _trailRenderer = GetComponent<TrailRenderer>();
            _animator = GetComponent<Animator>();
            _jumpAudioSource = GetComponent<AudioSource>();

            Invoke(nameof(Activate), 0.5f);
        }

        private void Activate() => _active = true;

        private void Update()
        {
            if (!_active || !Player.Alive || !StartCountdown.TimerIsOver) return;

            Velocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;

            GatherInput();
            RunCollisionChecks();

            CalculateWalk(); // Horizontal movement
            CalculateJumpApex(); // Affects fall speed, so calculate before gravity
            CalculateGravity(); // Vertical movement
            CalculateJump(); // Possibly overrides vertical
            CalculateSlide();
            CalculateAnimations();
            MoveCharacter(); // Actually perform the axis movement
        }

        private void GatherInput()
        {
            Input = Controller.CurrentFrameInput;
            if (Input.JumpDown)
            {
                _lastJumpPressed = Time.time;
            }
        }

        private void RunCollisionChecks()
        {
            // Generate ray ranges.
            CalculateRayRanged();

            // Ground
            LandingThisFrame = false;
            var groundedCheck = RunDetection(_raysDown);
            if (_colDown && !groundedCheck) _timeLeftGrounded = Time.time; // Only trigger when first leaving
            else if (!_colDown && groundedCheck)
            {
                _coyoteUsable = true; // Only trigger when first touching
                LandingThisFrame = true;
            }

            _colDown = groundedCheck;

            // The rest
            _colUp = RunDetection(_raysUp);
            _colLeft = RunDetection(_raysLeft);
            _colRight = RunDetection(_raysRight);

            bool RunDetection(RayRange range)
            {
                return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, Utills.DirectionToVector(range.Direction), _detectionRayLength, _groundLayer));
            }
        }

        private void CalculateRayRanged()
        {
            // This is crying out for some kind of refactor.
            var b = new Bounds(transform.position + _characterBounds.center, _characterBounds.size);

            _raysDown = new RayRange(b.min.x + _bottomRayBuffer, b.min.y, b.max.x - _bottomRayBuffer, b.min.y, Direction.Down);
            _raysUp = new RayRange(b.min.x + _topRayBuffer, b.max.y, b.max.x - _topRayBuffer, b.max.y, Direction.Up);
            _raysLeft = new RayRange(b.min.x, b.min.y + _sideRayBuffer, b.min.x, b.max.y - _sideRayBuffer, Direction.Left);
            _raysRight = new RayRange(b.max.x, b.min.y + _sideRayBuffer, b.max.x, b.max.y - _sideRayBuffer, Direction.Right);
        }

        private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
        {
            for (var i = 0; i < _detectorCount; i++)
            {
                var t = (float)i / (_detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

            if (!Application.isPlaying)
            {
                CalculateRayRanged();
                Gizmos.color = Color.blue;
                foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft })
                {
                    foreach (var point in EvaluateRayPositions(range))
                    {
                        Gizmos.DrawRay(point, Utills.DirectionToVector(range.Direction) * _detectionRayLength);
                    }
                }
            }

            if (!Application.isPlaying) return;

            Gizmos.color = Color.red;
            var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
            Gizmos.DrawWireCube(transform.position + _characterBounds.center + move, _characterBounds.size);
        }

        private void CalculateWalk()
        {
            if (Input.X != 0)
            {
                // Set horizontal move speed
                _currentHorizontalSpeed += Input.X * _acceleration * Time.deltaTime;

                // clamped by max frame movement
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);

                // Apply bonus at the apex of a jump
                var apexBonus = Mathf.Sign(Input.X) * _apexBonus * _apexPoint;
                _currentHorizontalSpeed += apexBonus * Time.deltaTime;
            }
            else
            {
                // No input. Let's slow the character down
                _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
            }

            if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft)
            {
                // Don't walk through walls
                _currentHorizontalSpeed = 0;
            }
        }

        private void CalculateGravity()
        {
            if (_colDown)
            {
                // Move out of the ground
                if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
            }
            else
            {
                // Add downward force while ascending if we ended the jump early
                var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;
                // Fall
                _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

                // Clamp
                if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
            }
        }

        private void CalculateJumpApex()
        {
            if (!_colDown)
            {
                // Gets stronger the closer to the top of the jump
                _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
            }
            else
            {
                _apexPoint = 0;
            }
        }

        private void CalculateJump()
        {
            // Jump if: grounded or within coyote threshold || sufficient jump buffer
            if (Input.JumpDown && CanUseCoyote || HasBufferedJump)
            {
                if (!JumpingThisFrame)
                {
                    _particleSystem.Play();

                    _jumpAudioSource.Play();
                }
                if (!_colDown && !JumpingThisFrame)
                {
                    if (_colLeft) _currentHorizontalSpeed += _jumpHeight * _jumpOfWallMultipler;
                    if (_colRight) _currentHorizontalSpeed += _jumpHeight * -_jumpOfWallMultipler;
                }

                _endedJumpEarly = false;
                _coyoteUsable = false;
                _timeLeftGrounded = float.MinValue;
                JumpingThisFrame = true;
                _currentVerticalSpeed = _jumpHeight;
            }
            else
            {
                JumpingThisFrame = false;
            }

            // End the jump early if button released
            if (!_colDown && Input.JumpUp && !_endedJumpEarly && Velocity.y > 0)
            {
                // _currentVerticalSpeed = 0;
                _endedJumpEarly = true;
            }

            if (_colUp)
            {
                if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
            }
        }

        private void CalculateSlide()
        {
            if (_currentVerticalSpeed < 0)
            {
                if ((_colLeft && Input.X < 0) || (_colRight && Input.X > 0))
                {
                    _currentVerticalSpeed *= _slideMultipler;
                }
            }
            if (_colLeft && _colRight)
            {
                if (_currentVerticalSpeed > 0 && !_colDown)
                {
                    _currentVerticalSpeed += _squizeSpeed;
                }
                else if (_currentVerticalSpeed < 0 && !_colDown)
                {
                    _currentVerticalSpeed -= _squizeSpeed;
                }
            }
        }

        private void CalculateAnimations()
        {
            if (_colDown)
            {
                if (_currentHorizontalSpeed != 0)
                {
                    _animator.Play("Run");
                }
                else
                {
                    _animator.Play("Stand");
                }
            }
            else
            {
                if (_currentVerticalSpeed > 0)
                {
                    _animator.Play("Jump");
                }
                else
                {
                    if (_colLeft || _colRight)
                    {
                        if (_colLeft)
                        {
                            _animator.Play("SlideLeft");
                        }
                        else
                        {
                            _animator.Play("SlideRight");
                        }
                    }
                    else
                    {
                        _animator.Play("Fall");
                    }
                }
            }
        }

        private void MoveCharacter()
        {
            _rigidbody.velocity = new Vector2(_currentHorizontalSpeed, _currentVerticalSpeed);
        }
    }
}