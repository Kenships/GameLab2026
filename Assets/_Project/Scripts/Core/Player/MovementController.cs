using _Project.Scripts.Core.InputManagement.Interfaces;
using KinematicCharacterController;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Player
{
    public class MovementController : MonoBehaviour<INESActionReader, KinematicCharacterMotor, Camera>, ICharacterController
    {
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 10f;
        [SerializeField] private float planarAcceleration = 10f;
        [SerializeField] private float verticalAcceleration = 20f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float jumpHeight = 5f;
        [SerializeField] private float gravity = 20f;
        
        private INESActionReader _inputReader;
        private KinematicCharacterMotor _motor;
        private Transform _cameraTransform;
        
        private Vector2 _rawMoveInput;
        private Vector3 _moveInputVector;
        private float _currentMovementSpeed;
        private Vector3 _lastLookDirection;
        private bool _jumpRequested;
        
        protected override void Init(INESActionReader argument, KinematicCharacterMotor motor, Camera camera)
        {
            _inputReader = argument;
            _motor = motor;
            _cameraTransform = camera.transform;
        }

        //Potentially change to use DI
        protected override void OnAwake()
        {
            _motor = GetComponent<KinematicCharacterMotor>();
            _motor.CharacterController = this;
        }

        private void Start()
        {
            _currentMovementSpeed = walkSpeed;
            _lastLookDirection = Vector3.forward;
        }

        private void OnEnable()
        {
            _inputReader.OnDPadInput += HandleMove;
        }

        private void OnDisable()
        {
            _inputReader.OnDPadInput -= HandleMove;
        }

        private void HandleMove(Vector2 movementInput)
        {
            _rawMoveInput = movementInput;
        }
        private void UpdateMoveVector()
        {
            Vector3 cameraForward = _cameraTransform.forward;
            Vector3 cameraRight = _cameraTransform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // character direction is relative to the camera
            _moveInputVector = (cameraForward * _rawMoveInput.y + cameraRight * _rawMoveInput.x).normalized;

            if (_moveInputVector.sqrMagnitude > 0.01f)
            {
                // for rotation
                _lastLookDirection = _moveInputVector.normalized;
            }
        }

        private void HandleSprint(bool isSprinting)
        {
            if (isSprinting)
            {
                _currentMovementSpeed = sprintSpeed;
            }
            else
            {
                _currentMovementSpeed = walkSpeed;
            }
        }

        private void HandleJump()
        {
            _jumpRequested = true;
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            UpdateMoveVector();
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_lastLookDirection);
            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * deltaTime);
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (_jumpRequested && _motor.GroundingStatus.IsStableOnGround)
            {
                currentVelocity.y = Mathf.Sqrt(2 * jumpHeight * gravity);
                _jumpRequested = false;
                _motor.ForceUnground();
            }

            if (!_motor.GroundingStatus.IsStableOnGround)
            {
                currentVelocity.y -= gravity * deltaTime;
            }
            else if (currentVelocity.y < 0)
            {
                currentVelocity.y = 0;
            }

            Vector3 horizontalTarget = _moveInputVector * _currentMovementSpeed;
            currentVelocity.x = Mathf.Lerp(currentVelocity.x, horizontalTarget.x, planarAcceleration * deltaTime);
            currentVelocity.z = Mathf.Lerp(currentVelocity.z, horizontalTarget.z, verticalAcceleration * deltaTime);
        }

        public void AfterCharacterUpdate(float deltaTime) { }
        public bool IsColliderValidForCollisions(Collider coll) { return true; }
        public void OnDiscreteCollisionDetected(Collider hitCollider) { }
        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
        public void PostGroundingUpdate(float deltaTime) { }
        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
    }
}
