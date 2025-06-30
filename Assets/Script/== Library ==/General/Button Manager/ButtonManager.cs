using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections; // <-- Diperlukan untuk Coroutine (IEnumerator)

public class ButtonManager : MonoBehaviour, IPointerEnterHandler {
    [Space]
    [Header("Sprite")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite selectedSprite;
    private Button button;
    private Image buttonImage;

    [Space]
    private AudioSource audioSource;

    [Space]
    [Header("Audio Sfx")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;

    [Space]
    [Header("Scene Settings")]
    public bool toggleLoadScene;
    public string sceneToLoad;

    public string GetSceneToLoad() => toggleLoadScene ? sceneToLoad : null;

    void Start() {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        button.image.sprite = idleSprite;
        button.onClick.AddListener(OnButtonClick);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnButtonClick() {
        // Memutar suara klik tombol
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

        // ---- PERUBAHAN UTAMA DIMULAI DI SINI ----
        // Memeriksa apakah scene perlu di-load dan tidak kosong
        if (toggleLoadScene && !string.IsNullOrEmpty(sceneToLoad)) {
            // Pengecekan kondisi khusus tetap sama
            if (sceneToLoad == "MainHub Sore" && PlayerPrefs.GetInt("HasLaunched", 0) == 0) {
                Debug.Log("Kondisi khusus, scene tidak dimuat.");
                return;
            }

            // Memulai Coroutine untuk memuat scene secara async
            StartCoroutine(LoadSceneAsyncCoroutine());
        }
        // ---- PERUBAHAN UTAMA SELESAI ----
    }

    // ---- COROUTINE BARU UNTUK ASYNC LOADING ----
    private IEnumerator LoadSceneAsyncCoroutine() {
        Debug.Log("Mulai memuat scene secara async: " + sceneToLoad);

        // Mulai memuat scene di background dan simpan operasinya
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        // (Opsional) Di sini Anda bisa mengaktifkan UI loading screen
        // contoh: loadingScreenPanel.SetActive(true);

        // Tunggu sampai scene selesai di-load
        while (!asyncOperation.isDone) {
            // (Opsional) Di sini Anda bisa memperbarui progress bar
            // float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            // loadingBar.fillAmount = progress;

            // Tunggu frame berikutnya sebelum melanjutkan loop
            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (buttonHoverSound != null) {
            audioSource.PlayOneShot(buttonHoverSound);
        }
    }

    public void QuitApplication() {
        Application.Quit();
    }

    public void ResetToIdle() {
        buttonImage.sprite = idleSprite;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ResetToIdle();
        }
    }
}