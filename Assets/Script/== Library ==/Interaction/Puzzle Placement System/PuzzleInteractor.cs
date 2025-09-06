using UnityEngine;

/// <summary>
/// Mengelola interaksi pemain dengan objek puzzle, seperti memegang dan melepaskan.
/// Pasang skrip ini pada GameObject Player.
/// </summary>
public class PuzzleInteractor : MonoBehaviour {
    [Tooltip("Posisi di depan pemain di mana potongan puzzle akan dipegang.")]
    [SerializeField] private Transform heldItemPosition;

    private GameObject heldPiece = null;

    /// <summary>
    /// Mengambil dan "memegang" sebuah potongan puzzle.
    /// </summary>
    public void PickUpPiece(GameObject piece) {
        if (heldPiece == null) {
            heldPiece = piece;
            // Jadikan potongan puzzle sebagai anak dari transform pemain
            // agar ikut bergerak bersama pemain.
            piece.transform.SetParent(heldItemPosition, false);
            piece.transform.localPosition = Vector3.zero;
            // Nonaktifkan physics saat dipegang
            if (piece.GetComponent<Rigidbody>()) {
                piece.GetComponent<Rigidbody>().isKinematic = true;
            }
            Debug.Log($"Mengambil: {piece.name}");
        }
    }

    /// <summary>
    /// Melepaskan potongan puzzle yang sedang dipegang.
    /// </summary>
    public void PlacePiece() {
        if (heldPiece != null) {
            Debug.Log($"Meletakkan: {heldPiece.name}");
            heldPiece = null;
        }
    }

    /// <summary>
    /// Mengembalikan referensi potongan puzzle yang sedang dipegang.
    /// </summary>
    public GameObject GetHeldPiece() {
        return heldPiece;
    }
}