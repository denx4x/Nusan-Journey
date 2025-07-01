using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections; // <-- WAJIB: Diperlukan untuk Coroutine
using System.Collections.Generic;

/// <summary>
/// Mengelola semua interaksi trigger untuk Player dari satu tempat terpusat.
/// Versi ini menggunakan asynchronous loading untuk perpindahan scene yang mulus.
/// </summary>
public class PlayerTriggerManager : MonoBehaviour {
    public enum TriggerActionType {
        Win,
        Dead,
        Camera,
        Clue,
        Fade
    }

    [System.Serializable]
    public class TriggerSetup {
        [Tooltip("Nama deskriptif untuk trigger ini (hanya untuk memudahkan identifikasi di Inspector).")]
        public string description;

        [Tooltip("Geser (drag) Collider dari GameObject trigger ke sini.")]
        public Collider triggerCollider;

        [Tooltip("Pilih jenis aksi yang akan terjadi.")]
        public TriggerActionType actionType = TriggerActionType.Fade;

        [Tooltip("Event yang akan dipanggil saat pemain MASUK ke trigger ini.")]
        public UnityEvent onPlayerEnter;

        [Tooltip("Event yang akan dipanggil saat pemain KELUAR dari trigger ini.")]
        public UnityEvent onPlayerExit;
    }

    [Header("Daftar Trigger yang Dikelola")]
    [Tooltip("Masukkan semua collider yang ingin dijadikan trigger ke dalam daftar ini.")]
    public List<TriggerSetup> managedTriggers = new List<TriggerSetup>();

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


    // --- FUNGSI ASYNCHRONOUS BARU ---
    // Gunakan fungsi-fungsi ini di UnityEvent Anda untuk loading yang mulus.

    /// <summary>
    /// Memuat scene berdasarkan build index secara asynchronous.
    /// </summary>
    public void LoadSceneAsyncByIndex(int sceneIndex) {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    /// <summary>
    /// Memuat ulang scene yang sedang aktif secara asynchronous.
    /// </summary>
    public void RestartCurrentSceneAsync() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadSceneCoroutine(currentSceneIndex));
    }

    /// <summary>
    /// Coroutine yang menjalankan proses loading di background.
    /// </summary>
    private IEnumerator LoadSceneCoroutine(int sceneIndex) {
        Debug.Log($"Mulai memuat scene index {sceneIndex} secara async...");

        // Opsional: Di sini Anda bisa memanggil FADE OUT sebelum loading dimulai
        // contoh: FindObjectOfType<Fading>().FadeOut();
        // yield return new WaitForSeconds(1f); // Tunggu fade selesai

        // Mulai proses loading di background
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        // Tunggu hingga proses loading selesai
        while (!asyncOperation.isDone) {
            // Di sini Anda bisa menampilkan progress bar jika perlu
            // float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            // Debug.Log("Loading progress: " + (progress * 100) + "%");
            yield return null; // Tunggu frame berikutnya
        }
    }

    // --- FUNGSI LAMA (Disarankan untuk tidak dipakai lagi) ---

    public void LoadSceneByIndex(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }

    public void RestartCurrentScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}