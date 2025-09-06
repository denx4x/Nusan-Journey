using UnityEngine;

/// <summary>
/// Menandai sebuah objek sebagai item yang bisa diambil dengan cara berinteraksi.
/// Pemain harus menekan tombol 'E' saat berada di dalam jangkauan untuk mengambil.
/// </summary>
[RequireComponent(typeof(Collider))] // Memastikan objek punya Collider
public class CollectibleItem : MonoBehaviour {
    [Header("Pengaturan Item")]
    [Tooltip("Nama unik untuk item ini. Nama ini harus sama dengan yang dibutuhkan oleh ItemRequirement.")]
    [SerializeField] private string itemName = "Key";

    // Variabel untuk melacak apakah pemain berada dalam jangkauan
    private bool isPlayerInRange = false;
    // Variabel untuk menyimpan referensi ke inventaris pemain
    private PlayerInventory playerInventory;

    private void Awake() {
        // Pastikan collider-nya adalah trigger agar tidak menghalangi pemain
        GetComponent<Collider>().isTrigger = true;
    }

    // Update dipanggil setiap frame
    private void Update() {
        // Hanya jalankan jika pemain ada di jangkauan DAN menekan tombol 'E'
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) {
            Collect();
        }
    }

    private void OnTriggerEnter(Collider other) {
        // Periksa apakah yang masuk adalah pemain
        if (other.CompareTag("Player")) {
            isPlayerInRange = true;
            // Simpan referensi inventaris pemain untuk digunakan nanti
            playerInventory = other.GetComponent<PlayerInventory>();
            Debug.Log($"Pemain di dekat '{itemName}'. Tekan 'E' untuk mengambil.");
        }
    }

    private void OnTriggerExit(Collider other) {
        // Jika pemain keluar dari jangkauan
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
            // Kosongkan referensi inventaris
            playerInventory = null;
        }
    }

    /// <summary>
    /// Logika untuk mengambil item.
    /// </summary>
    private void Collect() {
        if (playerInventory != null) {
            // Tambahkan item ini ke inventaris pemain
            playerInventory.AddItem(itemName);

            // Hancurkan objek item ini dari scene
            Destroy(gameObject);
        } else {
            Debug.LogWarning("Objek Player tidak memiliki skrip PlayerInventory!");
        }
    }
}