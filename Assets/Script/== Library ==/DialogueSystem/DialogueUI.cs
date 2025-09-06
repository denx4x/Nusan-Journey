using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;

public class DialogueUI : MonoBehaviour {
    [Header("UI Components")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;

    [Header("Settings")]
    public bool IsOpen { get; private set; }

    [Header("Events")]
    public UnityEvent OnDialogueIsCompleted;

    [Header("Internal References")]
    [SerializeField] private GameObject audioSourceObject;
    [Tooltip("Drag GameObject dari scene yang memiliki skrip DialogueSignalReceiver ke sini.")]
    [SerializeField] private DialogueSignalReceiver signalReceiver;

    private PlayerDialogueHandler PlayerDialogueHandler;
    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private AudioSource audioSource;

    private CanvasGroup dialogueBoxCanvasGroup;
    private RectTransform dialogueBoxRect;

    private bool proceedToNextLine = false;
    private bool isSkipping = false;

    private void Start() {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        PlayerDialogueHandler = FindObjectOfType<PlayerDialogueHandler>();
        dialogueBoxCanvasGroup = dialogueBox.GetComponent<CanvasGroup>();
        dialogueBoxRect = dialogueBox.GetComponent<RectTransform>();

        if (audioSourceObject != null) audioSource = audioSourceObject.GetComponent<AudioSource>();

        if (dialogueBoxCanvasGroup != null) dialogueBoxCanvasGroup.alpha = 0;
        dialogueBox.SetActive(false);
    }

    public void OnSkipClicked() {
        if (!isSkipping) {
            StartCoroutine(SkipCoroutine());
        }
    }

    private IEnumerator SkipCoroutine() {
        isSkipping = true;

        if (audioSource != null && audioSource.isPlaying) {
            audioSource.Stop();
        }
        if (typewriterEffect.IsRunning) {
            typewriterEffect.Stop();
        }

        proceedToNextLine = true;

        yield return new WaitForSeconds(0.1f);
        isSkipping = false;
    }

    public void ShowDialogue(DialogueObject dialogueObject) {
        IsOpen = true;
        if (PlayerDialogueHandler) PlayerDialogueHandler.enabled = false;

        dialogueBoxRect.DOKill();
        dialogueBoxCanvasGroup.DOKill();

        dialogueBox.SetActive(true);
        dialogueBoxRect.localScale = Vector3.one * 0.9f;
        dialogueBoxRect.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
        dialogueBoxCanvasGroup.DOFade(1f, 0.3f);

        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void CloseDialogueBox() {
        if (audioSource != null && audioSource.isPlaying) {
            audioSource.Stop();
        }

        dialogueBoxRect.DOKill();
        dialogueBoxCanvasGroup.DOKill();

        Sequence closeSequence = DOTween.Sequence();
        closeSequence.Join(dialogueBoxRect.DOScale(0.9f, 0.3f).SetEase(Ease.InBack));
        closeSequence.Join(dialogueBoxCanvasGroup.DOFade(0f, 0.3f));
        closeSequence.OnComplete(() => {
            IsOpen = false;
            dialogueBox.SetActive(false);
            textLabel.text = string.Empty;
            if (PlayerDialogueHandler) PlayerDialogueHandler.enabled = true;
            OnDialogueIsCompleted?.Invoke();
        });
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents) {
        if (responseHandler != null) responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject) {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++) {
            DialogueObject.DialogueEntry entry = dialogueObject.Dialogue[i];

            proceedToNextLine = false;

            if (signalReceiver != null && !string.IsNullOrWhiteSpace(entry.eventOnLineShown)) {
                signalReceiver.ReceiveSignal(entry.eventOnLineShown);
            }

            if (entry.Audio != null && audioSource != null) {
                audioSource.PlayOneShot(entry.Audio, 1f);
            }

            yield return RunTypingEffect(entry.Text1, entry.typewriterSpeedMultiplier);

            if (signalReceiver != null && !string.IsNullOrWhiteSpace(entry.eventOnLineFinished)) {
                signalReceiver.ReceiveSignal(entry.eventOnLineFinished);
            }

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || proceedToNextLine);

            // --- PERUBAHAN DI SINI: Tambahkan jeda 1 frame ---
            // Ini memastikan semua event di frame saat ini selesai sebelum lanjut ke baris berikutnya.
            yield return null;
        }

        if (dialogueObject.HasResponses) {
            if (responseHandler != null) responseHandler.ShowResponses(dialogueObject.Responses);
        } else {
            CloseDialogueBox();
        }
    }

    private IEnumerator RunTypingEffect(string dialogue, float speedMultiplier) {
        typewriterEffect.Run(dialogue, textLabel, speedMultiplier);
        while (typewriterEffect.IsRunning) {
            yield return null;
            if (Input.GetKeyDown(KeyCode.LeftAlt)) {
                OnSkipClicked();
            }
        }
    }
}