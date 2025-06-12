// File: SettingsManager.cs

using UnityEngine;

public class SettingsManager : MonoBehaviour {
    // Singleton pattern sederhana agar mudah diakses dari mana saja
    public static SettingsManager Instance { get; private set; }

    private void Awake() {
        // Pastikan hanya ada satu instance dari SettingsManager
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Jaga agar object ini tidak hancur saat ganti scene
        }
    }

    // --- FUNGSI PUBLIK YANG DIPANGGIL OLEH UI ---
    // Setiap fungsi menangani logika untuk satu setelan spesifik

    public void SetMasterVolume(float value) {
        // Ganti baris ini dengan logika audio Anda, contoh: AudioListener.volume = value;
        Debug.Log($"Setting Master Volume to: {value}");
        PlayerPrefs.SetFloat("settings_audio_mastervolume", value);
    }

    public void SetBrightness(float value) {
        // Ganti baris ini dengan logika brightness Anda, contoh: post-processing effect
        Debug.Log($"Setting Brightness to: {value}");
        PlayerPrefs.SetFloat("settings_video_brightness", value);
    }

    public void SetVsync(bool isOn) {
        // Logika untuk VSync sudah bawaan dari Unity
        QualitySettings.vSyncCount = isOn ? 1 : 0;
        Debug.Log($"Setting VSync to: {isOn}");
        PlayerPrefs.SetInt("settings_video_vsync", isOn ? 1 : 0);
    }

    // Anda bisa menambahkan fungsi publik lain untuk setelan baru di sini
    // public void SetFOV(float value) { ... }
}