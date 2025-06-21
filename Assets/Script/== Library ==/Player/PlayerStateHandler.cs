using UnityEngine;

/// <summary>
/// Skrip ini berfungsi sebagai pusat untuk semua state atau kondisi pemain.
/// Skrip lain (seperti PlayerMovement atau GrabbableObject) akan berkomunikasi
/// dengan skrip ini untuk mengetahui atau mengubah state pemain.
/// </summary>
public class PlayerStateHandler : MonoBehaviour {
    // Properti publik untuk mengecek apakah player sedang mendorong/membawa objek.
    // 'set' bisa private agar hanya skrip ini yang bisa mengubahnya dari dalam,
    // atau public jika Anda ingin skrip lain bisa mengubahnya. Kita buat public untuk fleksibilitas.
    public bool IsPushing { get; set; }

    // Di masa depan, Anda bisa menambahkan state lain di sini:
    // public bool IsCrouching { get; set; }
    // public bool IsStunned { get; set; }
    // dll.

    private void Awake() {
        // Pastikan state awal selalu false.
        IsPushing = false;
    }
}