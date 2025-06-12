using UnityEngine;

public class MelodyGameTrigger : MonoBehaviour {
    [Tooltip("Seret GameObject NoteManager dari Hierarchy ke sini")]
    public NoteManager noteManager;

    private void OnTriggerEnter(Collider other) {
        // Cek jika yang masuk adalah player dan NoteManager sudah di-assign
        if (other.CompareTag("Player") && noteManager != null) {
            // Panggil fungsi publik di NoteManager untuk memulai game
            noteManager.StartMelodyGame();

            // Hancurkan objek trigger ini agar tidak bisa digunakan lagi
            Destroy(gameObject);
        }
    }
}