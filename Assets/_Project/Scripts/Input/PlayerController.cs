using System;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private CharacterController _controller;
        [SerializeField, Self] private Animator _animator;
        [SerializeField, Anywhere] private CinemachineFreeLook _freeLookVCam;
        [SerializeField, Anywhere] private InputReader _input;

        [Header("Settings")] 
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _rotationSpeed = 500f;
        [SerializeField] private float _smoothTime = 0.2f;

        private Transform _mainCam;
        private float _currentSpeed;
        private float _velocity;
        private static readonly int Speed = Animator.StringToHash("Speed");

        private const float ZeroF = 0f;

        private void Awake()
        {
            _mainCam = Camera.main.transform;
            _freeLookVCam.Follow = transform;
            _freeLookVCam.LookAt = transform;
            
            // Invoke event when observed transform is teleported, adjusting freeLookVCam's position accordingly
            _freeLookVCam.OnTargetObjectWarped(transform, transform.position - _freeLookVCam.transform.position - Vector3.forward);
        }

        private void Start() => _input.EnablePlayerActions();

        private void Update()
        {
            HandleMovement();
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            _animator.SetFloat(Speed, _currentSpeed);
        }

        private void HandleMovement()
        {
            var movementDirection = new Vector3(_input.Direction.x, 0f, _input.Direction.y).normalized;
            
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(_mainCam.eulerAngles.y, Vector3.up) * movementDirection;

            if (adjustedDirection.magnitude > ZeroF)
            {
                HandleRotation(adjustedDirection);
                HandleCharacterController(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(ZeroF);
            }
        }

        private void HandleCharacterController(Vector3 adjustedDirection)
        {
            // Move the player
            var adjustedMovement = adjustedDirection * (_moveSpeed * Time.deltaTime);
            _controller.Move(adjustedMovement);
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