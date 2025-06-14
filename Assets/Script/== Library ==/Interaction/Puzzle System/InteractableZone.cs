using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableZone : MonoBehaviour {
    private IInteractPlayer interactableObject;

    private void Awake() {
        // Pastikan collider adalah trigger
        GetComponent<Collider>().isTrigger = true;
        // Ambil komponen yang bisa di-interact (dalam kasus ini, PuzzleTrigger)
        interactableObject = GetComponent<IInteractPlayer>();
    }

    private void OnTriggerEnter(Collider other) {
        // Cek jika yang masuk adalah player
        if (other.CompareTag("Player")) // Pastikan player Anda punya tag "Player"
        {
            // Coba dapatkan komponen PlayerInteraction dari player
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null) {
                // Set objek ini sebagai target interaksi saat ini
                playerInteraction.SetCurrentInteractable(interactableObject);
                Debug.Log("Player masuk zona interaksi: " + gameObject.name);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null) {
                // Hapus objek ini dari target interaksi
                playerInteraction.ClearCurrentInteractable(interactableObject);
                Debug.Log("Player keluar zona interaksi: " + gameObject.name);
            }
        }
    }
}