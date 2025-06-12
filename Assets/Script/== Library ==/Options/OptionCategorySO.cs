// File: OptionCategorySO.cs

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Option Category", menuName = "Game Options/Option Category")]
public class OptionCategorySO : ScriptableObject {
    public string categoryName;
    public List<OptionSettingSO> settings; // Daftar semua setelan di dalam kategori ini
}