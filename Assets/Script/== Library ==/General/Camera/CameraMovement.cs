using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    [Header("Object To Move")]
    [Tooltip("Jika aktif, akan menggerakkan GameObject tempat skrip ini terpasang.")]
    public bool useCurrentObject = true;

    [Tooltip("Tentukan GameObject yang ingin digerakkan. Abaikan jika 'Use Current Object' aktif.")]
    public Transform objectToMove;

    [Header("Movement Settings")]
    [Tooltip("Kecepatan pergerakan dalam satuan unit per detik.")]
    public float transitionSpeed = 2.0f;

    [Tooltip("Pilih untuk menggunakan target Transform sebagai tujuan.")]
    public bool useTransformTarget;

    [Header("Manual Target")]
    [Tooltip("Tentukan apakah nilai di bawah menggunakan koordinat Lokal (relative to parent) atau Global (World).")]
    public bool useLocalCoordinates;

    [Tooltip("Posisi tujuan jika tidak menggunakan target Transform.")]
    public Vector3 manualTargetPosition;
    [Tooltip("Rotasi tujuan (dalam Euler Angles) jika tidak menggunakan target Transform.")]
    public Vector3 manualTargetRotation;

    [Header("Transform Target")]
    [Tooltip("GameObject yang menjadi tujuan posisi dan rotasi kamera.")]
    public Transform targetTransform;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool initialPositionStored = false;
    private Coroutine _moveCoroutine;

    private Transform MovingObjectTransform {
        get {
            if (useCurrentObject) return this.transform;
            if (objectToMove == null) {
                Debug.LogError("Pilih 'Use Current Object' atau assign sebuah Transform ke field 'Object To Move'.", this);
                return null;
            }
            return objectToMove;
        }
    }

    public void MoveCameraToTarget() {
        Transform movingObject = MovingObjectTransform;
        if (movingObject == null) return;

        if (!initialPositionStored) {
            initialPosition = movingObject.position;
            initialRotation = movingObject.rotation;
            initialPositionStored = true;
        }

        Vector3 targetPos;
        Quaternion targetRot;

        if (useTransformTarget) {
            if (targetTransform == null) {
                Debug.LogError("Mode 'Use Transform Target' aktif, tetapi 'Target Transform' belum diisi!", this);
                return;
            }
            targetPos = targetTransform.position;
            targetRot = targetTransform.rotation;
        } else {
            if (useLocalCoordinates) {
                if (movingObject.parent != null) {
                    targetPos = movingObject.parent.TransformPoint(manualTargetPosition);
                    targetRot = movingObject.parent.rotation * Quaternion.Euler(manualTargetRotation);
                } else {
                    targetPos = manualTargetPosition;
                    targetRot = Quaternion.Euler(manualTargetRotation);
                }
            } else {
                targetPos = manualTargetPosition;
                targetRot = Quaternion.Euler(manualTargetRotation);
            }
        }

        StartTransition(targetPos, targetRot);
    }

    public void ReturnToInitialPosition() {
        if (initialPositionStored) {
            StartTransition(initialPosition, initialRotation);
        } else {
            Debug.LogWarning("Objek belum pernah bergerak, tidak ada posisi awal untuk kembali.", this);
        }
    }

    private void StartTransition(Vector3 targetPosition, Quaternion targetRotation) {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(TransitionCoroutine(targetPosition, targetRotation));
    }

    // --- FUNGSI INI TELAH DIPERBAIKI ---
    private IEnumerator TransitionCoroutine(Vector3 targetPos, Quaternion targetRot) {
        Transform movingObject = MovingObjectTransform;
        if (movingObject == null) yield break;

        Vector3 startPosition = movingObject.position;
        Quaternion startRotation = movingObject.rotation;
        float journeyTime = 0f;

        // Hitung durasi perjalanan berdasarkan jarak dan kecepatan
        float journeyDistance = Vector3.Distance(startPosition, targetPos);
        float duration = 1f; // Durasi default jika kecepatan tidak valid atau jarak 0

        if (transitionSpeed > 0 && journeyDistance > 0) {
            duration = journeyDistance / transitionSpeed; // Durasi = Jarak / Kecepatan
        }

        while (journeyTime < duration) {
            // Tambahkan waktu yang telah berlalu sejak frame terakhir
            journeyTime += Time.deltaTime;

            // Hitung persentase perjalanan yang telah selesai (nilai dari 0 ke 1)
            float percentComplete = journeyTime / duration;

            // Lakukan interpolasi menggunakan persentase yang benar
            movingObject.position = Vector3.Lerp(startPosition, targetPos, percentComplete);
            movingObject.rotation = Quaternion.Slerp(startRotation, targetRot, percentComplete);

            yield return null;
        }

        // Pastikan objek sampai tepat di posisi & rotasi akhir
        movingObject.position = targetPos;
        movingObject.rotation = targetRot;
        _moveCoroutine = null;
    }
}