using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Mengelola daftar item yang telah dikumpulkan oleh pemain.
/// Pasang skrip ini pada GameObject Player.
/// </summary>
public class PlayerInventory : MonoBehaviour {
    // Menggunakan HashSet untuk penyimpanan yang lebih efisien dan mencegah duplikat.
    private HashSet<string> collectedItems = new HashSet<string>();

    /// <summary>
    /// Menambahkan sebuah item ke dalam daftar inventaris.
    /// </summary>
    /// <param name="itemName">Nama unik dari item yang ditambahkan.</param>
    public void AddItem(string itemName) {
        if (!collectedItems.Contains(itemName)) {
            collectedItems.Add(itemName);
            Debug.Log($"Item ditambahkan ke inventaris: {itemName}");
        }
    }

    /// <summary>
    /// Memeriksa apakah pemain sudah memiliki item tertentu.
    /// </summary>
    /// <param name="itemName">Nama unik dari item yang diperiksa.</param>
    /// <returns>True jika item ada di inventaris, false jika tidak.</returns>
    public bool HasItem(string itemName) {
        return collectedItems.Contains(itemName);
    }

    /// <summary>
    /// Menghapus item dari inventaris (jika itemnya perlu dikonsumsi/hilang setelah dipakai).
    /// </summary>
    /// <param name="itemName">Nama unik dari item yang dihapus.</param>
    public void RemoveItem(string itemName) {
        if (collectedItems.Contains(itemName)) {
            collectedItems.Remove(itemName);
            Debug.Log($"Item dihapus dari inventaris: {itemName}");
        }
    }
}