using UnityEngine;

public class MirrorIDReference : MonoBehaviour {
    // Identifier yang mengikuti mirrorID yang ada pada MirrorObject.
    public string mirrorID = "";

    // Referensi ke MirrorManager, diinisialisasi saat Start.
    private MirrorManager mirrorManager;

    void Start() {
        mirrorManager = FindObjectOfType<MirrorManager>();
    }

    // Pastikan objek pemain memiliki tag "Player" dan rigidbody.
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            mirrorManager.SetActiveMirror(mirrorID);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            mirrorManager.ClearActiveMirror(mirrorID);
        }
    }
}
