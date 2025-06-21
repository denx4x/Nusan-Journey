using UnityEngine;
using UnityEngine.Events; // Diperlukan untuk menggunakan UnityEvent

/// <summary>
/// Mendeteksi interaksi pemain dengan tombol 'E' untuk memicu sebuah UnityEvent.
/// Objek ini harus memiliki Collider dengan 'Is Trigger' yang diaktifkan.
/// Pemain juga harus memiliki tag 'Player'.
/// </summary>
public class ClueCanvas : MonoBehaviour {
    // Event yang akan dijalankan saat pemain berinteraksi.
    // Anda bisa mengatur apa yang terjadi dari Unity Inspector.
    public UnityEvent OnInteract;

    // Untuk melacak apakah pemain berada dalam jangkauan.
    private bool isPlayerInRange = false;

    // Update dipanggil setiap frame.
    private void Update() {
        // Cek jika pemain berada dalam jangkauan dan menekan tombol 'E'.
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) {
            // Menjalankan semua fungsi yang terdaftar pada event OnInteract.
            OnInteract.Invoke();
            Debug.Log("Interaksi sukses pada objek: " + gameObject.name);
        }
    }

    // Fungsi ini dipanggil ketika objek lain masuk ke dalam trigger collider.
    private void OnTriggerEnter(Collider other) {
        // Cek apakah yang masuk adalah pemain (berdasarkan tag).
        if (other.CompareTag("Player")) {
            isPlayerInRange = true;
            Debug.Log("Pemain masuk jangkauan.");
            // Anda bisa menambahkan UI prompt di sini untuk memberitahu pemain bisa berinteraksi.
        }
    }

    // Fungsi ini dipanggil ketika objek lain keluar dari trigger collider.
    private void OnTriggerExit(Collider other) {
        // Cek apakah yang keluar adalah pemain.
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
            Debug.Log("Pemain keluar jangkauan.");
            // Anda bisa menyembunyikan UI prompt di sini.
        }
    }
}