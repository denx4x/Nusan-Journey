using UnityEngine;

public class PuzzleTrigger : MonoBehaviour, IInteractPlayer {
    [Header("Referensi Puzzle")]
    [Tooltip("Masukkan PuzzleManager yang sesuai untuk puzzle ini")]
    [SerializeField] private GridPuzzleManager puzzleManager;

    public void Interact() {
        // Pastikan puzzle manager ada dan PuzzleModeController ada
        if (puzzleManager != null && PuzzleModeController.Instance != null) {
            Debug.Log("Interaksi dengan Puzzle Trigger, memulai puzzle...");
            // Panggil fungsi di controller untuk memulai puzzle
            PuzzleModeController.Instance.StartPuzzle(puzzleManager);

            // Opsional: Nonaktifkan collider ini setelah puzzle dimulai agar tidak bisa diinteract lagi
            GetComponent<Collider>().enabled = false;
        } else {
            Debug.LogError("Puzzle Manager atau PuzzleModeController tidak ditemukan!");
        }
    }
}