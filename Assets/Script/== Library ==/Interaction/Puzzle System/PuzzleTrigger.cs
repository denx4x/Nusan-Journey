using UnityEngine;

public class PuzzleTrigger : MonoBehaviour, IInteractPlayer {
    [Header("Referensi Puzzle")]
    [Tooltip("Masukkan PuzzleManager yang sesuai untuk puzzle ini")]
    [SerializeField] private GridPuzzleManager puzzleManager;

    private bool isPermanentlySolved = false; // Variabel untuk mengingat status

    private void Start() {
        if (puzzleManager == null) {
            Debug.LogError($"PuzzleTrigger pada {gameObject.name} tidak memiliki referensi PuzzleManager!");
            return;
        }
        // Berlangganan event. Saat puzzle selesai, panggil HandlePuzzleSolved.
        puzzleManager.OnPuzzleCompleted += HandlePuzzleSolved;
    }

    private void OnDestroy() {
        // Selalu berhenti berlangganan saat objek hancur untuk mencegah error
        if (puzzleManager != null) {
            puzzleManager.OnPuzzleCompleted -= HandlePuzzleSolved;
        }
    }

    public void Interact() {
        // Cek apakah puzzle sudah permanen selesai
        if (isPermanentlySolved) {
            Debug.Log("Puzzle ini sudah selesai dan tidak bisa dimainkan lagi.");
            // Di sini Anda bisa memutar suara "sudah selesai" atau menampilkan pesan UI
            return;
        }

        // Jika belum selesai, mulai puzzle seperti biasa
        if (puzzleManager != null && PuzzleModeController.Instance != null) {
            Debug.Log("Interaksi dengan Puzzle Trigger, memulai puzzle...");
            PuzzleModeController.Instance.StartPuzzle(puzzleManager);
        }
    }

    // Fungsi ini akan dipanggil secara otomatis saat puzzle selesai
    private void HandlePuzzleSolved() {
        Debug.Log($"PuzzleTrigger di {gameObject.name} menerima sinyal puzzle selesai.");
        isPermanentlySolved = true;

        // Nonaktifkan collider trigger ini agar tidak bisa di-interact lagi selamanya
        Collider col = GetComponent<Collider>();
        if (col != null) {
            col.enabled = false;
        }
    }
}