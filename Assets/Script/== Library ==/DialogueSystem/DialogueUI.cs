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
    [SerializeField] private DialogueSignalReceiver signalReceiver; // <-- Referensi ke penerima sinyal

    private PlayerDialogueHandler PlayerDialogueHandler;
    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;

    private CanvasGroup dialogueBoxCanvasGroup;
    private RectTransform dialogueBoxRect;

    private void Start() {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        PlayerDialogueHandler = FindObjectOfType<PlayerDialogueHandler>();
        dialogueBoxCanvasGroup = dialogueBox.GetComponent<CanvasGroup>();
        dialogueBoxRect = dialogueBox.GetComponent<RectTransform>();

        if (dialogueBoxCanvasGroup != null) dialogueBoxCanvasGroup.alpha = 0;
        dialogueBox.SetActive(false);
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

            // Mengirim sinyal event 'OnLineShown' dari DialogueObject
            if (signalReceiver != null && !string.IsNullOrWhiteSpace(entry.eventOnLineShown)) {
                signalReceiver.ReceiveSignal(entry.eventOnLineShown);
            }

            if (entry.Audio != null && audioSourceObject != null) {
                AudioSource audioSource = audioSourceObject.GetComponent<AudioSource>();
                if (audioSource != null) audioSource.PlayOneShot(entry.Audio, 1f);
            }

            yield return RunTypingEffect(entry.Text1, entry.typewriterSpeedMultiplier);

            // Mengirim sinyal event 'OnLineFinished' dari DialogueObject
            if (signalReceiver != null && !string.IsNullOrWhiteSpace(entry.eventOnLineFinished)) {
                signalReceiver.ReceiveSignal(entry.eventOnLineFinished);
            }

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
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
                typewriterEffect.Stop();
                textLabel.maxVisibleCharacters = dialogue.Length;
            }
        }
    }
}