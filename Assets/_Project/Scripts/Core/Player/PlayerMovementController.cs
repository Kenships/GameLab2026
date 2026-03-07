using _Project.Scripts.Core.InputManagement.Interfaces;
using KinematicCharacterController;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Player
{
    public class PlayerMovementController : MonoBehaviour<INESActionReader, KinematicCharacterMotor, Camera>, ICharacterController
    {
        [Header("References")]
        [SerializeField] private Animator _animator;
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 10f;
        [SerializeField] private float planarAcceleration = 10f;
        [SerializeField] private float verticalAcceleration = 20f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float jumpHeight = 5f;
        [SerializeField] private float gravity = 20f;
        
        private INESActionReader _inputReader;
        private KinematicCharacterMotor _motor;
        
        private Vector2 _rawMoveInput;
        private Vector3 _moveInputVector;
        private float _currentMovementSpeed;
        private Vector3 _lastLookDirection;
        private bool _jumpRequested;

        private bool _disableMovement;
        
        protected override void Init(INESActionReader argument, KinematicCharacterMotor motor, Camera mainCamera)
        {
            _inputReader = argument;
            _motor = motor;
            _motor.CharacterController = this;
        }

        private void Start()
        {
            _currentMovementSpeed = walkSpeed;
            _lastLookDirection = Vector3.forward;
        }

        public void DisableMovement()
        {
            _disableMovement = true;
        }

        public void EnableMovement()
        {
            _disableMovement = false;
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
            // character direction is relative to the camera
            _moveInputVector = new Vector3(_rawMoveInput.x, 0, _rawMoveInput.y);

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
            if (_disableMovement)
            {
                return;
            }
            
            Quaternion targetRotation = Quaternion.LookRotation(_lastLookDirection);
            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * deltaTime);
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (_disableMovement)
            {
                currentVelocity = Vector3.zero;
                return;
            }
            
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

        public void AfterCharacterUpdate(float deltaTime)
        {
            _animator.SetBool("IsMoving", _motor.Velocity.magnitude > 0.01f);
        }
        public bool IsColliderValidForCollisions(Collider coll) { return true; }
        public void OnDiscreteCollisionDetected(Collider hitCollider) { }
        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
        public void PostGroundingUpdate(float deltaTime) { }
        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
    }
}
