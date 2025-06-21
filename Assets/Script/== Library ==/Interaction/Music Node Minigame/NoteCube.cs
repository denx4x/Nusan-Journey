using UnityEngine;
using System.Collections; // Ditambahkan untuk IEnumerator

public class NoteCube : MonoBehaviour {
    [Tooltip("ID unik untuk nada ini (misal: 0 untuk C, 1 untuk D, dst.)")]
    public int noteID;

    [Tooltip("File audio untuk nada ini")]
    public AudioClip noteSound;

    [Header("Visual Feedback Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color confirmFlashColor = Color.cyan;

    private AudioSource audioSource;
    private Renderer cubeRenderer;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null) {
            audioSource.clip = noteSound;
        }

        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer != null) cubeRenderer.material.color = defaultColor;
    }

    // Fungsi ini sekarang hanya dipanggil saat nada dikonfirmasi (Enter) atau saat contoh dimainkan.
    public void PlayNoteFeedback() {
        if (audioSource != null) {
            audioSource.Play();
        }
        StartCoroutine(FlashCoroutine(confirmFlashColor));
    }

    public void Highlight() {
        PlayNoteFeedback();
    }

    public void Select() {
        if (cubeRenderer != null) cubeRenderer.material.color = selectedColor;
    }

    public void Deselect() {
        if (cubeRenderer != null) cubeRenderer.material.color = defaultColor;
    }

    private IEnumerator FlashCoroutine(Color flashColor) {
        // Simpan warna saat ini (yaitu warna 'selected')
        Color originalColor = selectedColor;
        if (cubeRenderer != null) {
            cubeRenderer.material.color = flashColor;
            yield return new WaitForSeconds(0.4f);
            // Hanya kembalikan ke warna 'selected' jika tidak ada yang mengubahnya
            if (cubeRenderer.material.color == flashColor) {
                cubeRenderer.material.color = originalColor;
            }
        }
    }
}