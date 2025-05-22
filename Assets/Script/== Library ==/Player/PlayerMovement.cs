using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private Animator animator;

    private Vector2 movementInput;
    private bool isRunning;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 5f;
    private CharacterController controller;

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    private void Start() {      

        // Mengaitkan callback menggunakan instance kontrol dari komponen Player.
        player.PlayerControls.Character.Movement.performed += OnMovementPerformed;
        player.PlayerControls.Character.Movement.canceled += OnMovementCanceled;
        player.PlayerControls.Character.Run.performed += OnRunPerformed;
        player.PlayerControls.Character.Run.canceled += OnRunCanceled;
    }

    private void OnDestroy() {
        // Unsubscribe callback untuk menghindari memory leak
        if (player != null && player.PlayerControls != null) {
            player.PlayerControls.Character.Movement.performed -= OnMovementPerformed;
            player.PlayerControls.Character.Movement.canceled -= OnMovementCanceled;
            player.PlayerControls.Character.Run.performed -= OnRunPerformed;
            player.PlayerControls.Character.Run.canceled -= OnRunCanceled;
        }
    }

    // Callback untuk aksi Movement
    private void OnMovementPerformed(InputAction.CallbackContext ctx) {
        movementInput = ctx.ReadValue<Vector2>();
        Debug.Log("OnMovement: " + movementInput);
    }

    private void OnMovementCanceled(InputAction.CallbackContext ctx) {
        movementInput = Vector2.zero;
        Debug.Log("Movement canceled");
    }

    // Callback untuk aksi Run
    private void OnRunPerformed(InputAction.CallbackContext ctx) {
        isRunning = ctx.ReadValueAsButton();
        Debug.Log("OnRun: " + isRunning);
    }

    private void OnRunCanceled(InputAction.CallbackContext ctx) {
        isRunning = false;
        Debug.Log("Run canceled");
    }

    private void Update() {
        HandleMovement();
        UpdateAnimatorState();
    }

    private void HandleMovement() {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);

        if (moveDirection.magnitude > 0.1f) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        float speed = isRunning ? runSpeed : walkSpeed;
        controller.Move(moveDirection * speed * Time.deltaTime);
    }
    
    private void UpdateAnimatorState() {
        if (animator == null) return;

        // Cek apakah ada input pergerakan.
        bool moving = movementInput.magnitude >= 0.1f;
        
        animator.SetBool("IsMovingNusan", moving);
        animator.SetBool("IsRunningNusan", moving && isRunning);
    }
}
