using UnityEngine;

public class PlayerDialogueHandler : MonoBehaviour {
    //PlayerDialogueHandler
    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

    private void Update() {
        ShowDialogue();
    }

    private void ShowDialogue() {
        if (dialogueUI.IsOpen) return;

        if (Input.GetKeyDown(KeyCode.E)) {
            Interactable?.Interact(this);
            Debug.Log("E Jalan");
        }
    }
}
