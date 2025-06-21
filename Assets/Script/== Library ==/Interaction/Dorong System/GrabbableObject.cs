using UnityEngine;

public class GrabbableObject : MonoBehaviour, IInteractPlayer {
    private Rigidbody rb;
    private Transform playerHoldPoint;
    private PlayerStateHandler playerStateHandler;
    // BARU: Referensi ke PlayerInteraction
    private PlayerInteraction playerInteraction;

    private bool isBeingHeld = false;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void Interact() {
        if (!isBeingHeld) {
            // Pastikan kita punya semua referensi sebelum mengambil
            if (playerHoldPoint != null && playerStateHandler != null && playerInteraction != null) {
                rb.isKinematic = true;
                transform.SetParent(playerHoldPoint);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                playerStateHandler.IsPushing = true;
                // BARU: Kunci sistem interaksi saat objek dipegang
                playerInteraction.LockInteraction();

                isBeingHeld = true;
            }
        } else {
            // Pastikan kita punya referensi sebelum melepas
            if (playerStateHandler != null && playerInteraction != null) {
                transform.SetParent(null);
                rb.isKinematic = false;

                playerStateHandler.IsPushing = false;
                // BARU: Buka kunci sistem interaksi saat objek dilepas
                playerInteraction.UnlockInteraction();

                isBeingHeld = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (isBeingHeld) return;

        if (other.CompareTag("Player")) {
            // Simpan referensi ke semua komponen player yang dibutuhkan
            playerInteraction = other.GetComponent<PlayerInteraction>();
            if (playerInteraction != null) {
                playerInteraction.SetCurrentInteractable(this);
            }

            playerHoldPoint = other.transform.Find("holdPoint");
            playerStateHandler = other.GetComponent<PlayerStateHandler>();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (isBeingHeld) return;

        if (other.CompareTag("Player")) {
            // Gunakan referensi yang sudah disimpan untuk membersihkan
            if (playerInteraction != null) {
                playerInteraction.ClearCurrentInteractable(this);
            }

            // Hapus semua referensi
            playerInteraction = null;
            playerHoldPoint = null;
            playerStateHandler = null;
        }
    }
}