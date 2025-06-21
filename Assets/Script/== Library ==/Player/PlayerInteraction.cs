using UnityEngine;
using UnityEngine.InputSystem;
using System;

public interface IInteractPlayer {
    void Interact();
}

public class PlayerInteraction : MonoBehaviour {
    [SerializeField] private Player player;
    [Header("Optional Dialogue")]
    [SerializeField] private bool useDialogue;
    [SerializeField] private PlayerDialogueHandler dialogueHandler;

    public event Action OnInteract;
    public IInteractPlayer currentInteractable { get; private set; }

    // BARU: State untuk mengunci interaksi
    private bool isInteractionLocked = false;

    private void Start() {
        if (player == null || player.PlayerControls == null) { Debug.LogError("Player/PlayerControls tidak di-assign!"); return; }
        player.PlayerControls.Character.Interaction.performed += OnInteractionPerformed;
    }

    private void OnDestroy() {
        if (player != null && player.PlayerControls != null) {
            player.PlayerControls.Character.Interaction.performed -= OnInteractionPerformed;
        }
    }

    private void OnInteractionPerformed(InputAction.CallbackContext ctx) {
        if (currentInteractable != null) {
            currentInteractable.Interact();
        }

        if (useDialogue && dialogueHandler != null) {
            dialogueHandler.TriggerDialogue();
        }
        OnInteract?.Invoke();
    }

    // DIUBAH: Fungsi ini sekarang tidak akan berjalan jika interaksi terkunci
    public void SetCurrentInteractable(IInteractPlayer interactable) {
        // Jika sedang memegang objek, jangan biarkan objek lain mencuri fokus.
        if (isInteractionLocked) return;
        currentInteractable = interactable;
    }

    public void ClearCurrentInteractable(IInteractPlayer interactable) {
        if (currentInteractable == interactable) {
            currentInteractable = null;
        }
    }

    // --- FUNGSI BARU UNTUK MENGUNCI/MEMBUKA INTERAKSI ---
    public void LockInteraction() {
        isInteractionLocked = true;
    }

    public void UnlockInteraction() {
        isInteractionLocked = false;
    }
}