using UnityEngine;

public class NoteCube : MonoBehaviour {
    [Tooltip("ID unik untuk nada ini (misal: 0 untuk C, 1 untuk D, dst.)")]
    public int noteID;

    [Tooltip("File audio untuk nada ini")]
    public AudioClip noteSound;

    private AudioSource audioSource;
    private NoteManager noteManager; // <-- DIUBAH DARI GameManager
    private Renderer cubeRenderer;
    private Color originalColor;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && noteSound != null) {
            audioSource.clip = noteSound;
        }

        // Cari NoteManager di scene
        noteManager = FindObjectOfType<NoteManager>(); // <-- DIUBAH DARI GameManager

        cubeRenderer = GetComponent<Renderer>();
        originalColor = cubeRenderer.material.color;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            // Beri tahu NoteManager bahwa kubus ini diinjak
            if (noteManager != null) {
                noteManager.PlayerSteppedOnCube(this); // <-- DIUBAH DARI gameManager
            }
        }
    }

    public void PlayNoteFeedback() {
        if (audioSource != null) {
            audioSource.Play();
        }
        StartCoroutine(FlashColor());
    }

    public void Highlight() {
        StartCoroutine(FlashColor());
    }

    private System.Collections.IEnumerator FlashColor() {
        cubeRenderer.material.color = Color.cyan;
        yield return new WaitForSeconds(0.5f);
        cubeRenderer.material.color = originalColor;
    }
}