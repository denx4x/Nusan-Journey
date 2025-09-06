using UnityEngine;

public class UIDirectLink : MonoBehaviour {
    [Tooltip("Masukkan URL lengkap yang ingin dibuka, contoh: https://www.google.com")]
    public string urlToOpen = "https://www.google.com";

    /// <summary>
    /// Fungsi ini akan dipanggil oleh event OnClick pada tombol.
    /// </summary>
    public void OpenLink() {
        // Cek jika URL tidak kosong sebelum mencoba membukanya
        if (!string.IsNullOrEmpty(urlToOpen)) {
            // Perintah utama untuk membuka URL di browser default
            Application.OpenURL(urlToOpen);
            Debug.Log("Membuka URL: " + urlToOpen);
        } else {
            Debug.LogWarning("URL pada objek ini kosong!");
        }
    }
}