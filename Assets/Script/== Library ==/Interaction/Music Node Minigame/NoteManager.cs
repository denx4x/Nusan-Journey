using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class NoteManager : MonoBehaviour {
    public enum MelodyGenerationMode { Random, Template }

    [Header("Game Components")]
    public List<NoteCube> allNoteCubes;
    public TextMeshProUGUI statusText;

    [Header("Player References")]
    public Player player;
    public PlayerMovement playerMovement;

    [Header("Game Settings")]
    public MelodyGenerationMode generationMode = MelodyGenerationMode.Random;
    public int sequenceLength = 3;
    public List<int> melodyTemplate;
    public float delayBetweenNotes = 0.7f;
    [SerializeField]
    [Range(0.1f, 1.0f)]
    private float inputDebounceTime = 0.2f;

    // <-- BARU: Variabel untuk mengatur jeda setelah menang
    [Tooltip("Jeda waktu (detik) setelah menang sebelum keluar dari mode melodi.")]
    [SerializeField] private float delayAfterCompletion = 2.0f;

    [Header("Transitions")]
    [Tooltip("Seret komponen Image dari UI Panel hitam Anda ke sini.")]
    [SerializeField] private Image fadeScreen;
    [Tooltip("Durasi animasi fade dalam detik.")]
    [SerializeField] private float fadeDuration = 0.7f;

    [Header("Events")]
    public UnityEvent OnMelodyIsComplete;

    // --- Variabel Internal ---
    private List<int> melodySequence = new List<int>();
    private int playerInputStep = 0;
    private AudioSource audioSource;
    private enum GameState { Off, Listen, Play, Win, Lose }
    private GameState currentState;
    private bool isAcceptingInput = false;
    private int currentSelectionIndex = 0;
    private bool isTransitioning = false;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        currentState = GameState.Off;
        if (statusText != null) {
            statusText.gameObject.SetActive(false);
        }
        if (fadeScreen != null) {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0);
        }
    }

    private void OnEnable() {
        if (player?.PlayerControls == null) return;
        player.PlayerControls.UI.Pause.performed += OnPauseOrCancelPerformed;
    }

    private void OnDisable() {
        if (player?.PlayerControls == null) return;
        player.PlayerControls.UI.Pause.performed -= OnPauseOrCancelPerformed;
    }

    private void Update() {
        if (isTransitioning || currentState != GameState.Play) return;

        if (Input.GetKeyDown(KeyCode.E)) {
            allNoteCubes[currentSelectionIndex].Deselect();
            currentSelectionIndex = (currentSelectionIndex + 1) % allNoteCubes.Count;
            allNoteCubes[currentSelectionIndex].Select();
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            allNoteCubes[currentSelectionIndex].Deselect();
            currentSelectionIndex--;
            if (currentSelectionIndex < 0) { currentSelectionIndex = allNoteCubes.Count - 1; }
            allNoteCubes[currentSelectionIndex].Select();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            if (isAcceptingInput) {
                isAcceptingInput = false;
                NoteCube selectedCube = allNoteCubes[currentSelectionIndex];
                selectedCube.PlayNoteFeedback();

                if (selectedCube.noteID == melodySequence[playerInputStep]) {
                    playerInputStep++;
                    if (playerInputStep >= melodySequence.Count) {
                        // <-- PERUBAHAN: Panggil Coroutine kemenangan, bukan EndMelodyGame langsung
                        StartCoroutine(WinSequence());
                    } else {
                        StartCoroutine(EnableInputAfterDelay(inputDebounceTime));
                    }
                } else {
                    currentState = GameState.Lose;
                    if (statusText != null) statusText.text = "Salah! Coba Lagi...";
                    StartCoroutine(ReplayRound());
                }
            }
        }
    }

    private void OnPauseOrCancelPerformed(InputAction.CallbackContext context) {
        if (currentState == GameState.Listen || currentState == GameState.Play) {
            // <-- PERUBAHAN: Set pesan dulu, baru panggil EndMelodyGame tanpa parameter
            if (statusText != null) statusText.text = "Permainan dibatalkan.";
            EndMelodyGame();
        }
    }

    // <-- BARU: Coroutine untuk urutan kemenangan
    private IEnumerator WinSequence() {
        currentState = GameState.Win;
        if (statusText != null) statusText.text = "Hebat! Urutan Nada Benar!";        

        // Tunggu sejenak sesuai durasi yang kita tentukan
        yield return new WaitForSeconds(delayAfterCompletion);

        // Setelah menunggu, baru mulai proses keluar dari game
        EndMelodyGame();
    }

    public void StartMelodyGame() {
        if (currentState == GameState.Off && !isTransitioning) {
            StartCoroutine(StartGameSequence());
        }
    }

    private IEnumerator StartGameSequence() {
        isTransitioning = true;
        yield return StartCoroutine(Fade(1f));

        playerMovement.enabled = false;
        if (statusText != null) statusText.gameObject.SetActive(true);
        StartCoroutine(StartNewRound());

        yield return StartCoroutine(Fade(0f));
        isTransitioning = false;
    }

    // <-- PERUBAHAN: EndMelodyGame tidak lagi butuh parameter pesan
    public void EndMelodyGame() {
        if (currentState == GameState.Off || isTransitioning) return;
        StopAllCoroutines();
        StartCoroutine(EndGameSequence());        
    }

    // <-- PERUBAHAN: EndGameSequence tidak lagi butuh parameter pesan
    private IEnumerator EndGameSequence() {
        isTransitioning = true;
        yield return StartCoroutine(Fade(1f));

        currentState = GameState.Off;
        if (allNoteCubes.Count > 0 && currentSelectionIndex < allNoteCubes.Count) {
            allNoteCubes[currentSelectionIndex].Deselect();
        }
        playerMovement.enabled = true;

        // Panggil event
        OnMelodyIsComplete.Invoke();

        if (statusText != null) {
            // Pesan sudah diatur sebelumnya, kita hanya perlu menunggu sebelum menyembunyikannya
            yield return new WaitForSeconds(1.5f);
            statusText.gameObject.SetActive(false);
        }

        yield return StartCoroutine(Fade(0f));
        isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha) {
        if (fadeScreen == null) {
            Debug.LogWarning("Fade Screen tidak di-assign di NoteManager!");
            yield break;
        }

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
    }

    private IEnumerator StartNewRound() {
        if (statusText != null) statusText.text = "Bersiap...";
        yield return new WaitForSeconds(1.5f);
        GenerateMelody();
        yield return StartCoroutine(PlayMelody());
    }

    void GenerateMelody() {
        melodySequence.Clear();
        playerInputStep = 0;
        switch (generationMode) {
            case MelodyGenerationMode.Random:
                if (allNoteCubes.Count == 0) return;
                for (int i = 0; i < sequenceLength; i++) {
                    int randomNoteIndex = UnityEngine.Random.Range(0, allNoteCubes.Count);
                    melodySequence.Add(allNoteCubes[randomNoteIndex].noteID);
                }
                break;
            case MelodyGenerationMode.Template:
                if (melodyTemplate == null || melodyTemplate.Count == 0) return;
                melodySequence = new List<int>(melodyTemplate);
                break;
        }
    }

    private IEnumerator PlayMelody() {
        currentState = GameState.Listen;
        isAcceptingInput = false;
        if (statusText != null) statusText.text = "Dengarkan Baik-baik...";
        yield return new WaitForSeconds(1f);

        foreach (int noteID in melodySequence) {
            NoteCube cubeToPlay = FindCubeByID(noteID);
            if (cubeToPlay != null) {
                audioSource.PlayOneShot(cubeToPlay.noteSound);
                cubeToPlay.Highlight();
            }
            yield return new WaitForSeconds(delayBetweenNotes);
        }

        yield return new WaitForSeconds(0.5f);
        foreach (NoteCube cube in allNoteCubes) {
            cube.Deselect();
        }

        currentState = GameState.Play;
        isAcceptingInput = true;
        if (statusText != null) statusText.text = "Giliranmu! Ikuti Nadanya.";

        currentSelectionIndex = 0;
        if (allNoteCubes.Count > 0) allNoteCubes[currentSelectionIndex].Select();
    }

    private IEnumerator ReplayRound() {
        yield return new WaitForSeconds(1.5f);
        playerInputStep = 0;
        yield return StartCoroutine(PlayMelody());
    }

    private IEnumerator EnableInputAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        if (currentState == GameState.Play) { isAcceptingInput = true; }
    }

    private NoteCube FindCubeByID(int id) {
        foreach (NoteCube cube in allNoteCubes) {
            if (cube.noteID == id) return cube;
        }
        return null;
    }
}