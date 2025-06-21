// File: DialogueSignalReceiver.cs

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

// Kelas kecil ini berfungsi sebagai "wadah" untuk memasangkan 
// sebuah nama sinyal dengan aksi (UnityEvent) di Inspector.
[System.Serializable]
public class SignalEventMapping {
    [Tooltip("Nama sinyal yang harus diketik sama persis dengan yang ada di DialogueObject")]
    public string signalName;

    [Tooltip("Daftar aksi yang akan dijalankan ketika sinyal dengan nama di atas diterima")]
    public UnityEvent onSignalReceived;
}

/// <summary>
/// Menerima sinyal berupa string dari DialogueUI dan menjalankan UnityEvent yang terhubung.
/// Skrip ini harus diletakkan pada sebuah GameObject di dalam Scene.
/// </summary>
public class DialogueSignalReceiver : MonoBehaviour {
    [Header("Daftar Sinyal dan Aksi")]
    [Tooltip("Isi daftar ini dengan semua kemungkinan sinyal dan aksi yang ingin Anda tangani.")]
    public List<SignalEventMapping> signalMappings;

    // Dictionary digunakan saat runtime untuk pencarian sinyal yang lebih cepat dan efisien.
    private Dictionary<string, UnityEvent> signalDictionary;

    private void Awake() {
        // Mengubah List dari Inspector menjadi Dictionary untuk performa optimal.
        signalDictionary = new Dictionary<string, UnityEvent>();
        foreach (var mapping in signalMappings) {
            // Cek untuk menghindari nama sinyal yang duplikat.
            if (!signalDictionary.ContainsKey(mapping.signalName)) {
                signalDictionary.Add(mapping.signalName, mapping.onSignalReceived);
            } else {
                Debug.LogWarning($"Sinyal '{mapping.signalName}' terdeteksi lebih dari satu kali di {gameObject.name}. Hanya yang pertama yang akan digunakan.", this);
            }
        }
    }

    /// <summary>
    /// Metode publik yang dipanggil oleh DialogueUI untuk memproses sinyal.
    /// </summary>
    /// <param name="signal">Nama sinyal yang dikirim dari DialogueObject.</param>
    public void ReceiveSignal(string signal) {
        // Jangan proses jika sinyal yang dikirim kosong.
        if (string.IsNullOrWhiteSpace(signal)) return;

        // Cari event yang cocok di Dictionary, lalu panggil (invoke) jika ada.
        if (signalDictionary.TryGetValue(signal, out UnityEvent eventToInvoke)) {
            Debug.Log($"Sinyal '{signal}' diterima. Menjalankan UnityEvent yang terhubung.");
            eventToInvoke?.Invoke();
        } else {
            // Beri peringatan jika sinyal diterima tapi tidak ada aksi yang cocok.
            // Ini membantu debugging jika ada salah ketik.
            Debug.LogWarning($"Sinyal '{signal}' diterima, tapi tidak ada aksi yang terhubung di {gameObject.name}.", this);
        }
    }
}