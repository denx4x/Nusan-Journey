using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events; // <-- PENTING: Namespace ini wajib ada untuk menggunakan UnityEvent

/// <summary>
/// Mengontrol efek fade-in dan fade-out untuk sebuah UI Image.
/// Kini dilengkapi dengan event yang dieksekusi setelah fade selesai.
/// </summary>
public class Fading : MonoBehaviour {
    [Header("Pengaturan Fade Utama")]
    [Tooltip("Seret komponen Image dari UI Panel hitam Anda ke sini.")]
    [SerializeField] private Image fadeScreen;

    [Tooltip("Durasi total untuk animasi fade (dalam detik).")]
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Fitur Otomatis di Start")]
    [Tooltip("Jika dicentang, layar akan mulai gelap lalu melakukan transisi fade-in saat game dimulai.")]
    [SerializeField] private bool fadeInOnStart = false;

    [Tooltip("Jika dicentang, layar akan mulai transparan lalu melakukan transisi fade-out saat game dimulai.")]
    [SerializeField] private bool fadeOutOnStart = false;

    [Header("Events")]
    [Tooltip("Event ini akan dijalankan setelah transisi FADE IN selesai.")]
    public UnityEvent OnFadeInComplete;

    [Tooltip("Event ini akan dijalankan setelah transisi FADE OUT selesai.")]
    public UnityEvent OnFadeOutComplete;


    // --- Coroutine Inti ---
    private IEnumerator Fade(float targetAlpha) {
        if (fadeScreen == null) {
            Debug.LogError("Error: Komponen 'Fade Screen' belum di-assign di Inspector!");
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

        if (targetAlpha == 0f) {
            OnFadeInComplete?.Invoke();
        } else if (targetAlpha == 1f) {
            OnFadeOutComplete?.Invoke();
        }
    }


    // --- Fungsi Publik untuk Kontrol ---
    public void FadeIn() {
        StartCoroutine(Fade(0f));
    }

    public void FadeOut() {
        StartCoroutine(Fade(1f));
    }


    // --- Fungsi Bawaan Unity ---
    private void Start() {
        if (fadeScreen == null) { return; }

        if (fadeInOnStart && fadeOutOnStart) {
            Debug.LogWarning("Peringatan: 'Fade In On Start' dan 'Fade Out On Start' keduanya aktif. Hanya 'Fade In On Start' yang akan dijalankan untuk menghindari konflik.");
            fadeOutOnStart = false;
        }

        if (fadeInOnStart) {
            // --- KOREKSI DI SINI ---
            // Mengakses .color terlebih dahulu sebelum .r, .g, .b
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1f);
            FadeIn();
        } else if (fadeOutOnStart) {
            // --- KOREKSI DI SINI ---
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0f);
            FadeOut();
        } else {
            // --- KOREKSI DI SINI ---
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0f);
        }
    }
}