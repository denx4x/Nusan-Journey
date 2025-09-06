using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening; // <-- PASTIKAN DOTWEEN ADA DI SINI

public class ButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler { // <-- TAMBAHKAN IPointerExitHandler
    [Space]
    [Header("Sprite")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite selectedSprite;
    private Button button;
    private Image buttonImage;

    [Space]
    [Header("Audio Sfx")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    private AudioSource audioSource;

    // ---- KODE BARU UNTUK ANIMASI HOVER ----
    [Space]
    [Header("Hover Animation")]
    public bool useHoverScale = true; // Aktifkan/nonaktifkan efek hover
    public float hoverScale = 1.1f;   // Seberapa besar skala saat di-hover
    public float hoverDuration = 0.2f;// Durasi animasi hover
    private Vector3 initialScale;
    // ---- AKHIR DARI KODE BARU ----

    [Space]
    [Header("Scene Settings")]
    public bool toggleLoadScene;
    public string sceneToLoad;

    public string GetSceneToLoad() => toggleLoadScene ? sceneToLoad : null;

    void Start() {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        audioSource = gameObject.AddComponent<AudioSource>();

        initialScale = transform.localScale; // <-- Simpan skala awal

        if (idleSprite != null) {
            button.image.sprite = idleSprite;
        }
        button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick() {
        if (buttonClickSound != null) {
            audioSource.PlayOneShot(buttonClickSound);
        }

        ButtonManager[] allButtons = FindObjectsOfType<ButtonManager>();
        foreach (ButtonManager btn in allButtons) {
            btn.ResetToIdle();
        }

        if (selectedSprite != null) {
            buttonImage.sprite = selectedSprite;
        }

        if (toggleLoadScene && !string.IsNullOrEmpty(sceneToLoad)) {
            if (sceneToLoad == "MainHub Sore" && PlayerPrefs.GetInt("HasLaunched", 0) == 0) {
                Debug.Log("Kondisi khusus, scene tidak dimuat.");
                return;
            }
            StartCoroutine(LoadSceneAsyncCoroutine());
        }
    }

    private IEnumerator LoadSceneAsyncCoroutine() {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncOperation.isDone) {
            yield return null;
        }
    }

    // ---- METHOD HOVER DIPERBARUI ----
    public void OnPointerEnter(PointerEventData eventData) {
        // Memainkan suara hover
        if (buttonHoverSound != null) {
            audioSource.PlayOneShot(buttonHoverSound);
        }

        // Memainkan animasi skala jika diaktifkan
        if (useHoverScale) {
            transform.DOKill(); // Hentikan animasi sebelumnya
            transform.DOScale(initialScale * hoverScale, hoverDuration).SetEase(Ease.OutBack);
        }
    }

    // ---- METHOD BARU SAAT KURSOR KELUAR ----
    public void OnPointerExit(PointerEventData eventData) {
        // Mengembalikan skala ke ukuran semula
        if (useHoverScale) {
            transform.DOKill(); // Hentikan animasi sebelumnya
            transform.DOScale(initialScale, hoverDuration).SetEase(Ease.OutSine);
        }
    }

    public void QuitApplication() {
        Application.Quit();
    }

    public void ResetToIdle() {
        if (idleSprite != null) {
            buttonImage.sprite = idleSprite;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ResetToIdle();
        }
    }
}