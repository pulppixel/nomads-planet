using DigitalRubyShared;
using UnityEngine;

namespace NomadsPlanet
{
    public class MenuCharacterController : MonoBehaviour
    {
        public AudioSource sfxAudioSource;
        public AudioClip leftFoot;
        public AudioClip rightFoot;

        private const float SpeedSmoothTime = 0.1f;
        private const float WalkSpeed = 3f;
        private const float RunSpeed = 6f;
        private const float TurnSmoothTime = 0.1f;
        private const float JumpHeight = 2.5f;
        private const float Gravity = -9.81f;
        private const float DanceTriggerTime = 5f;

        private float _speed;
        private float _speedSmoothVelocity;
        private float _turnSmoothVelocity;
        private float _idleTime = 5f;
        private Vector3 _velocity;

        private CharacterController _controller;
        private Animator _animator;

        private bool _isMoving = false;
        private bool _isLeftFoot = true;
        private float _footstepCounter = 0f;
        private const float WalkStepInterval = 0.5f; // 걷는 발걸음 소리 간격
        private const float RunStepInterval = 0.3f;

        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int Dance = Animator.StringToHash("Dance");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Jump1 = Animator.StringToHash("Jump");

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            HandleJump();
            ApplyGravity();
            HandleMovementAndRun();
            HandleDance();
            HandleFootsteps();
            FinalizeMovement();
        }

        private void HandleMovementAndRun()
        {
            Vector3 direction = GetInputDirection();

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            bool isRunning = FingersJoystickScript.Instance.CurrentAmount.magnitude > .8f;
#else
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
#endif
            SetRunning(isRunning);

            if (direction.magnitude >= 0.1f)
            {
                MoveCharacter(direction);
                SetAnimationStates(true, false);
                ResetIdleTime();
            }
            else
            {
                SetAnimationStates(false, false);
                AccumulateIdleTime();
            }
        }

        private Vector3 GetInputDirection()
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            float horizontal = FingersJoystickScript.Instance.CurrentAmount.x;
            float vertical = FingersJoystickScript.Instance.CurrentAmount.y;
#else
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
#endif
            return new Vector3(horizontal, 0f, vertical).normalized;
        }

        private void MoveCharacter(Vector3 direction)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity,
                TurnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            _controller.Move(direction.normalized * _speed * Time.deltaTime);
        }

        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && _controller.isGrounded)
            {
                Jump();
                SetAnimationStates(false, true);
                ResetIdleTime();
            }
        }

        private void Jump()
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        private void SetRunning(bool isRunning)
        {
            _speed = isRunning ? RunSpeed : WalkSpeed;
            float targetSpeed = isRunning ? RunSpeed : WalkSpeed;
            _speed = Mathf.SmoothDamp(_speed, targetSpeed, ref _speedSmoothVelocity, SpeedSmoothTime);
            _animator.SetBool(Run, isRunning);
        }

        private void HandleDance()
        {
            _animator.SetBool(Dance, _idleTime >= DanceTriggerTime);
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y += Gravity * Time.deltaTime;
        }

        private void FinalizeMovement()
        {
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void ResetIdleTime()
        {
            _idleTime = 0f;
        }

        private void AccumulateIdleTime()
        {
            _idleTime += Time.deltaTime;
        }

        private void HandleFootsteps()
        {
            bool isWalking = _animator.GetBool(Walk);
            bool isRunning = _animator.GetBool(Run);
            bool isJumping = _animator.GetCurrentAnimatorStateInfo(0).IsName("Jump");
            bool currentlyMoving = (isWalking || isRunning) && !isJumping;

            if (currentlyMoving)
            {
                if (!_isMoving)
                {
                    PlayFootstepSound();
                    _footstepCounter = 0f;
                    _isMoving = true;
                }

                float stepInterval = isRunning ? RunStepInterval : WalkStepInterval;
                _footstepCounter += Time.deltaTime;

                if (_footstepCounter >= stepInterval)
                {
                    PlayFootstepSound();
                    _footstepCounter = 0f;
                }
            }
            else
            {
                _footstepCounter = 0f;
                _isMoving = false;
            }
        }

        private void PlayFootstepSound()
        {
            AudioClip clip = _isLeftFoot ? leftFoot : rightFoot;
            sfxAudioSource.PlayOneShot(clip);
            _isLeftFoot = !_isLeftFoot;
        }

        private void SetAnimationStates(bool walk, bool jump)
        {
            _animator.SetBool(Walk, walk);

            if (jump)
            {
                _animator.SetTrigger(Jump1);
            }
        }
    }
}