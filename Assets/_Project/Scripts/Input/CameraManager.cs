using System;
using System.Collections;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;

namespace Platformer
{
    public class CameraManager : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField, Anywhere] private InputReader _input;
        [SerializeField, Anywhere] private CinemachineFreeLook _freeLookVCam;

        [Header("Settings")] 
        [SerializeField, Range(0.5f, 3f)] private float _speedMultiplier = 1f;

        private bool _isRMBPressed;
        private bool _cameraMovementLock;

        private void OnEnable()
        {
            _input.Look += OnLook;
            _input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            _input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        }

        private void OnDisable()
        {
            _input.Look -= OnLook;
            _input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            _input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        }

        private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if (_cameraMovementLock) return;
            if(isDeviceMouse && !_isRMBPressed) return;
            
            // If the device is mouse use fixedDeltaTime, otherwise use deltaTime
            var deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;
            
            // Set the camera axis values
            _freeLookVCam.m_XAxis.m_InputAxisValue = cameraMovement.x * _speedMultiplier * deviceMultiplier;
            _freeLookVCam.m_YAxis.m_InputAxisValue = cameraMovement.y * _speedMultiplier * deviceMultiplier;
        }
        
        private void OnEnableMouseControlCamera()
        {
            _isRMBPressed = true;
            
            // Lock the cursor to the center of the screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartCoroutine(DisableMouseForFrame());
        }

        private IEnumerator DisableMouseForFrame()
        {
            _cameraMovementLock = true;
            yield return new WaitForEndOfFrame();
            _cameraMovementLock = false;
        }
        
        private void OnDisableMouseControlCamera()
        {
            _isRMBPressed = false;
            
            // Unlock the cursor and make it visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Reset the camera axis to prevent jumping when re-enabling mouse control
            _freeLookVCam.m_XAxis.m_InputAxisValue = 0f;
            _freeLookVCam.m_YAxis.m_InputAxisValue = 0f;
        }
    }
}