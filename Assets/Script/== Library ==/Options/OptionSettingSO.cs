// File: OptionSettingSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Option Setting", menuName = "Game Options/Universal Setting")]
public class OptionSettingSO : ScriptableObject {
    [Header("General Info")]
    public string settingName; // Nama yang tampil di UI, contoh: "Brightness"
    public string settingKey;  // Key untuk PlayerPrefs, contoh: "settings_video_brightness"
    public SettingType type;   // <-- INI KUNCINYA! Pilihan jenis setelan.

    [Header("Values for SLIDER type")]
    public float slider_minValue = 0f;
    public float slider_maxValue = 1f;
    public float slider_defaultValue = 0.8f;
    public bool slider_wholeNumbers = false;

    [Header("Values for TOGGLE type")]
    public bool toggle_defaultValue = true;

    // Nanti kalau butuh dropdown, tambahkan variabel di sini
    // [Header("Values for DROPDOWN type")]
    // public List<string> dropdown_options;
    // public int dropdown_defaultValue = 0;
}