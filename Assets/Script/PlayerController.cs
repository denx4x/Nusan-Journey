using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public Animator animator;
    [SerializeField] private string inputNameParameter;
    public CharacterController characterController;
    public int playerIndex; // Menentukan pemain (1 atau 2)

    private PlayerControls controls;
    private Vector2 moveInput;
    private string animMove = "isMoving";
    private Gamepad gamepad; // Deklarasi gamepad

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Bind input actions berdasarkan playerIndex
        if (playerIndex == 1)
        {
            gamepad = Gamepad.all.FirstOrDefault(); // Ambil gamepad pertama
            if (gamepad != null)
            {
                controls.Player1.Move.AddBinding("<Gamepad>/leftStick").WithInteraction("press").WithProcessor("normalize");
            }
            controls.Player1.Move.performed += ctx =>
            {
                moveInput = ctx.ReadValue<Vector2>();
                Debug.Log("Player 1 Move Input: " + moveInput);
            };
            controls.Player1.Move.canceled += ctx => moveInput = Vector2.zero;
        }
        else if (playerIndex == 2)
        {
            gamepad = Gamepad.all.ElementAtOrDefault(1); // Ambil gamepad kedua jika ada
            if (gamepad != null)
            {
                controls.Player2.Move.AddBinding("<Gamepad>/leftStick").WithInteraction("press").WithProcessor("normalize");
            }
            controls.Player2.Move.performed += ctx =>
            {
                moveInput = ctx.ReadValue<Vector2>();
                Debug.Log("Player 2 Move Input: " + moveInput);
            };
            controls.Player2.Move.canceled += ctx => moveInput = Vector2.zero;
        }
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        characterController.Move(move * speed * Time.deltaTime);

        if (move != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            animator.SetBool(inputNameParameter, true);
        }
        else
        {
            animator.SetBool(inputNameParameter, false);
        }
    }
}
