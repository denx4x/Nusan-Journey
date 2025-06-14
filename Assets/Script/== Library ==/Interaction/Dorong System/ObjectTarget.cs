using UnityEngine;

public class ObjectTarget : MonoBehaviour {
    [Header("Puzzle Settings")]
    [Tooltip("Batu atau bola spesifik yang dibutuhkan untuk menyelesaikan puzzle ini.")]
    // DIUBAH: Sekarang kita mencari GrabbableObject, bukan PushableObject
    [SerializeField] private GrabbableObject requiredObject;

    [Tooltip("Platform yang akan bergerak setelah puzzle selesai.")]
    [SerializeField] private MovingPlatform platformToActivate;

    [Header("Snapping")]
    [Tooltip("Titik di mana batu akan menempel. Kosongkan jika ingin menempel di pusat target.")]
    [SerializeField] private Transform snapPoint;

    private bool isSolved = false;

    private void OnTriggerEnter(Collider other) {
        // Jika puzzle sudah selesai, jangan lakukan apa-apa
        if (isSolved) {
            return;
        }

        // DIUBAH: Cek apakah objek yang masuk memiliki skrip GrabbableObject
        if (other.TryGetComponent<GrabbableObject>(out GrabbableObject grabbable)) {
            // Cek apakah ini adalah objek yang benar yang kita tunggu
            if (grabbable == requiredObject) {
                Debug.Log("Target reached by the correct Grabbable object!");
                SolvePuzzle(grabbable);
            }
        }
    }

    private void SolvePuzzle(GrabbableObject solvedObject) {
        isSolved = true;

        // 1. Matikan fisika pada batu/bola agar berhenti bergerak dan tidak bisa di-grab lagi
        Rigidbody rb = solvedObject.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
        }

        // Hancurkan skrip GrabbableObject agar tidak bisa diinteraksi lagi setelah masuk target
        Destroy(solvedObject);

        // 2. Posisikan dan tempelkan batu/bola ke titik yang ditentukan
        Transform objectTransform = solvedObject.transform;
        Transform targetParent = (snapPoint != null) ? snapPoint : transform;
        objectTransform.SetParent(targetParent);
        objectTransform.localPosition = Vector3.zero;
        objectTransform.localRotation = Quaternion.identity;

        // 3. Aktifkan platform untuk bergerak
        if (platformToActivate != null) {
            platformToActivate.ActivatePlatform();
        }
    }
}