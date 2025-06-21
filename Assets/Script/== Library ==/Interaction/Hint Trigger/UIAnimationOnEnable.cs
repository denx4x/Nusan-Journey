using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Pastikan DOTween sudah terinstall

public class UIAnimationOnEnable : MonoBehaviour {
    public float startScale = 0.5f;     // Skala awal saat canvas diaktifkan
    public float endScale = 1.0f;       // Skala akhir setelah animasi
    public float animationDuration = 0.3f; // Durasi animasi dalam detik
    public Ease animationEase = Ease.OutBack; // Jenis efek easing untuk animasi

    private RectTransform rectTransform;

    void Awake() {
        // Dapatkan komponen RectTransform dari GameObject ini
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) {
            Debug.LogError("UIAnimationOnEnable harus ditempelkan pada GameObject dengan komponen RectTransform.", this);
            enabled = false; // Menonaktifkan skrip jika tidak ada RectTransform
        }
    }

    void OnEnable() {
        // Set skala awal saat objek diaktifkan
        rectTransform.localScale = Vector3.one * startScale;

        // Membuat animasi skala menggunakan DOTween
        rectTransform.DOScale(endScale, animationDuration)
            .SetEase(animationEase)
            .SetUpdate(true); // Penting untuk animasi UI yang tidak terpengaruh Time.timeScale
    }
}