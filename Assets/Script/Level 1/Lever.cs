using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    public float smooth = 2f; // Adjust this to control rotation speed
    public bool autoClose = true; // Set to true if you want the door to auto close

    private Transform player;
    private float defaultYRotation;
    private float targetYRotation;
    private float timer = 0f;

    public Transform pivot; // Assign this in the Inspector

    private bool isOpen = false;

    public GameObject lever_path1;
    private LeverPath1 leverPath1Script;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found. Make sure there is a GameObject with the 'Player' tag in the scene.");
        }

        if (pivot == null)
        {
            Debug.LogError("Pivot not assigned. Please assign a pivot Transform in the Inspector.");
        }
        else
        {
            defaultYRotation = pivot.eulerAngles.y;
        }

        if (lever_path1 != null)
        {
            leverPath1Script = lever_path1.GetComponent<LeverPath1>();
            if (leverPath1Script == null)
            {
                Debug.LogError("LeverPath1Script not found on lever_path1. Please add the LeverPath1Script to lever_path1.");
            }
        }
        else
        {
            Debug.LogError("lever_path1 not assigned. Please assign the lever_path1 GameObject in the Inspector.");
        }
    }

    void Update()
    {
        if (pivot == null) return;

        // Rotate the pivot smoothly
        pivot.rotation = Quaternion.Lerp(pivot.rotation, Quaternion.Euler(0f, defaultYRotation + targetYRotation, 0f), smooth * Time.deltaTime);

        // Countdown timer for auto close
        timer -= Time.deltaTime;

        // Auto close the door if the timer expires
        if (timer <= 0f && isOpen && autoClose)
        {
            ToggleLever(player.position);
        }
    }

    public void ToggleLever(Vector3 pos)
    {
        if (pivot == null) return;

        isOpen = !isOpen;

        if (isOpen)
        {
            Vector3 dir = (pos - transform.position).normalized;
            float dot = Vector3.Dot(transform.right, dir);
            targetYRotation = Mathf.Sign(dot) * 180f; // Rotate 180 degrees
            timer = 5f; // Set the timer for auto close

            if (leverPath1Script != null)
            {
                leverPath1Script.isMovingLever = true;
            }
        }
        else
        {
            targetYRotation = 0f;

            if (leverPath1Script != null)
            {
                leverPath1Script.isMovingLever = false;
            }
        }
    }

    public void Interact()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned.");
            return;
        }

        ToggleLever(player.position);
    }

    public string GetDescription()
    {
        return isOpen ? "Press E to pull the lever counterclockwise" : "Press E to pull the lever clockwise";
    }
}
