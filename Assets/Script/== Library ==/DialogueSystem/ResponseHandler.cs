using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening; // 1. Tambahkan namespace DoTween

public class ResponseHandler : MonoBehaviour {
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private GameObject responseContainerPrefab;
    [SerializeField] private GameObject responseButtonTemplatePrefab;
    [SerializeField] private float responseAnimDelay = 0.1f; // Jeda antar animasi tombol

    private DialogueUI dialogueUI;
    private ResponseEvent[] responseEvents;

    private List<GameObject> tempResponseContainers = new List<GameObject>();

    private void Start() {
        dialogueUI = GetComponent<DialogueUI>();
        // Pastikan responseBox memiliki CanvasGroup untuk animasi fade
        if (responseBox.GetComponent<CanvasGroup>() == null) {
            responseBox.gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void ShowResponses(Response[] responses) {
        responseBox.gameObject.SetActive(true);
        responseBox.GetComponent<CanvasGroup>().alpha = 1; // Pastikan box terlihat

        float responseBoxHeight = 0;

        // Loop untuk instansiasi dan animasi
        for (int i = 0; i < responses.Length; i++) {
            Response response = responses[i];
            GameObject responseContainer = Instantiate(responseContainerPrefab, responseBox.transform);
            responseContainer.SetActive(true);

            GameObject responseButton = Instantiate(responseButtonTemplatePrefab, responseContainer.transform);
            responseButton.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response));

            tempResponseContainers.Add(responseContainer);
            responseBoxHeight += responseButtonTemplatePrefab.GetComponent<RectTransform>().sizeDelta.y;

            // 2. Animasi tombol jawaban
            CanvasGroup containerCanvasGroup = responseContainer.GetComponent<CanvasGroup>();
            RectTransform containerRect = responseContainer.GetComponent<RectTransform>();

            // Atur kondisi awal untuk animasi
            containerCanvasGroup.alpha = 0;
            containerRect.anchoredPosition = new Vector2(100, containerRect.anchoredPosition.y); // Mulai dari kanan

            // Jalankan animasi dengan delay berurutan
            containerCanvasGroup.DOFade(1f, 0.3f).SetDelay(i * responseAnimDelay);
            containerRect.DOAnchorPosX(0, 0.4f).SetEase(Ease.OutCubic).SetDelay(i * responseAnimDelay);
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
    }

    private void OnPickedResponse(Response response) {
        // 3. Animasikan hilangnya response box
        responseBox.GetComponent<CanvasGroup>().DOFade(0, 0.2f).OnComplete(() => {
            // Jalankan logika setelah animasi selesai
            ProcessResponseLogic(response);
        });
    }

    // 4. Pindahkan semua logika lama ke fungsi terpisah
    private void ProcessResponseLogic(Response response) {
        responseBox.gameObject.SetActive(false); // Sembunyikan box setelah fade
        responseBox.GetComponent<CanvasGroup>().alpha = 1; // Reset alpha untuk penggunaan selanjutnya

        foreach (GameObject container in tempResponseContainers) {
            Destroy(container);
        }
        tempResponseContainers.Clear();

        if (responseEvents != null) {
            for (int i = 0; i < dialogueUI.GetComponent<DialogueActivator>().GetComponents<DialogueResponseEvents>().Length; i++) {
                var eventsComponent = dialogueUI.GetComponent<DialogueActivator>().GetComponents<DialogueResponseEvents>()[i];
                for (int j = 0; j < eventsComponent.DialogueObject.Responses.Length; j++) {
                    if (eventsComponent.DialogueObject.Responses[j].ResponseText == response.ResponseText) {
                        if (j < responseEvents.Length) {
                            responseEvents[j].OnPickedResponse?.Invoke();
                        }
                        break;
                    }
                }
            }
        }

        responseEvents = null;

        if (response.DialogueObject) {
            dialogueUI.ShowDialogue(response.DialogueObject);
        } else {
            dialogueUI.CloseDialogueBox();
        }
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents) {
        this.responseEvents = responseEvents;
    }
}