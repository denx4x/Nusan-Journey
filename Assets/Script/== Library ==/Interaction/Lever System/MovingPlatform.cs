using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour {
    public enum PlatformMode { Manual, Automatic }
    public enum LoopType { Loop, PingPong }

    [Header("Platform Mode")]
    [SerializeField] private PlatformMode mode = PlatformMode.Manual;
    [SerializeField] private LoopType loopType = LoopType.PingPong;

    [Header("Movement Points")]
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float pauseDuration = 1f;

    private Rigidbody rb;
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private bool isPaused = false;
    private bool movingForward = true;
    private List<CharacterController> passengers = new List<CharacterController>();

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Start() {
        if (waypoints.Count < 2) {
            Debug.LogError("MovingPlatform: Butuh minimal 2 waypoint!", this);
            enabled = false;
            return;
        }

        // Atur posisi awal dan siapkan target pertama
        transform.position = waypoints[0].position;
        currentWaypointIndex = 1; // DIUBAH: Langsung siapkan target pertama untuk mode Manual

        if (mode == PlatformMode.Automatic) {
            isMoving = true;
        }
    }

    private void FixedUpdate() {
        if (!isMoving || isPaused || waypoints.Count == 0) return;

        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        Vector3 previousPosition = rb.position;
        Vector3 newPosition = Vector3.MoveTowards(previousPosition, targetPosition, moveSpeed * Time.fixedDeltaTime);
        Vector3 moveDelta = newPosition - previousPosition;

        rb.MovePosition(newPosition);

        if (passengers.Count > 0) {
            foreach (var passenger in passengers) {
                passenger.Move(moveDelta);
            }
        }

        if (transform.position == targetPosition) {
            HandleArrival();
        }
    }

    private void HandleArrival() {
        // Logika untuk mempersiapkan titik selanjutnya sekarang sama untuk kedua mode
        UpdateTargetWaypoint();

        if (mode == PlatformMode.Manual) {
            isMoving = false; // Berhenti jika manual
        } else // mode == PlatformMode.Automatic
          {
            isMoving = false; // Berhenti sementara untuk jeda
            StartCoroutine(WaitAndMoveOn());
        }
    }

    private IEnumerator WaitAndMoveOn() {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
        isMoving = true; // Lanjutkan pergerakan setelah jeda
    }

    private void UpdateTargetWaypoint() {
        // ... (Logika PingPong dan Loop di sini tidak berubah) ...
        if (loopType == LoopType.Loop) {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        } else // loopType == LoopType.PingPong
          {
            if (movingForward) {
                if (currentWaypointIndex >= waypoints.Count - 1) {
                    movingForward = false;
                    currentWaypointIndex--;
                } else {
                    currentWaypointIndex++;
                }
            } else // moving backward
              {
                if (currentWaypointIndex <= 0) {
                    movingForward = true;
                    currentWaypointIndex++;
                } else {
                    currentWaypointIndex--;
                }
            }
        }
    }

    public void ActivatePlatform() {
        if (mode == PlatformMode.Automatic || isMoving) return;
        isMoving = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            CharacterController passenger = other.GetComponent<CharacterController>();
            if (passenger != null && !passengers.Contains(passenger)) {
                passengers.Add(passenger);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            CharacterController passenger = other.GetComponent<CharacterController>();
            if (passenger != null && passengers.Contains(passenger)) {
                passengers.Remove(passenger);
            }
        }
    }
}