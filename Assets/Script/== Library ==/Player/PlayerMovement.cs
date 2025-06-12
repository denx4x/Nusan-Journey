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

    // --- Variabel Baru untuk Gravitasi ---
    // BARU: Vektor untuk menyimpan kecepatan vertikal (jatuh)
    private Vector3 playerVelocity;
    // BARU: Nilai gravitasi. -9.81f adalah nilai realistis, bisa disesuaikan.
    private float gravityValue = -9.81f;

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    private void Start() {
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

    // --- Fungsi Input (Tidak ada perubahan di sini) ---
    private void OnMovementPerformed(InputAction.CallbackContext ctx) {
        movementInput = ctx.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext ctx) {
        movementInput = Vector2.zero;
    }

    private void OnRunPerformed(InputAction.CallbackContext ctx) {
        isRunning = ctx.ReadValueAsButton();
    }

    private void OnRunCanceled(InputAction.CallbackContext ctx) {
        isRunning = false;
    }
    // ---------------------------------------------------

    private void Update() {
        HandleGravity(); // BARU: Panggil fungsi untuk handle gravitasi
        HandleMovement();
        UpdateAnimatorState();
    }

    // BARU: Fungsi terpisah untuk mengelola gravitasi
    private void HandleGravity() {
        // Cek apakah karakter sedang menyentuh tanah.
        bool isGrounded = controller.isGrounded;

        // Jika di tanah dan sedang jatuh (velocity.y < 0), reset kecepatan jatuh.
        if (isGrounded && playerVelocity.y < 0) {
            // Kita set ke nilai kecil, bukan 0, agar karakter tetap menempel di tanah.
            playerVelocity.y = -3f;
        }

        // Terapkan gravitasi secara terus-menerus.
        // Kecepatan akan bertambah seiring waktu selama karakter di udara.
        playerVelocity.y += gravityValue * Time.deltaTime;
    }

    private void HandleMovement() {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);

        if (moveDirection.magnitude > 0.1f) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float speed = isRunning ? runSpeed : walkSpeed;

        // DIUBAH: Terapkan gerakan horizontal
        controller.Move(moveDirection * speed * Time.deltaTime);

        // BARU: Terapkan gerakan vertikal (gravitasi) secara terpisah
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void UpdateAnimatorState() {
        bool moving = movementInput.magnitude >= 0.1f;
        animator.SetBool("IsMovingNusan", moving);
        animator.SetBool("IsRunningNusan", moving && isRunning);
    }
}