using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [Header("Component References")]
    [SerializeField] private Player player;
    [SerializeField] private Animator animator;

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float pushSpeed = 1.5f; // Kecepatan saat mendorong/membawa objek

    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Physics")]
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float pushPower = 2.0f; // Kekuatan untuk mendorong objek fisika biasa

    // Properti ini akan diubah oleh skrip lain (GrabbableObject)
    // untuk menandakan player sedang membawa/mendorong objek.
    public bool IsPushing { get; set; }

    // Variabel internal
    private CharacterController controller;
    private Vector2 movementInput;
    private Vector3 playerVelocity;
    private bool isRunning;

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    private void Start() {
        // Pastikan player dan controls tidak null sebelum berlangganan
        if (player == null || player.PlayerControls == null) {
            Debug.LogError("Player atau PlayerControls belum di-assign atau diinisialisasi!");
            return;
        }

        // Berlangganan (subscribe) ke event dari Input System
        player.PlayerControls.Character.Movement.performed += OnMovementPerformed;
        player.PlayerControls.Character.Movement.canceled += OnMovementCanceled;
        player.PlayerControls.Character.Run.performed += OnRunPerformed;
        player.PlayerControls.Character.Run.canceled += OnRunCanceled;
    }

    private void OnDestroy() {
        // Berhenti berlangganan (unsubscribe) untuk mencegah error
        if (player != null && player.PlayerControls != null) {
            player.PlayerControls.Character.Movement.performed -= OnMovementPerformed;
            player.PlayerControls.Character.Movement.canceled -= OnMovementCanceled;
            player.PlayerControls.Character.Run.performed -= OnRunPerformed;
            player.PlayerControls.Character.Run.canceled -= OnRunCanceled;
        }
    }

    // --- FUNGSI-FUNGSI INPUT ---
    private void OnMovementPerformed(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();
    private void OnMovementCanceled(InputAction.CallbackContext ctx) => movementInput = Vector2.zero;
    private void OnRunPerformed(InputAction.CallbackContext ctx) => isRunning = ctx.ReadValueAsButton();
    private void OnRunCanceled(InputAction.CallbackContext ctx) => isRunning = false;
    // -------------------------

    private void Update() {
        HandleGravity();
        HandleMovement();
        UpdateAnimatorState();
    }

    private void HandleGravity() {
        // Cek jika karakter menapak di tanah
        bool isGrounded = controller.isGrounded;

        // Jika di tanah dan sedang jatuh (velocity.y < 0), reset kecepatan jatuh agar tetap menempel
        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = -2f;
        }

        // Terapkan gravitasi secara terus-menerus
        playerVelocity.y += gravityValue * Time.deltaTime;
    }

    private void HandleMovement() {
        // Ubah input 2D menjadi vektor gerakan 3D
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);

        // Rotasi player agar menghadap arah gerakan
        if (moveDirection.magnitude > 0.1f) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Tentukan kecepatan berdasarkan kondisi player
        float currentSpeed;
        if (IsPushing) {
            // Jika sedang memegang/mendorong objek yang bisa di-grab
            currentSpeed = pushSpeed;
        } else {
            // Jika sedang bergerak normal
            currentSpeed = isRunning ? runSpeed : walkSpeed;
        }

        // Gerakkan player secara horizontal
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Terapkan gerakan vertikal (gravitasi)
        controller.Move(playerVelocity * Time.deltaTime);
    }

    /*private void UpdateAnimatorState() {
        // Cek apakah player sedang bergerak (input tidak nol)
        bool moving = movementInput.magnitude >= 0.1f;

        // Atur parameter di Animator
        animator.SetBool("IsMovingNusan", moving && !IsPushing); // Hanya jalan jika bergerak dan tidak mendorong
        animator.SetBool("IsRunningNusan", moving && isRunning && !IsPushing); // Hanya lari jika bergerak, lari, dan tidak mendorong
        animator.SetBool("IsPushingNusan", moving && IsPushing); // Atur animasi mendorong (Anda perlu menambahkannya di Animator)
    }*/

    // VERSI BARU (MENGABAIKAN PENGARUH GRABBING)
    private void UpdateAnimatorState() {
        bool moving = movementInput.magnitude >= 0.1f;

        // Mengembalikan ke logika animasi dasar.
        // Animator sekarang hanya peduli pada input gerakan, bukan status IsPushing.
        animator.SetBool("IsMovingNusan", moving);
        animator.SetBool("IsRunningNusan", moving && isRunning);
    }

    // Fungsi ini untuk mendorong objek fisika yang tidak bisa di-grab (misal: kotak kecil, kaleng)
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        // Cek apakah objek yang ditabrak punya Rigidbody
        Rigidbody body = hit.collider.attachedRigidbody;

        // Kondisi untuk tidak mendorong: tidak ada rigidbody, atau rigidbody-nya kinematic (statis)
        if (body == null || body.isKinematic) {
            return;
        }

        // Jangan mendorong objek jika kita berada di atasnya (misal, saat jatuh)
        if (hit.moveDirection.y < -0.3f) {
            return;
        }

        // Hitung arah dorongan (hanya sumbu X dan Z)
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Beri kecepatan pada rigidbody objek yang didorong
        body.linearVelocity = pushDir * pushPower;
    }
}