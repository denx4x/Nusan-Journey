// File: OptionsMenuUIBuilder.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptionsMenuUIBuilder : MonoBehaviour {
    [Header("Referensi Wajib")]
    public SettingsManager settingsManager;
    public Transform settingsContainer; // Panel kosong tempat menaruh UI setelan

    [Header("Prefab UI")]
    public GameObject sliderSettingPrefab;
    public GameObject toggleSettingPrefab;

    [Header("Data Opsi")]
    [Tooltip("Masukkan semua kategori opsi yang ingin ditampilkan di sini")]
    public List<OptionCategorySO> optionCategories;

    // Untuk sistem tab (opsional, bisa dikembangkan)
    // Untuk saat ini, kita tampilkan satu kategori saja
    public int categoryToShow = 0;

    void Start() {
        // Pastikan semua referensi terisi
        if (settingsManager == null || settingsContainer == null || sliderSettingPrefab == null || toggleSettingPrefab == null) {
            Debug.LogError("Referensi di OptionsMenuUIBuilder belum lengkap!");
            return;
        }
        BuildSettingsPanel();
    }

    void BuildSettingsPanel() {
        // Hapus UI lama jika ada
        foreach (Transform child in settingsContainer) {
            Destroy(child.gameObject);
        }

        // Cek apakah ada kategori yang bisa ditampilkan
        if (optionCategories.Count == 0 || optionCategories.Count <= categoryToShow) {
            Debug.LogWarning("Tidak ada kategori opsi untuk ditampilkan.");
            return;
        }

        // Ambil kategori yang mau ditampilkan
        OptionCategorySO category = optionCategories[categoryToShow];

        // Loop melalui setiap setelan di kategori yang dipilih
        foreach (var setting in category.settings) {
            // Gunakan switch case berdasarkan ENUM yang dipilih di ScriptableObject
            switch (setting.type) {
                case SettingType.Slider:
                    GameObject sliderGO = Instantiate(sliderSettingPrefab, settingsContainer);
                    sliderGO.GetComponent<SliderOptionUI>().Setup(setting, settingsManager);
                    break;

                case SettingType.Toggle:
                    GameObject toggleGO = Instantiate(toggleSettingPrefab, settingsContainer);
                    toggleGO.GetComponent<ToggleOptionUI>().Setup(setting, settingsManager);
                    break;
            }
        }
    }
}