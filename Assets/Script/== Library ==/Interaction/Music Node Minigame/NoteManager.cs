using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoteManager : MonoBehaviour {
    [Header("Game Components")] // <-- Atribut untuk merapikan Inspector
    [Tooltip("Seret semua objek NoteCube dari scene ke sini")]
    public List<NoteCube> allNoteCubes;

    [Tooltip("Teks untuk menampilkan status (misal: 'Dengarkan', 'Giliranmu')")]
    public TextMeshProUGUI statusText;

    [Header("Game Settings")] // <-- Atribut untuk merapikan Inspector
    [Tooltip("Berapa banyak nada dalam urutan awal")]
    public int sequenceLength = 3;

    [Tooltip("Jeda waktu antar nada saat melodi dimainkan (detik)")]
    public float delayBetweenNotes = 0.7f;

    // <-- PERUBAHAN DI SINI: Variabel baru untuk jeda debounce yang muncul di Inspector
    [SerializeField]
    [Range(0.1f, 1.0f)] // Opsi: Batasi nilai antara 0.1 dan 1 detik agar lebih aman
    [Tooltip("Jeda aman (detik) setelah input nada diterima untuk mencegah input ganda.")]
    private float inputDebounceTime = 0.2f;

    private List<int> melodySequence = new List<int>();
    private int playerInputStep = 0;
    private AudioSource audioSource;

    private enum GameState { Idle, Listen, Play, Correct, Wrong }
    private GameState currentState;

    private bool isAcceptingInput = false;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        currentState = GameState.Idle;
    }

    public void StartMelodyGame() {
        if (currentState == GameState.Idle) {
            Debug.Log("Melody Game has been started by an event!");
            StartCoroutine(StartNewRound());
        }
    }

    private IEnumerator StartNewRound() {
        if (statusText != null) statusText.text = "Bersiap...";
        yield return new WaitForSeconds(2f);

        GenerateMelody();
        yield return StartCoroutine(PlayMelody());
    }

    void GenerateMelody() {
        melodySequence.Clear();
        for (int i = 0; i < sequenceLength; i++) {
            int randomNoteIndex = Random.Range(0, allNoteCubes.Count);
            melodySequence.Add(allNoteCubes[randomNoteIndex].noteID);
        }
        playerInputStep = 0;
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

        currentState = GameState.Play;
        isAcceptingInput = true;
        if (statusText != null) statusText.text = "Giliranmu! Ikuti Nadanya.";
    }

    public void PlayerSteppedOnCube(NoteCube cube) {
        if (currentState != GameState.Play || !isAcceptingInput) return;

        isAcceptingInput = false;

        cube.PlayNoteFeedback();

        if (cube.noteID == melodySequence[playerInputStep]) {
            playerInputStep++;
            if (playerInputStep >= melodySequence.Count) {
                currentState = GameState.Correct;
                if (statusText != null) statusText.text = "Hebat! Nada Benar!";
                sequenceLength++;
                StartCoroutine(StartNewRound());
            } else {
                // <-- PERUBAHAN KUNCI: Gunakan variabel dari Inspector, bukan angka hardcode.
                StartCoroutine(EnableInputAfterDelay(inputDebounceTime));
            }
        } else {
            currentState = GameState.Wrong;
            if (statusText != null) statusText.text = "Salah! Coba Lagi...";
            StartCoroutine(ReplayRound());
        }
    }

    private IEnumerator EnableInputAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        if (currentState == GameState.Play) {
            isAcceptingInput = true;
        }
    }

    private IEnumerator ReplayRound() {
        yield return new WaitForSeconds(2f);
        playerInputStep = 0;
        yield return StartCoroutine(PlayMelody());
    }

    private NoteCube FindCubeByID(int id) {
        foreach (NoteCube cube in allNoteCubes) {
            if (cube.noteID == id) return cube;
        }
        return null;
    }
}