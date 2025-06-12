// File: ToggleOptionUI.cs

using UnityEngine;
using UnityEngine.UI;

public class ToggleOptionUI : MonoBehaviour {
    public Text labelText;
    public Toggle toggle;

    public void Setup(OptionSettingSO setting, SettingsManager manager) {
        labelText.text = setting.settingName;

        // Load nilai tersimpan. PlayerPrefs tidak punya bool, jadi kita pakai int (1=true, 0=false)
        bool currentValue = PlayerPrefs.GetInt(setting.settingKey, setting.toggle_defaultValue ? 1 : 0) == 1;
        toggle.isOn = currentValue;

        toggle.onValueChanged.RemoveAllListeners();

        // HUBUNGKAN event onValueChanged dari toggle ke FUNGSI yang TEPAT di SettingsManager
        switch (setting.settingKey) {
            case "settings_video_vsync":
                toggle.onValueChanged.AddListener(manager.SetVsync);
                break;
                // Tambahkan case baru di sini jika ada setelan toggle baru
        }
    }
}