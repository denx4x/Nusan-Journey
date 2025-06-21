using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStateHandler))]
public class PlayerMovement : MonoBehaviour {
    [Header("Component References")]
    [SerializeField] private Player player;
    [SerializeField] private Animator animator;
    private PlayerStateHandler stateHandler;

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float pushSpeed = 1.5f;

    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Physics")]
    [SerializeField] private float gravityValue = -9.81f;

    private CharacterController controller;
    private Vector2 movementInput;
    private Vector3 playerVelocity;
    private bool isRunning;

    private void Awake() {
        controller = GetComponent<CharacterController>();
        stateHandler = GetComponent<PlayerStateHandler>();
    }

    #region Input Handling
    private void Start() {
        if (player == null || player.PlayerControls == null) { Debug.LogError("Player atau PlayerControls belum di-assign!"); return; }
        player.PlayerControls.Character.Movement.performed += OnMovementPerformed;
        player.PlayerControls.Character.Movement.canceled += OnMovementCanceled;
        player.PlayerControls.Character.Run.performed += OnRunPerformed;
        player.PlayerControls.Character.Run.canceled += OnRunCanceled;
    }

    private void OnDestroy() {
        if (player != null && player.PlayerControls != null) {
            player.PlayerControls.Character.Movement.performed -= OnMovementPerformed;
            player.PlayerControls.Character.Movement.canceled -= OnMovementCanceled;
            player.PlayerControls.Character.Run.performed -= OnRunPerformed;
            player.PlayerControls.Character.Run.canceled -= OnRunCanceled;
        }
    }

    private void OnMovementPerformed(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();
    private void OnMovementCanceled(InputAction.CallbackContext ctx) => movementInput = Vector2.zero;
    private void OnRunPerformed(InputAction.CallbackContext ctx) => isRunning = ctx.ReadValueAsButton();
    private void OnRunCanceled(InputAction.CallbackContext ctx) => isRunning = false;
    #endregion

    private void FixedUpdate() {
        HandleGravity();
        HandleMovement();
        UpdateAnimatorState();
    }

    private void HandleGravity() {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0) { playerVelocity.y = -2f; }
        playerVelocity.y += gravityValue * Time.deltaTime;
    }

    private void HandleMovement() {
        Transform cameraTransform = Camera.main.transform;
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        Vector3 moveDirection = forward * movementInput.y + right * movementInput.x;

        if (moveDirection.magnitude > 0.1f) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float currentSpeed = stateHandler.IsPushing ? pushSpeed : (isRunning ? runSpeed : walkSpeed);

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void UpdateAnimatorState() {
        bool moving = movementInput.magnitude > 0.1f;
        bool isPushingState = stateHandler.IsPushing;

        animator.SetBool("IsMovingNusan", moving && !isPushingState);
        animator.SetBool("IsRunningNusan", moving && isRunning && !isPushingState);
        animator.SetBool("IsPushingNusan", moving && isPushingState);
    }

}