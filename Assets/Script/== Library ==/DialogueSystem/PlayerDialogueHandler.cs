using UnityEngine;

public class PlayerDialogueHandler : MonoBehaviour {
    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

    private void Start() {
        // Cek apakah ada `PlayerInteraction` di scene dan tambahkan event listener
        PlayerInteraction interaction = FindObjectOfType<PlayerInteraction>();
        if (interaction != null) {
            interaction.OnInteract += TriggerDialogue;
        } else {
            Debug.LogError("PlayerDialogueHandler: PlayerInteraction tidak ditemukan!");
        }
    }

    private void OnDestroy() {
        PlayerInteraction interaction = FindObjectOfType<PlayerInteraction>();
        if (interaction != null) {
            interaction.OnInteract -= TriggerDialogue;
        }
    }

    public void TriggerDialogue() {
        if (dialogueUI.IsOpen) return;

        // Memeriksa apakah ada objek yang bisa berinteraksi
        Interactable?.Interact(this);
        Debug.Log("Dialog aktif melalui PlayerInteraction!");
    }
}
