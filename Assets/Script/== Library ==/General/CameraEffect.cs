using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class CameraEffect : MonoBehaviour {
    public enum StartEffectType { None, Shake, Punch, Zoom }

    [Header("Pengaturan Awal (On Start)")]
    [Tooltip("Pilih efek yang akan otomatis berjalan saat game dimulai. Pilih 'None' jika tidak ada.")]
    public StartEffectType effectToPlayOnStart = StartEffectType.None;

    [Header("Pengaturan Efek Getaran (Shake)")]
    public float shakeDuration = 0.5f;
    public float shakeStrength = 3f;
    public int shakeVibrato = 10;

    [Header("Pengaturan Efek Hentakan (Punch/Thrust)")]
    public float punchDistance = -0.5f;
    public float punchDuration = 0.3f;
    public int punchVibrato = 5;

    [Header("Pengaturan Efek Zoom (Field of View)")]
    public float zoomFOVValue = 50f;
    public float zoomDuration = 0.5f;

    private Camera mainCamera;
    private Transform cameraTransform;
    private Vector3 initialPosition;
    private float initialFOV;

    void Awake() {
        mainCamera = GetComponent<Camera>();
        cameraTransform = transform;
    }

    void Start() {
        initialPosition = cameraTransform.position;
        initialFOV = mainCamera.fieldOfView;

        if (effectToPlayOnStart != StartEffectType.None) {
            switch (effectToPlayOnStart) {
                case StartEffectType.Shake:
                    PlayShake(true);
                    break;
                case StartEffectType.Punch:
                    PlayPunch(true);
                    break;
                case StartEffectType.Zoom:
                    PlayZoom(true);
                    break;
            }
        }
    }

    // Pengecekan 'enable' sudah dihapus dari semua fungsi Play...
    [ContextMenu("Test Play Shake")]
    public void PlayShake(bool loop = false) {
        StopAllEffects();
        cameraTransform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato)
            .SetLoops(loop ? -1 : 0, LoopType.Restart);
    }

    [ContextMenu("Test Play Punch")]
    public void PlayPunch(bool loop = false) {
        StopAllEffects();
        cameraTransform.DOPunchPosition(new Vector3(0, 0, punchDistance), punchDuration, punchVibrato)
             .SetLoops(loop ? -1 : 0, LoopType.Restart);
    }

    [ContextMenu("Test Play Zoom")]
    public void PlayZoom(bool loop = false) {
        StopAllEffects();
        mainCamera.DOFieldOfView(zoomFOVValue, zoomDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(loop ? -1 : 0, LoopType.Yoyo);
    }

    public void StopAllEffects() {
        cameraTransform.DOKill();
        mainCamera.DOKill();
        cameraTransform.position = initialPosition;
        mainCamera.fieldOfView = initialFOV;
    }
}