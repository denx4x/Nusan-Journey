using UnityEngine;
using DG.Tweening;

// Menambahkan CanvasGroup secara otomatis jika belum ada
[RequireComponent(typeof(CanvasGroup))]
public class UIPanelAnimator : MonoBehaviour {
    [Header("Pengaturan Animasi")]
    public float startScale = 0.5f;
    public float endScale = 1.0f;
    public float animationDuration = 0.3f;
    public Ease animationEase = Ease.OutBack;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Pastikan panel tidak terlihat di awal
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    // OnEnable tidak lagi digunakan untuk animasi

    /// <summary>
    /// Panggil method ini untuk MEMBUKA panel dengan animasi.
    /// </summary>
    public void OpenWithAnimation() {
        // Hentikan animasi yang mungkin masih berjalan
        rectTransform.DOKill();

        // 1. Jadikan panel terlihat dan bisa di-klik
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // 2. Set skala awal
        rectTransform.localScale = Vector3.one * startScale;

        // 3. Mainkan animasi skala membesar
        rectTransform.DOScale(endScale, animationDuration)
            .SetEase(animationEase)
            .SetUpdate(true);
    }

    /// <summary>
    /// Panggil method ini untuk MENUTUP panel dengan animasi.
    /// </summary>
    public void CloseWithAnimation() {
        // Hentikan animasi yang mungkin masih berjalan
        rectTransform.DOKill();

        // Mainkan animasi skala mengecil
        rectTransform.DOScale(startScale, animationDuration)
            .SetEase(animationEase)
            .SetUpdate(true)
            // Setelah animasi selesai, set alpha ke 0
            .OnComplete(() => {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            });
    }
}