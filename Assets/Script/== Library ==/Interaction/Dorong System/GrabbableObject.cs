using UnityEngine;

// Skrip ini mengimplementasikan IInteractPlayer agar bisa diinteraksi
public class GrabbableObject : MonoBehaviour, IInteractPlayer {
    private Rigidbody rb;
    private Transform playerHoldPoint;
    private PlayerMovement playerMovement;

    private bool isBeingHeld = false;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Fungsi ini dipanggil oleh PlayerInteraction saat tombol Interact ditekan
    public void Interact() {
        if (!isBeingHeld) {
            // --- LOGIKA MENGAMBIL OBJEK ---
            if (playerHoldPoint != null) {
                // Matikan fisika agar objek mengikuti player
                rb.isKinematic = true;

                // Tempelkan objek ke holdPoint player
                transform.SetParent(playerHoldPoint);
                transform.localPosition = Vector3.zero; // Reset posisi relatif terhadap holdPoint
                transform.localRotation = Quaternion.identity; // Reset rotasi

                // Beritahu PlayerMovement bahwa kita sedang mode mendorong/membawa
                if (playerMovement != null) playerMovement.IsPushing = true;

                isBeingHeld = true;
                Debug.Log("Objek diambil!");
            }
        } else {
            // --- LOGIKA MELEPASKAN OBJEK ---
            // Kembalikan objek ke root hierarchy
            transform.SetParent(null);

            // Aktifkan lagi fisika agar objek jatuh/berhenti secara natural
            rb.isKinematic = false;

            // Beritahu PlayerMovement kita sudah tidak mendorong
            if (playerMovement != null) playerMovement.IsPushing = false;

            isBeingHeld = false;
            Debug.Log("Objek dilepaskan!");
        }
    }

    // Saat player masuk jangkauan, siapkan referensi yang dibutuhkan
    // Tempelkan fungsi ini ke dalam GrabbableObject.cs
    private void OnTriggerEnter(Collider other) {
        if (isBeingHeld) return;

        // Pesan 1: Untuk mengecek apakah trigger berfungsi
        Debug.Log("Trigger dimasuki oleh objek: " + other.name);

        if (other.CompareTag("Player")) {
            // Pesan 2: Untuk mengecek apakah Tag Player terdeteksi
            Debug.Log("Objek yang masuk adalah Player.");

            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null) {
                // Pesan 3: Untuk mengecek apakah interaksi berhasil didaftarkan
                Debug.Log("PlayerInteraction DITEMUKAN! Mendaftarkan objek ini sebagai interactable.");
                playerInteraction.SetCurrentInteractable(this);
            } else {
                Debug.LogError("Player TIDAK MEMILIKI skrip PlayerInteraction!");
            }

            // Cek juga holdPoint dan PlayerMovement di sini
            playerHoldPoint = other.transform.Find("holdPoint");
            if (playerHoldPoint == null) {
                Debug.LogError("Tidak bisa menemukan 'holdPoint' sebagai child dari Player!");
            }

            playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement == null) {
                Debug.LogError("Player TIDAK MEMILIKI skrip PlayerMovement!");
            }
        }
    }

    // Saat player keluar jangkauan, hapus referensi
    private void OnTriggerExit(Collider other) {
        if (isBeingHeld) return;

        if (other.CompareTag("Player")) {
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null) {
                playerInteraction.ClearCurrentInteractable(this);
            }

            // Hapus referensi
            playerHoldPoint = null;
            playerMovement = null;
        }
    }
}