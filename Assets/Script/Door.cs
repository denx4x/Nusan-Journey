using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public float smooth = 2f; // Adjust this to control rotation speed
    public bool autoClose = true; // Set to true if you want the door to auto close

    private Transform player;
    private float defaultYRotation;
    private float targetYRotation;
    private float timer = 0f;

    public Transform pivot; // Assign this in the Inspector

    private bool isOpen = false;

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
            ToggleDoor(player.position);
        }
    }

    public void ToggleDoor(Vector3 pos)
    {
        if (pivot == null) return;

        isOpen = !isOpen;

        if (isOpen)
        {
            Vector3 dir = (pos - transform.position).normalized;
            float dot = Vector3.Dot(transform.right, dir);
            targetYRotation = Mathf.Sign(dot) * 180f; // Rotate 180 degrees
            timer = 5f; // Set the timer for auto close
        }
        else
        {
            targetYRotation = 0f;
        }
    }

    public void Interact()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned.");
            return;
        }

        ToggleDoor(player.position);
    }

    public string GetDescription()
    {
        return isOpen ? "Press E to close the door" : "Press E to open the door";
    }
}
