using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mengelola penempatan sebuah potongan puzzle di lokasi yang benar.
/// Pasang pada GameObject 'slot' atau 'frame' puzzle.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PuzzlePlacement : MonoBehaviour {
    [Header("Pengaturan Puzzle")]
    [Tooltip("ID potongan puzzle yang cocok untuk slot ini.")]
    [SerializeField] private string requiredPieceID;

    [Tooltip("Transform kosong sebagai titik di mana puzzle akan 'snap' (posisi & rotasi).")]
    [SerializeField] private Transform placementPoint;

    [Header("Event")]
    [Tooltip("Event yang dijalankan setelah potongan yang benar berhasil diletakkan.")]
    public UnityEvent onPiecePlaced;

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
                Debug.Log($"Pemain di dekat slot '{requiredPieceID}'. Tekan 'E' untuk meletakkan.");
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
        if (!isPlayerInRange || !Input.GetKeyDown(KeyCode.E)) return;

        GameObject heldPiece = playerInteractor.GetHeldPiece();

        // Jika pemain tidak memegang apa-apa, jangan lakukan apa-apa
        if (heldPiece == null) return;

        // Cek apakah potongan yang dipegang pemain adalah yang benar
        PuzzlePiece puzzlePiece = heldPiece.GetComponent<PuzzlePiece>();
        if (puzzlePiece != null && puzzlePiece.pieceID == requiredPieceID) {
            PlaceThePiece(heldPiece);
        } else {
            Debug.Log("Potongan puzzle tidak cocok!");
            // Opsional: Mainkan suara 'gagal'
        }
    }

    private void PlaceThePiece(GameObject piece) {
        // Beritahu interactor bahwa potongan sudah diletakkan
        playerInteractor.PlacePiece();

        // Lepaskan dari parent (pemain)
        piece.transform.SetParent(null);

        // Pindahkan ke posisi dan rotasi yang sudah ditentukan
        piece.transform.position = placementPoint.position;
        piece.transform.rotation = placementPoint.rotation;

        // Matikan komponen yang tidak perlu lagi pada potongan puzzle
        Destroy(piece.GetComponent<PuzzlePiece>()); // Hapus script agar tidak bisa diambil lagi
        if (piece.GetComponent<Rigidbody>()) Destroy(piece.GetComponent<Rigidbody>());

        // Jalankan event sukses
        onPiecePlaced?.Invoke();

        // Nonaktifkan slot ini agar tidak bisa digunakan lagi
        Debug.Log($"Potongan '{requiredPieceID}' berhasil diletakkan!");
        this.enabled = false;
        GetComponent<Collider>().enabled = false;
    }
}