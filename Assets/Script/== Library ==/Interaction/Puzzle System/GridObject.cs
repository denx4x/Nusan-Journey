using UnityEngine;
using System.Collections;

public enum ObjectType {
    Player,
    Obstacle,
    Wall
}

public class GridObject : MonoBehaviour {
    [Tooltip("Tipe dari objek ini (Player, Obstacle, atau Wall)")]
    public ObjectType type;

    [Header("Visuals")]
    [Tooltip("Objek yang akan diaktifkan untuk menandakan objek ini terpilih.")]
    public GameObject selectionHighlight; // <-- FIELD BARU

    [HideInInspector]
    public Vector2Int gridPosition;

    void Awake() {
        // Pastikan highlight mati di awal permainan
        selectionHighlight?.SetActive(false);
    }

    // Fungsi untuk menampilkan highlight saat objek ini dipilih
    public void Select() {
        if (type == ObjectType.Wall) return; // Dinding tidak bisa dipilih
        selectionHighlight?.SetActive(true);
    }

    // Fungsi untuk menyembunyikan highlight saat objek lain dipilih
    public void Deselect() {
        selectionHighlight?.SetActive(false);
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration) {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration) {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}