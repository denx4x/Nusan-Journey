using UnityEngine;
using UnityEngine.Events; // <-- Jangan lupa tambahkan ini untuk UnityEvent

public class TriggerCounterActivator : MonoBehaviour {
    [Header("Pengaturan Target")]
    [Tooltip("Jumlah trigger yang dibutuhkan untuk menjalankan event.")]
    [SerializeField]
    private int requiredTriggerCount = 3;

    // --- PERUBAHAN 1: Ganti GameObject dengan UnityEvent ---
    [Tooltip("Event yang akan dijalankan SEKALI saat jumlah trigger tercapai.")]
    public UnityEvent OnTargetReached;

    [Tooltip("Event yang akan dijalankan SEKALI saat jumlah trigger tidak lagi tercapai.")]
    public UnityEvent OnTargetLost;
    // ----------------------------------------------------

    [Header("Status Saat Ini (Untuk Debugging)")]
    [Tooltip("Jumlah objek yang saat ini berada di dalam trigger.")]
    [SerializeField]
    private int currentTriggerCount = 0;

    // Variabel untuk melacak status agar event tidak dijalankan berkali-kali
    private bool isTargetReached = false;

    // --- PERUBAHAN 2: Fungsi Start() dihapus ---
    // Kita tidak lagi perlu menonaktifkan objek spesifik di awal.
    // void Start() { ... }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("LightCount")) {
            currentTriggerCount++;
            Debug.Log("Objek dengan tag 'LightCount' masuk. Jumlah saat ini: " + currentTriggerCount);
            CheckActivationState();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("LightCount")) {
            currentTriggerCount--;
            if (currentTriggerCount < 0) {
                currentTriggerCount = 0;
            }
            Debug.Log("Objek dengan tag 'LightCount' keluar. Jumlah saat ini: " + currentTriggerCount);
            CheckActivationState();
        }
    }

    // --- PERUBAHAN 3: Logika CheckActivationState diubah total ---
    private void CheckActivationState() {
        // Kondisi target tercapai
        if (currentTriggerCount >= requiredTriggerCount) {
            // Jika target belum tercapai sebelumnya, jalankan event OnTargetReached
            if (!isTargetReached) {
                Debug.Log("TARGET TERCAPAI! Menjalankan OnTargetReached.");
                OnTargetReached?.Invoke(); // Tanda '?' mencegah error jika event kosong
                isTargetReached = true; // Tandai bahwa target sudah tercapai
            }
        }
        // Kondisi target tidak lagi tercapai
        else {
            // Jika sebelumnya target tercapai, jalankan event OnTargetLost
            if (isTargetReached) {
                Debug.Log("TARGET HILANG! Menjalankan OnTargetLost.");
                OnTargetLost?.Invoke();
                isTargetReached = false; // Tandai bahwa target sudah tidak tercapai
            }
        }
    }
}