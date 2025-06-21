using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Mengelola semua interaksi trigger untuk Player dari satu tempat terpusat.
/// Skrip ini harus ditempatkan pada GameObject Player.
/// Player wajib memiliki Rigidbody dan sebuah Collider.
/// </summary>
public class PlayerTriggerManager : MonoBehaviour {
    // Enum untuk tipe trigger. 'Custom' telah diganti dengan 'Fade'.
    public enum TriggerActionType {
        Win,
        Dead,
        Camera,
        Clue,
        Fade // Diganti dari Custom
    }

    /// <summary>
    /// Struktur data untuk menampung konfigurasi setiap trigger.
    /// Ini akan muncul sebagai daftar yang bisa diisi di Inspector.
    /// </summary>
    [System.Serializable]
    public class TriggerSetup {
        [Tooltip("Nama deskriptif untuk trigger ini (hanya untuk memudahkan identifikasi di Inspector).")]
        public string description;

        [Tooltip("Geser (drag) Collider dari GameObject trigger ke sini.")]
        public Collider triggerCollider;

        // === PERUBAHAN DI SINI ===
        [Tooltip("Pilih jenis aksi yang akan terjadi.")]
        public TriggerActionType actionType = TriggerActionType.Fade; // Default diubah ke Fade
        // ==========================

        [Tooltip("Event yang akan dipanggil saat pemain MASUK ke trigger ini.")]
        public UnityEvent onPlayerEnter;

        [Tooltip("Event yang akan dipanggil saat pemain KELUAR dari trigger ini.")]
        public UnityEvent onPlayerExit;
    }

    [Header("Daftar Trigger yang Dikelola")]
    [Tooltip("Masukkan semua collider yang ingin dijadikan trigger ke dalam daftar ini.")]
    public List<TriggerSetup> managedTriggers = new List<TriggerSetup>();

    // Dictionary untuk pencarian yang lebih cepat
    private Dictionary<Collider, TriggerSetup> triggerMap;

    private void Awake() {
        triggerMap = new Dictionary<Collider, TriggerSetup>();
        foreach (var trigger in managedTriggers) {
            if (trigger.triggerCollider != null && !triggerMap.ContainsKey(trigger.triggerCollider)) {
                triggerMap.Add(trigger.triggerCollider, trigger);
            } else {
                Debug.LogWarning($"Peringatan: Trigger '{trigger.description}' memiliki collider yang kosong atau duplikat dan tidak akan berfungsi.", this);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (triggerMap.TryGetValue(other, out TriggerSetup setup)) {
            Debug.Log($"Pemain MASUK ke trigger: '{setup.description}' dengan tipe '{setup.actionType}'");
            setup.onPlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (triggerMap.TryGetValue(other, out TriggerSetup setup)) {
            Debug.Log($"Pemain KELUAR dari trigger: '{setup.description}' dengan tipe '{setup.actionType}'");
            setup.onPlayerExit?.Invoke();
        }
    }

    // --- Contoh Fungsi yang Bisa Dipanggil dari UnityEvent ---

    public void LoadSceneByIndex(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }

    public void RestartCurrentScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}