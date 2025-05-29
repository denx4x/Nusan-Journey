using UnityEngine;

public class LightTrigger : MonoBehaviour {
    private MirrorManager mirrorManager;
    public string ownerMirrorID;

    void Start() {
        mirrorManager = FindObjectOfType<MirrorManager>();
        if (mirrorManager == null) {
            Debug.LogError("LightTrigger tidak dapat menemukan MirrorManager di scene!", this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("LightTarget")) {
            if (mirrorManager != null) {
                // JANGAN langsung aktifkan, tapi MULAI sekuens dengan delay.
                mirrorManager.BeginActivationSequence(ownerMirrorID);
            }
        }
    }

    /// <summary>
    /// FUNGSI BARU: Dipanggil ketika collider cahaya keluar dari collider target.
    /// </summary>
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("LightTarget")) {
            if (mirrorManager != null) {
                // BATALKAN sekuens aktivasi jika cahaya dipindahkan.
                mirrorManager.CancelActivationSequence(ownerMirrorID);
            }
        }
    }
}