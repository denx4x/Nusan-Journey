using UnityEngine;
using TMPro;

public class HintTrigger : MonoBehaviour {
    [Header("Hint Configuration")]
    [SerializeField] private string hintMessage = "Tekan E untuk interaksi"; // Pesan utama
    [SerializeField] private string hintKey = "E";

    [SerializeField] private GameObject hintUI;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private TMP_Text hintKeyText;

    private void Start() {       
        if (hintUI != null) {
            hintUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {        
        if (other.CompareTag("Player")) {
            ShowHint();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            HideHint();
        }
    }
    
    private void ShowHint() {
        if (hintUI != null) {            
            if (hintText != null) {
                hintText.text = hintMessage;
            }

            // Update label key hint, melalui fungsi hintKey
            UpdateHintKey();

            hintUI.SetActive(true);
        }
    }

    // Fungsi untuk menyembunyikan hint
    private void HideHint() {
        if (hintUI != null) {
            hintUI.SetActive(false);
        }
    }

    // Fungsi hintKey: mengupdate teks untuk key hint (misal "E" atau "Esc")
    private void UpdateHintKey() {
        if (hintKeyText != null) {
            hintKeyText.text = hintKey;
        }
    }
}
