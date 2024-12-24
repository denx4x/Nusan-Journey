using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;

    private Animator animator;
    [SerializeField] private string inputNameParameter;
    private CharacterController characterController;


    [SerializeField] private string inputNameHorizontal;
    [SerializeField] private string inputNameVertical;

    private float inputHorizontal;
    private float inputVertical;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis(inputNameHorizontal);
        float verticalInput = Input.GetAxis(inputNameVertical);

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        movementDirection.Normalize();



        Vector3 velocity = movementDirection * magnitude;

        characterController.Move(velocity * Time.deltaTime);

        if (movementDirection != Vector3.zero)
        {
            animator.SetBool(inputNameParameter, true);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool(inputNameParameter, false);
        }
    }
}
