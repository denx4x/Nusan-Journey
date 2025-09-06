using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Mengontrol efek fade-in dan fade-out untuk sebuah UI Image.
/// Kini dilengkapi dengan event untuk start dan complete.
/// </summary>
public class Fading : MonoBehaviour {
    [Header("Pengaturan Fade Utama")]
    [Tooltip("Seret komponen Image dari UI Panel hitam Anda ke sini.")]
    [SerializeField] private Image fadeScreen;

    [Tooltip("Durasi TOTAL untuk keseluruhan efek fade (termasuk waktu jeda/tahan).")]
    [SerializeField] private float fadeDuration = 2.0f;

    [Tooltip("Durasi KHUSUS untuk perubahan alpha. Nilai ini harus lebih kecil atau sama dengan Fade Duration.")]
    [SerializeField] private float alphaTransitionDuration = 0.5f;

    [Header("Fitur Otomatis di Start")]
    [Tooltip("Jika dicentang, layar akan mulai gelap lalu melakukan transisi fade-in saat game dimulai.")]
    [SerializeField] private bool fadeInOnStart = false;

    [Tooltip("Jika dicentang, layar akan mulai transparan lalu melakukan transisi fade-out saat game dimulai.")]
    [SerializeField] private bool fadeOutOnStart = false;

    [Header("Events")]
    // ---- TAMBAHAN BARU ----
    [Tooltip("Event ini akan dijalankan saat transisi FADE IN DIMULAI.")]
    public UnityEvent OnFadeInStart;

    [Tooltip("Event ini akan dijalankan saat transisi FADE OUT DIMULAI.")]
    public UnityEvent OnFadeOutStart;
    // -----------------------

    [Space(10)]
    [Tooltip("Event ini akan dijalankan setelah transisi FADE IN SELESAI.")]
    public UnityEvent OnFadeInComplete;

    [Tooltip("Event ini akan dijalankan setelah transisi FADE OUT SELESAI.")]
    public UnityEvent OnFadeOutComplete;


    // --- Coroutine Inti (Tidak ada perubahan di sini) ---
    private IEnumerator Fade(float targetAlpha) {
        if (fadeScreen == null) {
            Debug.LogError("Error: Komponen 'Fade Screen' belum di-assign di Inspector!");
            yield break;
        }

        if (alphaTransitionDuration > fadeDuration) {
            Debug.LogWarning("Alpha Transition Duration tidak boleh lebih besar dari Fade Duration. Nilai disamakan.");
            alphaTransitionDuration = fadeDuration;
        }

        Color screenColor = fadeScreen.color;
        float startAlpha = screenColor.a;
        float holdDuration = fadeDuration - alphaTransitionDuration;

        // Logika FADE OUT
        if (targetAlpha > startAlpha) {
            float timer = 0f;
            while (timer < alphaTransitionDuration) {
                timer += Time.deltaTime;
                float progress = timer / alphaTransitionDuration;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                fadeScreen.color = new Color(screenColor.r, screenColor.g, screenColor.b, newAlpha);
                yield return null;
            }
            fadeScreen.color = new Color(screenColor.r, screenColor.g, screenColor.b, targetAlpha);

            if (holdDuration > 0) {
                yield return new WaitForSeconds(holdDuration);
            }
        }
        // Logika FADE IN
        else {
            if (holdDuration > 0) {
                yield return new WaitForSeconds(holdDuration);
            }

            float timer = 0f;
            while (timer < alphaTransitionDuration) {
                timer += Time.deltaTime;
                float progress = timer / alphaTransitionDuration;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                fadeScreen.color = new Color(screenColor.r, screenColor.g, screenColor.b, newAlpha);
                yield return null;
            }
            fadeScreen.color = new Color(screenColor.r, screenColor.g, screenColor.b, targetAlpha);
        }

        // Menjalankan Event Selesai
        if (targetAlpha == 0f) {
            OnFadeInComplete?.Invoke();
        } else if (targetAlpha == 1f) {
            OnFadeOutComplete?.Invoke();
        }
    }


    // --- Fungsi Publik untuk Kontrol (Ada Perubahan) ---
    public void FadeIn() {
        // ---- PERUBAHAN ----
        OnFadeInStart?.Invoke(); // Panggil event "start" di sini
        // -------------------
        StartCoroutine(Fade(0f));
    }

    public void FadeOut() {
        // ---- PERUBAHAN ----
        OnFadeOutStart?.Invoke(); // Panggil event "start" di sini
        // -------------------
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
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1f);
            FadeIn();
        } else if (fadeOutOnStart) {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0f);
            FadeOut();
        }
    }
}