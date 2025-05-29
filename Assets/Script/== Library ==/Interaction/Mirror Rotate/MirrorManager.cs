using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class untuk menyimpan data dan state dari setiap objek cermin.
/// </summary>
[System.Serializable]
public class MirrorObject {
    [Tooltip("Identifier unik untuk cermin ini, contoh: 'Mirror1'.")]
    public string mirrorID = "";

    [Tooltip("GameObject cermin yang akan dirotasi.")]
    public GameObject Mirror;

    [Tooltip("Daftar target rotasi (dalam Euler angles) yang akan dilalui cermin.")]
    public List<Vector3> inputRotation = new List<Vector3>();

    [Tooltip("Jika dicentang, cermin ini dapat memicu aktivasi objek.")]
    public bool triggerLightEvent = false;

    // --- PERUBAHAN DI SINI ---
    [Tooltip("Jeda waktu (detik) spesifik untuk cermin ini dari saat cahaya mengenai target hingga objek aktif.")]
    public float activationDelay = 2f;

    [Tooltip("Objek-objek yang akan diaktifkan ketika event trigger terpenuhi.")]
    public List<GameObject> objectsToActivate = new List<GameObject>();

    // Variabel internal untuk melacak status rotasi
    [HideInInspector] public int currentRotationIndex = -1;
    [HideInInspector] public bool isForward = true;
}

/// <summary>
/// Mengelola semua interaksi, rotasi, dan event dari seluruh cermin di scene.
/// </summary>
public class MirrorManager : MonoBehaviour {
    #region Public Variables
    [Tooltip("Daftar semua objek cermin yang diatur melalui Inspector.")]
    public List<MirrorObject> mirrorObjects = new List<MirrorObject>();

    [Header("Rotation Settings")]
    [Tooltip("Durasi animasi rotasi cermin dalam detik.")]
    public float rotationDuration = 1f;
    [Tooltip("Jeda waktu setelah rotasi sebelum interaksi berikutnya bisa dilakukan.")]
    public float interactionCooldown = 0.5f;

    // --- PERUBAHAN DI SINI ---
    // Variabel delay global sudah dihapus dari sini.
    #endregion

    #region Private State
    private string activeMirrorID = "";
    private bool isRotating = false;
    private Dictionary<string, MirrorObject> mirrorMap;
    private Dictionary<string, Coroutine> runningActivationRoutines;
    #endregion

    #region Unity Lifecycle
    private void Awake() {
        mirrorMap = new Dictionary<string, MirrorObject>();
        runningActivationRoutines = new Dictionary<string, Coroutine>();

        foreach (var mirrorObj in mirrorObjects) {
            if (!string.IsNullOrEmpty(mirrorObj.mirrorID) && !mirrorMap.ContainsKey(mirrorObj.mirrorID)) {
                mirrorMap.Add(mirrorObj.mirrorID, mirrorObj);
            } else {
                Debug.LogWarning($"Mirror ID duplikat atau kosong ditemukan: '{mirrorObj.mirrorID}'. Harap perbaiki.", this);
            }
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E) && !isRotating && !string.IsNullOrEmpty(activeMirrorID)) {
            if (mirrorMap.TryGetValue(activeMirrorID, out MirrorObject activeMirror)) {
                if (activeMirror.inputRotation.Count == 0 || activeMirror.Mirror == null)
                    return;

                int nextIndex = GetNextRotationIndex(activeMirror);

                if (nextIndex >= 0 && nextIndex < activeMirror.inputRotation.Count) {
                    Debug.Log($"Mirror '{activeMirror.mirrorID}' akan berotasi ke indeks: {nextIndex}");
                    activeMirror.currentRotationIndex = nextIndex;
                    StartCoroutine(RotateTo(activeMirror.Mirror.transform, activeMirror.inputRotation[nextIndex]));
                } else {
                    Debug.LogWarning($"Gagal mendapatkan indeks rotasi yang valid untuk Mirror '{activeMirror.mirrorID}'.");
                }
            }
        }
    }
    #endregion

    #region Rotation Logic
    private int GetNextRotationIndex(MirrorObject mirrorObj) {
        int nextIndex = mirrorObj.currentRotationIndex < 0 ? 0 : (mirrorObj.isForward ? mirrorObj.currentRotationIndex + 1 : mirrorObj.currentRotationIndex - 1);

        while (nextIndex >= 0 && nextIndex < mirrorObj.inputRotation.Count &&
               IsSameRotation(mirrorObj.Mirror.transform.rotation, mirrorObj.inputRotation[nextIndex])) {
            nextIndex = mirrorObj.isForward ? nextIndex + 1 : nextIndex - 1;
            Debug.Log($"Rotasi sama terdeteksi, skipping ke indeks: {nextIndex}");
        }

        if (nextIndex >= mirrorObj.inputRotation.Count) {
            mirrorObj.isForward = false;
            nextIndex = mirrorObj.inputRotation.Count > 1 ? mirrorObj.inputRotation.Count - 2 : 0;
        } else if (nextIndex < 0) {
            mirrorObj.isForward = true;
            nextIndex = mirrorObj.inputRotation.Count > 1 ? 1 : 0;
        }
        return nextIndex;
    }

    private bool IsSameRotation(Quaternion currentRotation, Vector3 targetEuler) {
        return Quaternion.Angle(currentRotation, Quaternion.Euler(targetEuler)) < 1f;
    }

    private IEnumerator RotateTo(Transform target, Vector3 targetEuler) {
        isRotating = true;
        Quaternion startRot = target.rotation;
        Quaternion endRot = Quaternion.Euler(targetEuler);
        float elapsed = 0f;

        while (elapsed < rotationDuration) {
            elapsed += Time.deltaTime;
            target.rotation = Quaternion.Lerp(startRot, endRot, elapsed / rotationDuration);
            yield return null;
        }
        target.rotation = endRot;

        yield return new WaitForSeconds(interactionCooldown);
        isRotating = false;
    }
    #endregion

    #region Public API
    public void SetActiveMirror(string mirrorID) {
        activeMirrorID = mirrorID;
    }

    public void ClearActiveMirror(string mirrorID) {
        if (activeMirrorID == mirrorID) {
            activeMirrorID = "";
        }
    }

    // --- PERUBAHAN DI SINI ---
    public void BeginActivationSequence(string mirrorID) {
        if (runningActivationRoutines.ContainsKey(mirrorID)) return;

        if (mirrorMap.TryGetValue(mirrorID, out MirrorObject mirrorObj)) {
            // Menggunakan mirrorObj.activationDelay untuk log dan coroutine
            Debug.Log($"Memulai sekuens aktivasi untuk mirror '{mirrorID}' dengan delay {mirrorObj.activationDelay} detik.");
            Coroutine routine = StartCoroutine(ActivateWithDelay(mirrorObj));
            runningActivationRoutines.Add(mirrorID, routine);
        }
    }

    public void CancelActivationSequence(string mirrorID) {
        if (runningActivationRoutines.TryGetValue(mirrorID, out Coroutine routine)) {
            Debug.Log($"Membatalkan sekuens aktivasi untuk mirror '{mirrorID}'.");
            StopCoroutine(routine);
            runningActivationRoutines.Remove(mirrorID);
        }
    }
    #endregion

    #region Private Coroutines
    // --- PERUBAHAN DI SINI ---
    private IEnumerator ActivateWithDelay(MirrorObject mirrorObj) {
        // Menggunakan delay dari objek cermin yang spesifik
        yield return new WaitForSeconds(mirrorObj.activationDelay);

        if (mirrorObj.triggerLightEvent) {
            Debug.Log($"Delay selesai! Mengaktifkan objek untuk mirror '{mirrorObj.mirrorID}'.");
            foreach (GameObject obj in mirrorObj.objectsToActivate) {
                if (obj != null && !obj.activeSelf) {
                    obj.SetActive(true);
                }
            }
        }

        runningActivationRoutines.Remove(mirrorObj.mirrorID);
    }
    #endregion
}