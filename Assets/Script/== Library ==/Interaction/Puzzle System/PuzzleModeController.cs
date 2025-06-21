using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events; // <-- Pastikan using directive ini ada
using Cinemachine;

public class PuzzleModeController : MonoBehaviour {
    public static PuzzleModeController Instance { get; private set; }

    [Header("Referensi Komponen Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInteraction playerInteraction;

    [Header("Referensi Kamera")]
    [SerializeField] private GameObject playerFollowCamera;
    [SerializeField] private GameObject puzzleCamera;

    [Header("Referensi UI untuk Transisi")]
    [SerializeField] private Image fadeScreen;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Events")] // <-- BAGIAN BARU
    [Tooltip("Event ini akan dipanggil SETELAH transisi keluar dari mode puzzle selesai.")]
    public UnityEvent OnPuzzleExitComplete;

    private GridPuzzleManager currentPuzzle;
    private bool isTransitioning = false;

    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); } else { Instance = this; }
    }

    public void StartPuzzle(GridPuzzleManager puzzleToStart) {
        if (isTransitioning || puzzleToStart == null) return;
        StartCoroutine(StartPuzzleSequence(puzzleToStart));
    }

    public void EndPuzzle() {
        if (isTransitioning || currentPuzzle == null) return;
        StartCoroutine(EndPuzzleSequence());
    }

    private IEnumerator StartPuzzleSequence(GridPuzzleManager puzzleToStart) {
        isTransitioning = true;
        yield return StartCoroutine(Fade(1f));

        currentPuzzle = puzzleToStart;
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerInteraction != null) playerInteraction.enabled = false;

        currentPuzzle.gameObject.SetActive(true);
        currentPuzzle.enabled = true;
        // Berlangganan ke C# Action dari GridPuzzleManager
        currentPuzzle.OnPuzzleCompleted += HandlePuzzleCompletion;

        if (playerFollowCamera != null) playerFollowCamera.SetActive(false);
        if (puzzleCamera != null) puzzleCamera.SetActive(true);

        yield return StartCoroutine(Fade(0f));
        isTransitioning = false;
    }

    private IEnumerator EndPuzzleSequence() {
        isTransitioning = true;
        yield return StartCoroutine(Fade(1f));

        if (currentPuzzle != null) {
            // Berhenti berlangganan
            currentPuzzle.OnPuzzleCompleted -= HandlePuzzleCompletion;
            currentPuzzle.enabled = false;
        }

        if (playerMovement != null) playerMovement.enabled = true;
        if (playerInteraction != null) playerInteraction.enabled = true;

        if (puzzleCamera != null) puzzleCamera.SetActive(false);
        if (playerFollowCamera != null) playerFollowCamera.SetActive(true);

        currentPuzzle = null;

        yield return StartCoroutine(Fade(0f));

        // --- PANGGIL UNITY EVENT DI AKHIR TRANSISI ---
        Debug.Log("Transisi keluar puzzle selesai, memanggil UnityEvent OnPuzzleExitComplete.");
        OnPuzzleExitComplete?.Invoke();
        // -------------------------------------------

        isTransitioning = false;
    }

    // Fungsi baru untuk menjadi "pendengar" dari C# Action
    private void HandlePuzzleCompletion() {
        EndPuzzle();
    }

    private IEnumerator Fade(float targetAlpha) { /* ... (fungsi fade tidak berubah) ... */
        #region "Fungsi Fade"
        if (fadeScreen == null) { yield break; }
        Color currentColor = fadeScreen.color;
        float startAlpha = currentColor.a;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeScreen.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }
        fadeScreen.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
        #endregion
    }
}