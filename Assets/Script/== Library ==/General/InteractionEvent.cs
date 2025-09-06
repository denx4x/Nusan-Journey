using UnityEngine;
using UnityEngine.Events; // Wajib ada untuk menggunakan UnityEvent

/// <summary>
/// Menjalankan sebuah UnityEvent ketika pemain berada di dalam trigger
/// dan menekan tombol interaksi ('E').
/// </summary>
public class InteractionEvent : MonoBehaviour {
    [Header("Pengaturan Interaksi")]
    [Tooltip("Event yang akan dijalankan ketika pemain berinteraksi.")]
    public UnityEvent onInteract;

    [Tooltip("Jika dicentang, event ini hanya bisa dijalankan satu kali.")]
    [SerializeField] private bool interactOnce = true;

    // Variabel privat untuk melacak status
    private bool isPlayerInRange = false;
    private bool hasInteracted = false;

    // Fungsi ini akan terpanggil ketika sebuah collider lain MASUK ke trigger
    private void OnTriggerEnter(Collider other) {
        // Periksa apakah yang masuk adalah GameObject dengan tag "Player"
        if (other.CompareTag("Player")) {
            isPlayerInRange = true;
            // (Opsional) Beri feedback ke pemain bahwa mereka bisa berinteraksi
            Debug.Log("Pemain masuk jangkauan. Tekan 'E' untuk berinteraksi.");
        }
    }

    // Fungsi ini akan terpanggil ketika sebuah collider lain KELUAR dari trigger
    private void OnTriggerExit(Collider other) {
        // Periksa apakah yang keluar adalah GameObject dengan tag "Player"
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
            // (Opsional) Beri feedback bahwa pemain sudah tidak bisa berinteraksi
            Debug.Log("Pemain keluar dari jangkauan.");
        }
    }

    // Update dipanggil setiap frame
    private void Update() {
        // Cek jika pemain ada di dalam jangkauan DAN menekan tombol 'E'
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) {
            // Jika interactOnce aktif dan sudah pernah berinteraksi, hentikan fungsi
            if (interactOnce && hasInteracted) {
                return;
            }

            // Jalankan semua fungsi yang ada di dalam UnityEvent "onInteract"
            Debug.Log("Interaksi dijalankan!");
            onInteract?.Invoke();

            // Tandai bahwa interaksi sudah terjadi
            hasInteracted = true;
        }
    }
}