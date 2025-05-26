using UnityEngine;
using UnityEngine.InputSystem;
using System;

// Interface untuk objek interaktif
public interface IInteractPlayer {
    // Fungsi interaksi yang harus diimplementasikan oleh objek interaktif
    void Interact();
}

public class PlayerInteraction : MonoBehaviour {
    [SerializeField] private Player player;

    [Header("Optional Dialogue")]
    [SerializeField] private bool useDialogue; // Toggle untuk mengaktifkan dialogue (jika diperlukan)
    [SerializeField] private PlayerDialogueHandler dialogueHandler; // Reference ke DialogueHandler (optional)

    // Event global (jika diperlukan oleh skrip lain)
    public event Action OnInteract;

    // Target interaksi aktif: Menggunakan IInteractPlayer
    public IInteractPlayer currentInteractable { get; private set; }

    private void Start() {
        if (player == null) {
            Debug.LogError("PlayerInteraction: Player reference belum diassign di Inspector!");
            return;
        }
        if (player.PlayerControls == null) {
            Debug.LogError("PlayerInteraction: PlayerControls belum diinisialisasi di dalam Player!");
            return;
        }
        // Berlangganan ke input action interaksi
        player.PlayerControls.Character.Interaction.performed += OnInteractionPerformed;
    }

    private void OnDestroy() {
        if (player != null && player.PlayerControls != null) {
            player.PlayerControls.Character.Interaction.performed -= OnInteractionPerformed;
        }
    }

    // Fungsi yang dipanggil saat tombol interaksi ditekan
    private void OnInteractionPerformed(InputAction.CallbackContext ctx) {
        Debug.Log("Player melakukan interaksi!");

        // Panggil fungsi Interact() di target interaksi aktif (jika ada)
        if (currentInteractable != null) {
            currentInteractable.Interact();
        }

        // Jika opsi dialogue aktif dan DialogueHandler telah diassign, trigger dialog
        if (useDialogue && dialogueHandler != null) {
            dialogueHandler.TriggerDialogue();
        }

        // Memancarkan event global (jika skrip lain membutuhkan)
        OnInteract?.Invoke();
    }

    // Fungsi untuk mendaftarkan objek interaktif sebagai target aktif
    public void SetCurrentInteractable(IInteractPlayer interactable) {
        currentInteractable = interactable;
    }

    // Fungsi untuk menghapus objek interaktif dari target aktif
    public void ClearCurrentInteractable(IInteractPlayer interactable) {
        if (currentInteractable == interactable) {
            currentInteractable = null;
        }
    }
}
