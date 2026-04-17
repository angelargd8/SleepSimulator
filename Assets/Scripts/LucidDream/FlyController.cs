using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FlyController : MonoBehaviour
    {
        [Header("Player")]
        public float MoveSpeed = 4.0f;
        public float SprintSpeed = 6.0f;
        public float RotationSpeed = 1.0f;
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        [Space(10)]
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        [Header("Flying")]
        public float FlySpeed = 5.0f;
        public float FlyUpSpeed = 2.0f;
        public float FlyDownSpeed = 2.0f;
        public float DoubleTapSpaceTime = 0.25f;
        public float DoubleTapWTime = 0.25f;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // vuelo / sprint
        private bool _isFlying;
        private bool _doubleTapSprint;
        private float _lastSpacePressedTime = -10f;
        private float _lastWPressedTime = -10f;

        // tracking propio del espacio
        private bool _spaceHeld;
        private bool _spaceJustPressed;

        // tracking propio de W
        private bool _wWasDown;        // W estaba presionada el frame anterior

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies.");
#endif
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            // Leer espacio una sola vez por frame
            _spaceHeld = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
            _spaceJustPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

            // W: detectar flanco de subida 
            bool wDown = Keyboard.current != null && Keyboard.current.wKey.isPressed;
            bool wJustPressed = wDown && !_wWasDown;
            _wWasDown = wDown;

            GroundedCheck();
            HandleDoubleTapFlight();
            HandleDoubleTapSprint(wJustPressed);
            JumpAndGravity();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                CinemachineCameraTarget.transform.localRotation =
                    Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void HandleDoubleTapFlight()
        {
            // Usar _spaceJustPressed en lugar de _input.jump
            // para no consumirlo antes de que Move() lo lea
            if (!_spaceJustPressed) return;

            if (Time.time - _lastSpacePressedTime <= DoubleTapSpaceTime)
            {
                _isFlying = !_isFlying;
                _verticalVelocity = 0f;
                // Consumir el input de StarterAssets para que JumpAndGravity no salte
                _input.jump = false;
                _lastSpacePressedTime = -10f;
                return;
            }

            _lastSpacePressedTime = Time.time;
        }

        private void HandleDoubleTapSprint(bool wJustPressed)
        {
            if (wJustPressed)
            {
                if (Time.time - _lastWPressedTime <= DoubleTapWTime)
                    _doubleTapSprint = true;

                _lastWPressedTime = Time.time;
            }

            // Soltar W cancela el sprint
            if (!Keyboard.current.wKey.isPressed)
                _doubleTapSprint = false;
        }

        private void Move()
        {
            float targetSpeed = _isFlying
                ? FlySpeed
                : (_doubleTapSprint ? SprintSpeed : MoveSpeed);

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(
                _controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;

            Vector3 move = inputDirection.normalized * (_speed * Time.deltaTime);

            if (_isFlying)
            {
                float flyVertical = 0f;

                // Subir: espacio sostenido (leído de la tecla directamente)
                if (_spaceHeld)
                    flyVertical += FlyUpSpeed;

                // Bajar: shift (sprint input de StarterAssets)
                if (_input.sprint)
                    flyVertical -= FlyDownSpeed;

                move += new Vector3(0f, flyVertical * Time.deltaTime, 0f);
            }
            else
            {
                move += new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime;
            }

            _controller.Move(move);
        }

        private void JumpAndGravity()
        {
            if (_isFlying)
            {
                _verticalVelocity = 0f;
                _fallTimeoutDelta = FallTimeout;
                _jumpTimeoutDelta = JumpTimeout;
                return;
            }

            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                if (_jumpTimeoutDelta >= 0.0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += Gravity * Time.deltaTime;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color tGreen = new Color(0f, 1f, 0f, 0.35f);
            Color tRed = new Color(1f, 0f, 0f, 0.35f);
            Gizmos.color = Grounded ? tGreen : tRed;
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }
    }
}