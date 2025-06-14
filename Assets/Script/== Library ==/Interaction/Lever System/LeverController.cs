using UnityEngine;

// PERUBAHAN: Skrip ini sekarang mengimplementasikan interface IInteractPlayer Anda
public class LeverController : MonoBehaviour, IInteractPlayer {
    [SerializeField] private MovingPlatform platformToControl;
    [SerializeField] private Animator leverAnimator; // Opsional: untuk animasi tuas
    [Tooltip("UI Prompt yang muncul saat player bisa berinteraksi")]
    [SerializeField] private GameObject interactionPromptUI; // Opsional: UI seperti "Tekan E"

    private void Start() {
        // Pastikan UI prompt disembunyikan di awal
        if (interactionPromptUI != null) {
            interactionPromptUI.SetActive(false);
        }
    }

    // WAJIB: Implementasi fungsi dari interface IInteractPlayer
    public void Interact() {
        Debug.Log("Lever diaktifkan melalui interface!");

        // Beri tahu platform untuk bergerak
        if (platformToControl != null) {
            platformToControl.ActivatePlatform();
        }

        // Mainkan animasi tuas (jika ada)
        if (leverAnimator != null) {
            leverAnimator.SetTrigger("Pull"); // Ganti "Pull" dengan nama trigger di Animator Anda
        }
    }

    // Fungsi ini akan dipanggil ketika player masuk ke dalam jangkauan trigger tuas
    private void OnTriggerEnter(Collider other) {
        // Cek apakah yang masuk adalah Player dan memiliki komponen PlayerInteraction
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerInteraction>(out PlayerInteraction playerInteraction)) {
            // Daftarkan tuas ini sebagai target interaksi saat ini
            playerInteraction.SetCurrentInteractable(this);

            // Tampilkan UI Prompt (jika ada)
            if (interactionPromptUI != null) {
                interactionPromptUI.SetActive(true);
            }
        }
    }

    // Fungsi ini akan dipanggil ketika player keluar dari jangkauan trigger tuas
    private void OnTriggerExit(Collider other) {
        // Cek apakah yang keluar adalah Player dan memiliki komponen PlayerInteraction
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerInteraction>(out PlayerInteraction playerInteraction)) {
            // Hapus tuas ini dari target interaksi
            playerInteraction.ClearCurrentInteractable(this);

            // Sembunyikan UI Prompt (jika ada)
            if (interactionPromptUI != null) {
                interactionPromptUI.SetActive(false);
            }
        }
    }
}