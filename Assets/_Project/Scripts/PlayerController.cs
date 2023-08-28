using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using Utilities;

namespace Platformer
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private Rigidbody _rigidbody;
        [SerializeField, Self] private GroundChecker _groundChecker;
        [SerializeField, Self] private Animator _animator;
        [SerializeField, Anywhere] private CinemachineFreeLook _freeLookVCam;
        [SerializeField, Anywhere] private InputReader _input;

        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _rotationSpeed = 500f;
        [SerializeField] private float _smoothTime = 0.2f;

        [Header("Jump Settings")]
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _jumpDuration = 0.5f;
        [SerializeField] private float _jumpCooldown = 0f;
        // [SerializeField] private float _jumpMaxHeight = 2f;
        [SerializeField] private float _gravityMultiplier = 3f;

        [Header("Dash Settings")]
        [SerializeField] private float _dashForce = 10f;
        [SerializeField] private float _dashDuration = 1f;
        [SerializeField] private float _dashCooldown = 2f;

        [Header("Attack Settings")]
        [SerializeField] private float _attackCooldown = 0.5f;
        [SerializeField] private float _attackDistance = 1f;
        [SerializeField] private int _attackDamage = 10;

        private Transform _mainCam;
        private float _currentSpeed;
        private float _velocity;
        private float _jumpVelocity;
        private float _dashVelocity = 1f;

        private Vector3 _movement;

        private List<Timer> _timers;
        private CountdownTimer _jumpTimer;
        private CountdownTimer _jumpCooldownTimer;
        private CountdownTimer _dashTimer;
        private CountdownTimer _dashCooldownTimer;
        private CountdownTimer _attackTimer;

        private StateMachine _stateMachine;

        private static readonly int Speed = Animator.StringToHash("Speed");

        private const float ZeroF = 0f;

        private void Awake()
        {
            _mainCam = Camera.main.transform;
            _freeLookVCam.Follow = transform;
            _freeLookVCam.LookAt = transform;

            // Invoke event when observed transform is teleported, adjusting freeLookVCam's position accordingly
            _freeLookVCam.OnTargetObjectWarped(transform, transform.position - _freeLookVCam.transform.position - Vector3.forward);

            _rigidbody.freezeRotation = true;

            SetupTimers();
            SetupStateMachine();
        }

        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine();

            // Declare states
            var locomotionState = new LocomotionState(this, _animator);
            var jumpState = new JumpState(this, _animator);
            var dashState = new DashState(this, _animator);
            var attackState = new AttackState(this, _animator);

            // Declare transitions
            At(locomotionState, jumpState, new FuncPredicate(() => _jumpTimer.IsRunning));
            At(locomotionState, dashState, new FuncPredicate(() => _dashTimer.IsRunning));
            At(locomotionState, attackState, new FuncPredicate(() => _attackTimer.IsRunning));
            // At(attackState, locomotionState, new FuncPredicate(() => !_attackTimer.IsRunning));
            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));

            // Set initial state
            _stateMachine.SetState(locomotionState);
        }

        private bool ReturnToLocomotionState()
        {
            return _groundChecker.IsGrounded
            && !_attackTimer.IsRunning
            && !_jumpTimer.IsRunning
            && !_dashTimer.IsRunning;
        }

        private void SetupTimers()
        {
            _jumpTimer = new CountdownTimer(_jumpDuration);
            _jumpCooldownTimer = new CountdownTimer(_jumpCooldown);
            _jumpTimer.OnTimerStart += () => _jumpVelocity = _jumpForce;
            _jumpTimer.OnTimerStop += () => _jumpCooldownTimer.Start();

            _dashTimer = new CountdownTimer(_dashDuration);
            _dashCooldownTimer = new CountdownTimer(_dashCooldown);
            _dashTimer.OnTimerStart += () => _dashVelocity = _dashForce;
            _dashTimer.OnTimerStop += () =>
            {
                _dashVelocity = 1f;
                _dashCooldownTimer.Start();
            };

            _attackTimer = new CountdownTimer(_attackCooldown);

            _timers = new List<Timer>(5) { _jumpTimer, _jumpCooldownTimer, _dashTimer, _dashCooldownTimer, _attackTimer };
        }

        private void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);


        private void Start() => _input.EnablePlayerActions();

        private void OnEnable()
        {
            _input.Jump += OnJump;
            _input.Dash += OnDash;
            _input.Attack += OnAttack;
        }

        private void OnDisable()
        {
            _input.Jump -= OnJump;
            _input.Dash -= OnDash;
            _input.Attack -= OnAttack;
        }

        private void OnAttack()
        {
            if (!_attackTimer.IsRunning)
            {
                _attackTimer.Start();
            }
        }

        public void Attack()
        {
            var attackPos = transform.position + transform.forward;
            var hitEnemies = Physics.OverlapSphere(attackPos, _attackDistance);

            foreach (var enemy in hitEnemies)
            {
                Debug.Log(enemy.name);
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<Health>().TakeDamage(_attackDamage);
                }
            }
        }

        private void OnJump(bool performed)
        {
            if (performed && !_jumpTimer.IsRunning && !_jumpCooldownTimer.IsRunning && _groundChecker.IsGrounded)
            {
                _jumpTimer.Start();
            }
            else if (!performed && _jumpTimer.IsRunning)
            {
                _jumpTimer.Stop();
            }
        }

        private void OnDash(bool performed)
        {
            if (performed && !_dashTimer.IsRunning && !_dashCooldownTimer.IsRunning)
            {
                _dashTimer.Start();
            }
            else if (!performed && _dashTimer.IsRunning)
            {
                _dashTimer.Stop();
            }
        }

        private void Update()
        {
            _movement = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
            _stateMachine.Update();

            HandleTimers();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        private void UpdateAnimator()
        {
            _animator.SetFloat(Speed, _currentSpeed);
        }

        private void HandleTimers()
        {
            foreach (var timer in _timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        public void HandleJump()
        {
            // If not jumping and grounded, keep jump velocity at 0
            if (!_jumpTimer.IsRunning && _groundChecker.IsGrounded)
            {
                _jumpVelocity = ZeroF;
                _jumpTimer.Stop();
                return;
            }

            if (!_jumpTimer.IsRunning)
            {
                // Gravity takes over
                _jumpVelocity += Physics.gravity.y * _gravityMultiplier * Time.fixedDeltaTime;
            }
            #region Alternative

            // // If jumping of falling calculate velocity
            // if (_jumpTimer.IsRunning)
            // {
            //     // Progress point for initial burst of velocity
            //     const float launchPoint = 0.9f;
            //     if (_jumpTimer.Progress > launchPoint)
            //     {
            //         // Calculate the velocity required to reach the jump height using physics equations v = sqrt(2gh)
            //         _jumpVelocity = Mathf.Sqrt(2 * _jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
            //     }
            //     else
            //     {
            //         // Gradually apply less velocity as the jump progresses
            //         _jumpVelocity += (1 - _jumpTimer.Progress) * _jumpForce * Time.fixedDeltaTime;
            //     }
            // }
            // else
            // {
            //     // Gravity takes over
            //     _jumpVelocity += Physics.gravity.y * _gravityMultiplier * Time.fixedDeltaTime;
            // }

            #endregion

            // Apply velocity
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpVelocity, _rigidbody.velocity.z);
        }

        public void HandleMovement()
        {
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(_mainCam.eulerAngles.y, Vector3.up) * _movement;

            if (adjustedDirection.magnitude > ZeroF)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(ZeroF);

                // Reset horizontal velocity for a snappy stop
                _rigidbody.velocity = new Vector3(ZeroF, _rigidbody.velocity.y, ZeroF);
            }
        }

        private void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            // Move the player
            var velocity = adjustedDirection * (_moveSpeed * _dashVelocity * Time.fixedDeltaTime);
            _rigidbody.velocity = new Vector3(velocity.x, _rigidbody.velocity.y, velocity.z);
        }

        private void HandleRotation(Vector3 adjustedDirection)
        {
            // Adjust rotation to match movement direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            // transform.LookAt(transform.position + adjustedDirection);
        }

        private void SmoothSpeed(float value)
        {
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, value, ref _velocity, _smoothTime);
        }
    }
}