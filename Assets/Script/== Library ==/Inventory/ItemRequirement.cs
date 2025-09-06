using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Memeriksa apakah pemain memiliki item yang dibutuhkan saat berinteraksi.
/// Jika ya, jalankan event onRequirementMet. Jika tidak, jalankan onRequirementFailed.
/// </summary>
[RequireComponent(typeof(Collider))] // Memastikan objek punya Collider
public class ItemRequirement : MonoBehaviour {
    [Header("Pengaturan Persyaratan")]
    [Tooltip("Nama item yang dibutuhkan. Harus sama persis dengan 'Item Name' pada skrip CollectibleItem.")]
    [SerializeField] private string requiredItemName = "Key";

    [Tooltip("Jika dicentang, item akan hilang dari inventaris setelah berhasil digunakan.")]
    [SerializeField] private bool consumeItemOnUse = false;

    [Tooltip("Jika dicentang, interaksi ini akan dinonaktifkan setelah berhasil digunakan satu kali.")]
    [SerializeField] private bool disableAfterSuccess = true;

    [Header("Events")]
    [Tooltip("Event yang dijalankan jika pemain MEMILIKI item yang dibutuhkan.")]
    public UnityEvent onRequirementMet;

    [Tooltip("Event yang dijalankan jika pemain TIDAK MEMILIKI item yang dibutuhkan.")]
    public UnityEvent onRequirementFailed;

    private bool isPlayerInRange = false;

    private void Awake() {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = true;
            Debug.Log($"Pemain masuk jangkauan '{gameObject.name}'. Tekan 'E' untuk mencoba.");
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
        }
    }

    private void Update() {
        // Jika pemain tidak di dalam jangkauan, jangan lakukan apa-apa
        if (!isPlayerInRange) return;

        // Jika pemain menekan tombol 'E'
        if (Input.GetKeyDown(KeyCode.E)) {
            // Cari komponen PlayerInventory di scene
            PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
            if (inventory == null) {
                Debug.LogError("Tidak ada PlayerInventory di scene!");
                return;
            }

            // Periksa apakah pemain punya item yang dibutuhkan
            if (inventory.HasItem(requiredItemName)) {
                Debug.Log($"Syarat terpenuhi! Pemain memiliki '{requiredItemName}'.");
                onRequirementMet?.Invoke(); // Jalankan event sukses

                if (consumeItemOnUse) {
                    inventory.RemoveItem(requiredItemName);
                }
                if (disableAfterSuccess) {
                    this.enabled = false; // Nonaktifkan skrip ini agar tidak bisa dipakai lagi
                }
            } else {
                Debug.Log($"Syarat GAGAL! Pemain tidak memiliki '{requiredItemName}'.");
                onRequirementFailed?.Invoke(); // Jalankan event gagal
            }
        }
    }
}