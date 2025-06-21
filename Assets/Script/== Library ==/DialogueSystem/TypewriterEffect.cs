using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour {
    [SerializeField] private float typewriterSpeed = 50f;

    public bool IsRunning { get; private set; }

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>() {'.', '!', '?'}, 0.6f),
        new Punctuation(new HashSet<char>() {',', ';', ':'}, 0.3f)
    };

    private Coroutine typingCoroutine;
    private TMP_Text textLabel;
    private string textToType;
    private float currentSpeedMultiplier; // Variabel untuk menyimpan pengali kecepatan

    public void Run(string textToType, TMP_Text textLabel, float speedMultiplier) {
        this.textToType = textToType;
        this.textLabel = textLabel;
        this.currentSpeedMultiplier = speedMultiplier; // Simpan pengali kecepatan

        typingCoroutine = StartCoroutine(TypeText());
    }

    public void Stop() {
        if (!IsRunning) return;
        StopCoroutine(typingCoroutine);
        OnTypingCompleted();
    }

    private IEnumerator TypeText() {
        IsRunning = true;
        textLabel.maxVisibleCharacters = 0;
        textLabel.text = textToType;

        float t = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length) {
            int lastCharIndex = charIndex;

            // Menggunakan pengali kecepatan dalam perhitungan
            t += Time.deltaTime * typewriterSpeed * currentSpeedMultiplier;

            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for (int i = lastCharIndex; i < charIndex; i++) {
                bool isLast = i >= textToType.Length - 1;
                textLabel.maxVisibleCharacters = i + 1;

                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _)) {
                    yield return new WaitForSeconds(waitTime);
                }
            }
            yield return null;
        }
        OnTypingCompleted();
    }

    private void OnTypingCompleted() {
        IsRunning = false;
        textLabel.maxVisibleCharacters = textToType.Length;
    }

    private bool IsPunctuation(char character, out float waitTime) {
        foreach (Punctuation punctuationCategory in punctuations) {
            if (punctuationCategory.Punctuations.Contains(character)) {
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }
        waitTime = default;
        return false;
    }

    private readonly struct Punctuation {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HashSet<char> punctuations, float waitTime) {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
}