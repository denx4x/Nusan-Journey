using UnityEngine;

public class ObjectTarget : MonoBehaviour {
    [Header("Puzzle Settings")]
    [Tooltip("Batu atau bola spesifik yang dibutuhkan untuk menyelesaikan puzzle ini.")]
    [SerializeField] private GrabbableObject requiredObject;

    [Tooltip("Platform yang akan bergerak setelah puzzle selesai.")]
    [SerializeField] private MovingPlatform platformToActivate;

    [Header("Snapping")]
    [Tooltip("Titik di mana batu akan menempel. Kosongkan jika ingin menempel di pusat target.")]
    [SerializeField] private Transform snapPoint;

    private bool isSolved = false;

    private void OnTriggerEnter(Collider other) {
        if (isSolved) return;

        if (other.TryGetComponent<GrabbableObject>(out GrabbableObject grabbable)) {
            if (grabbable == requiredObject) {
                Debug.Log("Target reached by the correct Grabbable object!");
                SolvePuzzle(grabbable);
            }
        }
    }

    private void SolvePuzzle(GrabbableObject solvedObject) {
        isSolved = true;

        // --- INI BAGIAN YANG DIPERBAIKI ---
        // Cari komponen PlayerStateHandler pada parent dari batu (yaitu Player).
        PlayerStateHandler playerState = solvedObject.GetComponentInParent<PlayerStateHandler>();
        if (playerState != null) {
            // Reset status player melalui State Handler agar bisa lari lagi!
            playerState.IsPushing = false;
            Debug.Log("Player state 'IsPushing' direset ke false via State Handler.");
        }
        // ------------------------------------

        // Matikan fisika pada batu/bola
        Rigidbody rb = solvedObject.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
        }

        // Hancurkan skrip GrabbableObject agar tidak bisa diinteraksi lagi
        Destroy(solvedObject);

        // Posisikan dan tempelkan batu/bola ke titik yang ditentukan
        Transform objectTransform = solvedObject.transform;
        Transform targetParent = (snapPoint != null) ? snapPoint : transform;
        objectTransform.SetParent(targetParent);
        objectTransform.localPosition = Vector3.zero;
        objectTransform.localRotation = Quaternion.identity;

        // Aktifkan platform untuk bergerak
        if (platformToActivate != null) {
            platformToActivate.ActivatePlatform();
        }
    }
}