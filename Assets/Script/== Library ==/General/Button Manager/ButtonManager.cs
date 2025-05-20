using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;  // Diperlukan untuk interface event

public class ButtonManager : MonoBehaviour, IPointerEnterHandler {

    [Space] 
    [Header("Sprite")]
    [SerializeField] private Sprite idleSprite;  // Sprite default (idle)
    [SerializeField] private Sprite selectedSprite;  // Sprite ketika tombol dipilih
    private Button button;  // Referensi ke komponen Button
    private Image buttonImage;  // Referensi ke komponen Image pada button

    [Space]
    private AudioSource audioSource;  // Komponen AudioSource untuk memutar suara

    [Space]
    [Header("Audio Sfx")]
    public AudioClip buttonClickSound;  // AudioClip untuk suara klik tombol
    public AudioClip buttonHoverSound;  // AudioClip untuk suara hover tombol

    [Space]
    [Header("Scene Settings")]
    public bool toggleLoadScene; // Toggle untuk menampilkan sceneToLoad di Inspector
    public string sceneToLoad; // Nama Scene yang ingin di load

    public string GetSceneToLoad() => toggleLoadScene ? sceneToLoad : null;

    void Start() {
        // Mendapatkan referensi ke komponen Button dan Image
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        // Mengatur sprite default ke idle
        button.image.sprite = idleSprite;

        // Menambahkan event listener untuk event klik pada tombol
        button.onClick.AddListener(OnButtonClick);

        // Menambahkan komponen AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnButtonClick() {
        // Memutar suara klik tombol jika buttonClickSound tidak null
        if (buttonClickSound != null) {
            audioSource.PlayOneShot(buttonClickSound);
        }

        // Mengatur semua tombol kembali ke idle sprite
        ButtonManager[] allButtons = FindObjectsOfType<ButtonManager>();
        foreach (ButtonManager btn in allButtons) {
            btn.ResetToIdle();
        }

        // Mengatur sprite tombol yang diklik ke selected sprite
        buttonImage.sprite = selectedSprite;

        // Memuat scene yang ditentukan
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Memuat scene: " + sceneToLoad);
            if (sceneToLoad == "MainHub Sore" && PlayerPrefs.GetInt("HasLaunched", 0) == 0) return;
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // Memutar suara hover jika buttonHoverSound tidak null
        if (buttonHoverSound != null) {
            audioSource.PlayOneShot(buttonHoverSound);
        }
    }

    public void QuitApplication() {
        // Fungsi untuk keluar dari aplikasi
        Application.Quit();
    }

    public void ResetToIdle() {
        // Mengatur sprite tombol kembali ke idle sprite
        buttonImage.sprite = idleSprite;
    }

    void Update()
    {
        // Menjalankan fungsi apabila tombol Esc ditekan untuk menyembunyikan atau menampilkan panel
        if (Input.GetKeyDown(KeyCode.Escape)) {            
            // Mengatur semua tombol kembali ke idle sprite
            ResetToIdle();
        }
    }
}
