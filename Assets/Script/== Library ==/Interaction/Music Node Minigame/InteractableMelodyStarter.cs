using UnityEngine;

// Skrip ini akan dipasang pada objek yang memulai minigame.
// Ia mengimplementasikan interface IInteractPlayer dari skrip Anda.
public class InteractableMelodyStarter : MonoBehaviour, IInteractPlayer {
    [Tooltip("Seret GameObject NoteManager dari Hierarchy ke sini")]
    [SerializeField] private NoteManager noteManager;

    // Fungsi ini akan dipanggil oleh PlayerInteraction saat tombol 'E' ditekan
    public void Interact() {
        if (noteManager != null) {
            // Beri tahu NoteManager untuk memulai permainannya
            noteManager.StartMelodyGame();
        } else {
            Debug.LogError("NoteManager belum di-assign pada " + this.name);
        }
    }

    // Saat player masuk ke area trigger, daftarkan objek ini sebagai 'interactable'
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            // Ambil komponen PlayerInteraction dari player dan set 'this' sebagai target
            other.GetComponent<PlayerInteraction>()?.SetCurrentInteractable(this);
            // Tambahan: Mungkin tampilkan UI prompt "Tekan E untuk bermain"
        }
    }

    // Saat player keluar dari area trigger, hapus referensi agar tidak bisa interact dari jauh
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerInteraction>()?.ClearCurrentInteractable(this);
            // Tambahan: Sembunyikan UI prompt
        }
    }
}