using UnityEngine;
using Cinemachine; // Jika Anda menggunakan Cinemachine untuk kamera

public class PuzzleModeController : MonoBehaviour {
    public static PuzzleModeController Instance { get; private set; }

    [Header("Referensi Komponen Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CharacterController characterController;

    [Header("Referensi Kamera")]
    [Tooltip("Kamera utama yang mengikuti player")]
    [SerializeField] private GameObject playerFollowCamera;
    [Tooltip("Kamera yang fokus pada puzzle")]
    [SerializeField] private GameObject puzzleCamera;

    private GridPuzzleManager currentPuzzle;

    private void Awake() {
        // Singleton Pattern
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void StartPuzzle(GridPuzzleManager puzzleToStart) {
        currentPuzzle = puzzleToStart;
        Debug.Log($"Memulai Puzzle: {puzzleToStart.name}");

        // 1. Nonaktifkan kontrol player normal
        playerMovement.enabled = false;
        characterController.enabled = false;

        // 2. Aktifkan puzzle dan berlangganan event selesai
        currentPuzzle.enabled = true;
        currentPuzzle.OnPuzzleCompleted += EndPuzzle;

        // 3. Ganti kamera
        playerFollowCamera.SetActive(false);
        puzzleCamera.SetActive(true);
    }

    private void EndPuzzle() {
        if (currentPuzzle == null) return;
        Debug.Log($"Menyelesaikan Puzzle: {currentPuzzle.name}");

        // 1. Berhenti berlangganan event untuk mencegah memory leak
        currentPuzzle.OnPuzzleCompleted -= EndPuzzle;

        // 2. Nonaktifkan puzzle
        currentPuzzle.enabled = false;
        // Opsional: nonaktifkan trigger agar tidak bisa diulang
        // FindObjectOfType<PuzzleTrigger>()?.gameObject.SetActive(false); // Cari cara yang lebih spesifik jika ada banyak puzzle

        // 3. Aktifkan kembali kontrol player
        playerMovement.enabled = true;
        characterController.enabled = true;

        // 4. Kembalikan kamera
        puzzleCamera.SetActive(false);
        playerFollowCamera.SetActive(true);

        currentPuzzle = null;
    }
}