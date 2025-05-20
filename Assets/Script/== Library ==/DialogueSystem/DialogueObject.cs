using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject {

    void Start() {
        PlayAudio(0);
    }

    [System.Serializable]
    public class DialogueEntry {
        [TextArea] public string Text1; // Teks dialog
        public AudioClip Audio; // AudioSource yang terkait
    }

    public DialogueEntry[] Dialogue; // Array dari teks dialog dan AudioSource

    // Fungsi untuk memainkan audio berdasarkan indeks dialog
    public void PlayAudio(int index) {
        if (index >= 0 && index < Dialogue.Length) {
            DialogueEntry entry = Dialogue[index];

            if (entry.Audio != null) {
                // Menggunakan AudioSource.PlayClipAtPoint untuk memainkan AudioClip
                AudioSource.PlayClipAtPoint(entry.Audio, Vector3.zero);
            }
        }
    }

    [SerializeField] private Response[] responses;

    public bool HasResponses => Responses != null && Responses.Length > 0;
    
    public Response[] Responses => responses;
}
