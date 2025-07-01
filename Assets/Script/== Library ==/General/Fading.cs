using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Mengontrol efek fade-in dan fade-out untuk sebuah UI Image.
/// Kini dilengkapi dengan durasi total dan durasi transisi alpha yang terpisah.
/// </summary>
public class Fading : MonoBehaviour {
    [Header("Pengaturan Fade Utama")]
    [Tooltip("Seret komponen Image dari UI Panel hitam Anda ke sini.")]
    [SerializeField] private Image fadeScreen;

    [Tooltip("Durasi TOTAL untuk keseluruhan efek fade (termasuk waktu jeda/tahan).")]
    [SerializeField] private float fadeDuration = 2.0f; // Contoh: total durasi 2 detik

    // ---- VARIABEL BARU ----
    [Tooltip("Durasi KHUSUS untuk perubahan alpha dari transparan ke hitam (atau sebaliknya). Nilai ini harus lebih kecil atau sama dengan Fade Duration.")]
    [SerializeField] private float alphaTransitionDuration = 0.5f; // Contoh: transisi alpha hanya 0.5 detik

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


    // --- Coroutine Inti (Telah Dimodifikasi) ---
    private IEnumerator Fade(float targetAlpha) {
        if (fadeScreen == null) {
            Debug.LogError("Error: Komponen 'Fade Screen' belum di-assign di Inspector!");
            yield break;
        }

        // Memastikan durasi transisi tidak melebihi durasi total
        if (alphaTransitionDuration > fadeDuration) {
            Debug.LogWarning("Alpha Transition Duration tidak boleh lebih besar dari Fade Duration. Nilai disamakan.");
            alphaTransitionDuration = fadeDuration;
        }

        Color screenColor = fadeScreen.color;
        float startAlpha = screenColor.a;
        float holdDuration = fadeDuration - alphaTransitionDuration; // Menghitung sisa waktu untuk jeda

        // === Logika FADE OUT (Layar menjadi hitam) ===
        // Transisi alpha terjadi LEBIH DULU, baru ditahan (jeda).
        if (targetAlpha > startAlpha) {
            // Tahap 1: Transisi Alpha
            float timer = 0f;
            while (timer < alphaTransitionDuration) {
                timer += Time.deltaTime;
                float progress = timer / alphaTransitionDuration;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                fadeScreen.color = new Color(screenColor.r, screenColor.g, screenColor.b, newAlpha);
                yield return null;
            }
            // Memastikan alpha akhir sesuai target
            fadeScreen.color = new Color(screenColor.r, screenColor.g, screenColor.b, targetAlpha);

            // Tahap 2: Menahan layar hitam
            if (holdDuration > 0) {
                yield return new WaitForSeconds(holdDuration);
            }
        }
        // === Logika FADE IN (Layar menjadi transparan) ===
        // Layar ditahan (jeda) LEBIH DULU, baru transisi alpha.
        else {
            // Tahap 1: Menahan layar hitam
            if (holdDuration > 0) {
                yield return new WaitForSeconds(holdDuration);
            }

            // Tahap 2: Transisi Alpha
            float timer = 0f;
            while (timer < alphaTransitionDuration) {
                timer += Time.deltaTime;
                float progress = timer / alphaTransitionDuration;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                fadeScreen.color = new Color(screenColor.r, screenColor.g, screenColor.b, newAlpha);
                yield return null;
            }
            // Memastikan alpha akhir sesuai target
            fadeScreen.color = new Color(screenColor.r, screenColor.g, screenColor.b, targetAlpha);
        }

        // Menjalankan Event setelah seluruh durasi (termasuk jeda) selesai
        if (targetAlpha == 0f) {
            OnFadeInComplete?.Invoke();
        } else if (targetAlpha == 1f) {
            OnFadeOutComplete?.Invoke();
        }
    }


    // --- Fungsi Publik untuk Kontrol (Tidak ada perubahan) ---
    public void FadeIn() {
        StartCoroutine(Fade(0f));
    }

    public void FadeOut() {
        StartCoroutine(Fade(1f));
    }


    // --- Fungsi Bawaan Unity (Tidak ada perubahan signifikan) ---
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
        } else {
            // Defaultnya, layar mulai transparan jika tidak ada opsi Start yang aktif
            // fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0f);
        }
    }
}