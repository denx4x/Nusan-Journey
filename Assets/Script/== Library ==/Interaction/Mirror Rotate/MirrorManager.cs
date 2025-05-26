using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MirrorObject {
    // List untuk menyimpan input rotasi (Euler angles)
    public List<Vector3> inputRotation = new List<Vector3>();

    // Variabel untuk penggunaan di masa mendatang.
    public Vector3 targetRotation = Vector3.zero;

    // GameObject yang menentukan posisi Light dan Target Light untuk pemeriksaan "touching"
    public GameObject gameObjectLight;
    public GameObject gameObjectTargetLight;

    // GameObject Mirror yang akan dirotasi oleh script.
    public GameObject Mirror;

    // Identifier agar hanya mirror tertentu yang merespon interaksi.
    public string mirrorID = "";

    // State internal untuk melacak indeks rotasi saat ini dan mekanisme ping-pong (forward/mundur)
    [HideInInspector] public int currentRotationIndex = -1;
    [HideInInspector] public bool isForward = true;

    // Toggle optional event: jika true, event aktivasi akan dijalankan.
    public bool triggerLightEvent = false;

    // List objek yang akan diaktifkan ketika kondisi event terpenuhi.
    public List<GameObject> objectsToActivate = new List<GameObject>();
}

public class MirrorManager : MonoBehaviour {
    // Daftar MirrorObject diisi melalui Inspector.
    public List<MirrorObject> mirrorObjects = new List<MirrorObject>();

    // Durasi rotasi dan jeda antar interaksi.
    public float rotationDuration = 1f;
    public float interactionCooldown = 0.5f;

    // Active Mirror ID, di-set melalui trigger atau mekanisme lain.
    public string activeMirrorID = "";

    // Flag untuk mencegah input saat sedang dalam proses rotasi.
    private bool isRotating = false;

    // Threshold untuk menentukan apakah gameObjectLight "menyentuh" gameObjectTargetLight.
    public float lightTouchThreshold = 0.5f; // Nilai ini dapat disesuaikan

    void Update() {
        if (Input.GetKeyDown(KeyCode.E) && !isRotating) {
            foreach (MirrorObject mirrorObj in mirrorObjects) {
                if (mirrorObj.mirrorID == activeMirrorID) {
                    if (mirrorObj.inputRotation.Count == 0 || mirrorObj.Mirror == null)
                        continue;

                    int nextIndex = GetNextRotationIndex(mirrorObj);
                    Debug.Log("Mirror " + mirrorObj.mirrorID + " rotasi dari indeks "
                              + mirrorObj.currentRotationIndex + " ke " + nextIndex);

                    mirrorObj.currentRotationIndex = nextIndex;
                    StartCoroutine(RotateTo(mirrorObj, mirrorObj.inputRotation[nextIndex], mirrorObj.Mirror.transform));
                    break;
                }
            }
        }
    }

    int GetNextRotationIndex(MirrorObject mirrorObj) {
        int nextIndex = mirrorObj.currentRotationIndex < 0
                            ? 0
                            : (mirrorObj.isForward ? mirrorObj.currentRotationIndex + 1 : mirrorObj.currentRotationIndex - 1);

        // Skip indeks jika rotasi saat ini (transform) sudah sama dengan inputRotation pada indeks tersebut.
        while (nextIndex >= 0 && nextIndex < mirrorObj.inputRotation.Count &&
               IsSameRotation(mirrorObj.Mirror.transform.rotation, mirrorObj.inputRotation[nextIndex])) {
            nextIndex = mirrorObj.isForward ? nextIndex + 1 : nextIndex - 1;
        }

        // Mekanisme ping-pong: jika nextIndex keluar dari batas, balik arah.
        if (nextIndex >= mirrorObj.inputRotation.Count) {
            nextIndex = mirrorObj.inputRotation.Count - 2; // geser ke elemen sebelum batas atas
            mirrorObj.isForward = false;
        } else if (nextIndex < 0) {
            nextIndex = 1; // geser ke elemen kedua jika melewati batas bawah
            mirrorObj.isForward = true;
        }

        return nextIndex;
    }

    bool IsSameRotation(Quaternion currentRotation, Vector3 targetEuler) {
        Quaternion targetRotation = Quaternion.Euler(targetEuler);
        // Jika perbedaan sudut kurang dari 1 derajat, anggap kedua rotasi sama.
        return Quaternion.Angle(currentRotation, targetRotation) < 1f;
    }

    IEnumerator RotateTo(MirrorObject mirrorObj, Vector3 targetEuler, Transform target) {
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

        // Jika event toggle aktif, periksa apakah gameObjectLight "menyentuh" gameObjectTargetLight.
        if (mirrorObj.triggerLightEvent && AreLightsTouching(mirrorObj)) {
            // Aktivasi semua object pada list objectsToActivate.
            foreach (GameObject obj in mirrorObj.objectsToActivate) {
                if (obj != null) {
                    obj.SetActive(true);
                }
            }
            Debug.Log("Light event triggered for mirror " + mirrorObj.mirrorID);
        }

        isRotating = false;
    }

    // Fungsi untuk mengecek apakah gameObjectLight menyentuh gameObjectTargetLight.
    bool AreLightsTouching(MirrorObject mirrorObj) {
        if (mirrorObj.gameObjectLight == null || mirrorObj.gameObjectTargetLight == null)
            return false;

        float distance = Vector3.Distance(
            mirrorObj.gameObjectLight.transform.position,
            mirrorObj.gameObjectTargetLight.transform.position
        );

        Debug.Log("Jarak antara gameObjectLight dan gameObjectTargetLight untuk mirror "
                  + mirrorObj.mirrorID + " adalah: " + distance +
                  " (threshold: " + lightTouchThreshold + ")");

        return distance <= lightTouchThreshold;
    }


    public void SetActiveMirror(string mirrorID) {
        activeMirrorID = mirrorID;
        Debug.Log("Active Mirror di-set menjadi: " + mirrorID);
    }

    public void ClearActiveMirror(string mirrorID) {
        if (activeMirrorID == mirrorID) {
            activeMirrorID = "";
            Debug.Log("Active Mirror dikosongkan untuk: " + mirrorID);
        }
    }
}
