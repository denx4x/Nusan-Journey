using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Events; // <-- 1. Pastikan namespace ini ada

/// <summary>
/// Mengontrol animasi untuk memunculkan dan menyembunyikan sebuah panel UI.
/// Didesain untuk bisa digunakan kembali untuk panel UI mana pun.
/// Versi ini sudah mendukung Canvas World Space dan Screen Space.
/// </summary>
public class UIArea : MonoBehaviour {
    [Header("UI Element to Control")]
    [Tooltip("Panel atau container utama UI yang ingin dianimasikan.")]
    [SerializeField] private GameObject uiContainer;

    [Header("Behavior Settings")]
    [SerializeField] private bool showOnStart = false;
    [SerializeField] private bool autoHide = false;
    [SerializeField] private float autoHideDelay = 3f;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.4f;
    [SerializeField] private Ease easeTypeShow = Ease.OutBack;
    [SerializeField] private Ease easeTypeHide = Ease.InBack;

    // --- PERUBAHAN 1: Tambahkan UnityEvent baru ---
    [Header("Events")]
    [Tooltip("Event yang akan dipanggil setelah animasi HideUI selesai sepenuhnya.")]
    public UnityEvent OnHideComplete;
    // ---------------------------------------------

    // Komponen yang diperlukan untuk animasi
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isOpen = false;

    private Vector3 originalScale;
    private Coroutine autoHideCoroutine;

    private void Awake() {
        if (uiContainer == null) {
            Debug.LogError("MASALAH: UI Container belum di-assign pada script UIArea!", this.gameObject);
            return;
        }

        rectTransform = uiContainer.GetComponent<RectTransform>();
        canvasGroup = uiContainer.GetComponent<CanvasGroup>();

        if (canvasGroup == null) {
            canvasGroup = uiContainer.AddComponent<CanvasGroup>();
        }

        originalScale = rectTransform.localScale;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        isOpen = false;
    }

    private void Start() {
        if (showOnStart) {
            ShowUI();
        }
    }

    public void ShowUI() {
        if (isOpen) return;
        isOpen = true;

        if (autoHideCoroutine != null) {
            StopCoroutine(autoHideCoroutine);
        }

        rectTransform.DOKill();
        canvasGroup.DOKill();

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        rectTransform.localScale = originalScale * 0.9f;

        Sequence showSequence = DOTween.Sequence();
        showSequence.Join(rectTransform.DOScale(originalScale, animationDuration).SetEase(easeTypeShow));
        showSequence.Join(canvasGroup.DOFade(1f, animationDuration * 0.8f));

        showSequence.OnComplete(() => {
            if (autoHide) {
                autoHideCoroutine = StartCoroutine(AutoHideCoroutine());
            }
        });
    }

    public void HideUI() {
        if (!isOpen) return;
        isOpen = false;

        if (autoHideCoroutine != null) {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }

        rectTransform.DOKill();
        canvasGroup.DOKill();

        Sequence hideSequence = DOTween.Sequence();
        hideSequence.Join(rectTransform.DOScale(originalScale * 0.9f, animationDuration).SetEase(easeTypeHide));
        hideSequence.Join(canvasGroup.DOFade(0f, animationDuration * 0.8f));
        hideSequence.OnComplete(() => {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // --- PERUBAHAN 2: Panggil event di sini! ---
            // Event ini akan terpanggil tepat setelah UI selesai disembunyikan.
            OnHideComplete?.Invoke();
            // ------------------------------------------
        });
    }

    private IEnumerator AutoHideCoroutine() {
        yield return new WaitForSeconds(autoHideDelay);
        HideUI();
        autoHideCoroutine = null;
    }
}