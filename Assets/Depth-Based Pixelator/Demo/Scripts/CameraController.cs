using UnityEngine;
using UnityEngine.InputSystem;

namespace Depth_Based_Pixelator.Sample.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float sprintMultiplier = 2f;
        public float moveSmoothTime = 0.1f;

        [Header("Mouse Look")]
        public float mouseSensitivity = 2f;
        public float lookSmoothTime = 0.05f;
        public bool lockCursor = true;

        private Vector2 moveInput;
        private Vector2 currentMoveVelocity;
        private Vector2 smoothedMoveInput;

        private Vector2 lookInput;
        private Vector2 currentLookVelocity;
        private Vector2 smoothedLookInput;

        private float rotationX;
        private float rotationY;

        void Start()
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void Update()
        {
            var kb = Keyboard.current;
            var mouse = Mouse.current;

            // --- Movement input ---
            moveInput = Vector2.zero;
            if (kb.wKey.isPressed) moveInput.y += 1;
            if (kb.sKey.isPressed) moveInput.y -= 1;
            if (kb.aKey.isPressed) moveInput.x -= 1;
            if (kb.dKey.isPressed) moveInput.x += 1;

            // Smoothly interpolate movement input
            smoothedMoveInput = Vector2.SmoothDamp(smoothedMoveInput, moveInput, ref currentMoveVelocity, moveSmoothTime);

            // --- Mouse look ---
            lookInput = mouse.delta.ReadValue() * mouseSensitivity;
            smoothedLookInput = Vector2.SmoothDamp(smoothedLookInput, lookInput, ref currentLookVelocity, lookSmoothTime);

            rotationY += smoothedLookInput.x * Time.deltaTime;
            rotationX -= smoothedLookInput.y * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);

            transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);

            // --- Apply movement ---
            Vector3 move = transform.right * smoothedMoveInput.x + transform.forward * smoothedMoveInput.y;
            float speed = moveSpeed * (kb.leftShiftKey.isPressed ? sprintMultiplier : 1f);
            transform.position += move * (speed * Time.deltaTime);
        }
    }
}
