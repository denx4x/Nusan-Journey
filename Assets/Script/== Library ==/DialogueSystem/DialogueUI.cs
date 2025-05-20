using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour {
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;

    public bool IsOpen { get; private set; }

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;

    [SerializeField] private GameObject audioSourceObject; // GameObject dengan komponen AudioSource
    private PlayerDialogueHandler PlayerDialogueHandler;

    private void Start() {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        PlayerDialogueHandler = FindObjectOfType<PlayerDialogueHandler>();  // Inisialisasi PlayerController
        CloseDialogueBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject) {
        IsOpen = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
        PlayerDialogueHandler.enabled = false;
        Debug.Log("ShowDialogue Jalan");
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents) {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject) {
        Debug.Log("StepThroughDialogue Jalan");
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++) // Menggunakan for loop dengan iterator i
        {
            DialogueObject.DialogueEntry entry = dialogueObject.Dialogue[i];

            if (entry.Audio != null) {
                // Mendapatkan komponen AudioSource dari audioSourceObject
                AudioSource audioSource = audioSourceObject.GetComponent<AudioSource>();
                if (audioSource != null) {
                    audioSource.PlayOneShot(entry.Audio, 1f); // Mengatur volume maksimal
                    audioSource.spatialBlend = 0.0f; // Mengatur spatial blend ke 2D
                } else {
                    Debug.LogError("AudioSource tidak ditemukan pada audioSourceObject.");
                }
            }
            string dialogueText = entry.Text1;

            yield return RunTypingEffect(dialogueText);

            textLabel.text = dialogueText;

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            Debug.Log("For Jalan");
        }

        if (dialogueObject.HasResponses) {
            responseHandler.ShowResponses(dialogueObject.Responses);
        } else {
            CloseDialogueBox();
        }
    }

    private IEnumerator RunTypingEffect(string dialogue) {
        typewriterEffect.Run(dialogue, textLabel);

        while (typewriterEffect.IsRunning) {
            yield return null;

            if (Input.GetKeyDown(KeyCode.LeftAlt)) {
                typewriterEffect.Stop();
            }
        }
    }

    public void CloseDialogueBox() {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
        PlayerDialogueHandler.enabled = true;
    }
}
