// File: DialogueObject.cs

using UnityEngine;
// using UnityEngine.Events; // Namespace ini tidak lagi dibutuhkan di sini

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject {
    [System.Serializable]
    public class DialogueEntry {
        [TextArea(3, 5)]
        public string Text1;
        public AudioClip Audio;

        [Tooltip("Pengali kecepatan typewriter. 1 = normal, 2 = 2x lebih cepat, 0.5 = setengah kecepatan.")]
        [Range(0.1f, 5f)]
        public float typewriterSpeedMultiplier = 1f;

        // --- PERUBAHAN: Ganti UnityEvent dengan string untuk nama sinyal ---
        [Header("Line Events")]
        [Tooltip("Nama sinyal/event yang akan dikirim saat baris ini mulai ditampilkan.")]
        public string eventOnLineShown;

        [Tooltip("Nama sinyal/event yang akan dikirim setelah baris ini selesai diketik.")]
        public string eventOnLineFinished;
        // -------------------------------------------------------------
    }

    public DialogueEntry[] Dialogue;
    [SerializeField] private Response[] responses;

    public bool HasResponses => Responses != null && Responses.Length > 0;
    public Response[] Responses => responses;
}