using UnityEngine;
using DG.Tweening;

/// <summary>
/// Memberikan animasi idle (diam) pada elemen UI agar terasa lebih hidup.
/// Efek yang bisa digunakan: berdenyut (skala), bergoyang (rotasi), dan mengambang (posisi).
/// </summary>
public class UIBehaviour : MonoBehaviour {
    [Header("Pengaturan Utama")]
    [Tooltip("Aktifkan untuk memulai semua animasi idle.")]
    public bool enableIdleAnimation = true;

    [Header("Efek Berdenyut (Pulse)")]
    public bool enablePulse = true;
    [Tooltip("Seberapa besar skala akan membesar dari ukuran aslinya.")]
    public float pulseScale = 1.05f;
    [Tooltip("Durasi untuk satu siklus animasi (membesar lalu mengecil).")]
    public float pulseDuration = 2.0f;

    [Header("Efek Bergoyang (Wobble)")]
    public bool enableWobble = false;
    [Tooltip("Sudut maksimal goyangan ke kiri dan kanan.")]
    public float wobbleAngle = 5.0f;
    [Tooltip("Durasi untuk satu siklus animasi (goyang ke satu sisi lalu kembali).")]
    public float wobbleDuration = 1.5f;

    [Header("Efek Mengambang (Float)")]
    public bool enableFloat = false;
    [Tooltip("Jarak pergerakan naik-turun dari posisi awal.")]
    public float floatDistance = 10.0f;
    [Tooltip("Durasi untuk satu siklus animasi (naik lalu turun).")]
    public float floatDuration = 3.0f;

    private RectTransform rectTransform;
    private Vector3 initialPosition;
    private Vector3 initialScale;

    void Start() {
        if (!enableIdleAnimation) return;

        rectTransform = GetComponent<RectTransform>();

        // Simpan state awal dari UI
        initialPosition = rectTransform.anchoredPosition;
        initialScale = rectTransform.localScale;

        // Memulai animasi yang diaktifkan
        if (enablePulse) {
            AnimatePulse();
        }
        if (enableWobble) {
            AnimateWobble();
        }
        if (enableFloat) {
            AnimateFloat();
        }
    }

    private void AnimatePulse() {
        // Membuat animasi skala membesar dan mengecil secara terus-menerus
        rectTransform.DOScale(initialScale * pulseScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Looping selamanya (Yoyo = bolak-balik)
    }

    private void AnimateWobble() {
        // Membuat animasi rotasi bergoyang ke kiri dan kanan
        rectTransform.DORotate(new Vector3(0, 0, wobbleAngle), wobbleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void AnimateFloat() {
        // Membuat animasi posisi bergerak naik-turun
        rectTransform.DOAnchorPos(initialPosition + new Vector3(0, floatDistance, 0), floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    // Penting untuk menghentikan animasi saat objek dihancurkan untuk mencegah error
    void OnDestroy() {
        if (rectTransform != null) {
            rectTransform.DOKill();
        }
    }
}