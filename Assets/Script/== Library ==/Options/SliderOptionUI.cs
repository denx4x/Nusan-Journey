// File: SliderOptionUI.cs

using UnityEngine;
using UnityEngine.UI;

public class SliderOptionUI : MonoBehaviour {
    public Text labelText;
    public Slider slider;
    public Text valueText; // Opsional: untuk menampilkan nilai slider (misal: 80%)

    public void Setup(OptionSettingSO setting, SettingsManager manager) {
        // Mengisi info dari ScriptableObject ke UI
        labelText.text = setting.settingName;
        slider.minValue = setting.slider_minValue;
        slider.maxValue = setting.slider_maxValue;
        slider.wholeNumbers = setting.slider_wholeNumbers;

        // Load nilai yang tersimpan di PlayerPrefs atau gunakan nilai default
        float currentValue = PlayerPrefs.GetFloat(setting.settingKey, setting.slider_defaultValue);
        slider.value = currentValue;
        UpdateValueText(currentValue);

        // Hapus listener lama untuk menghindari duplikasi event
        slider.onValueChanged.RemoveAllListeners();

        // HUBUNGKAN event onValueChanged dari slider ke FUNGSI yang TEPAT di SettingsManager
        // Ini adalah bagian "reference script" yang Anda tanyakan
        switch (setting.settingKey) {
            case "settings_video_brightness":
                slider.onValueChanged.AddListener(manager.SetBrightness);
                break;
            case "settings_audio_mastervolume":
                slider.onValueChanged.AddListener(manager.SetMasterVolume);
                break;
                // Tambahkan case baru di sini jika ada setelan slider baru
        }

        // Listener untuk update text nilai
        slider.onValueChanged.AddListener(UpdateValueText);
    }

    private void UpdateValueText(float value) {
        if (valueText != null) {
            if (slider.wholeNumbers) {
                valueText.text = value.ToString();
            } else {
                valueText.text = (value * 100).ToString("F0") + "%";
            }
        }
    }
}