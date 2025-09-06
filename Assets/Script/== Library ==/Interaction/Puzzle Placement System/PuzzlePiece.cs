using UnityEngine;

/// <summary>
/// Menandai objek ini sebagai potongan puzzle yang bisa diambil.
/// Pasang pada setiap potongan puzzle.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PuzzlePiece : MonoBehaviour {
    [Tooltip("ID unik untuk potongan ini (misal: 'TopLeft', 'Piece_A').")]
    public string pieceID;

    private bool isPlayerInRange = false;
    private PuzzleInteractor playerInteractor;

    private void Awake() {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerInteractor = other.GetComponent<PuzzleInteractor>();
            if (playerInteractor != null) {
                isPlayerInRange = true;
                Debug.Log("Pemain di dekat potongan puzzle. Tekan 'E' untuk mengambil.");
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
            playerInteractor = null;
        }
    }

    private void Update() {
        // Jika pemain di dekat, menekan E, dan belum memegang apa-apa
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && playerInteractor.GetHeldPiece() == null) {
            // Nonaktifkan script ini agar tidak bisa diambil lagi saat sudah di tangan
            this.enabled = false;
            // Beritahu interactor pemain untuk mengambil objek ini
            playerInteractor.PickUpPiece(this.gameObject);
        }
    }
}