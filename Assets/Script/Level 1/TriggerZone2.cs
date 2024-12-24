using UnityEngine;

public class TriggerZone2 : MonoBehaviour
{
    public bool isMovingLever = false;
    public Animator animator;
    public string playerTag = "Player"; // Customizable tag in the Inspector

    void Start()
    {
        // Pastikan animator sudah di-assign di inspector
        if (animator == null)
        {
            Debug.LogError("Animator not assigned. Please assign an Animator component.");
        }
    }

    // Fungsi ini akan dipanggil saat objek dengan tag yang diatur memasuki collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            animator.SetBool("IsMovingLeverPath2", false);
            Debug.Log($"ANIMATOR TRUE: {playerTag} has entered the trigger zone!");
        }
    }

    // Fungsi ini akan dipanggil saat objek dengan tag yang diatur keluar dari collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log($"{playerTag} has exited the trigger zone!");
            animator.SetBool("IsMovingLeverPath2", false);
        }
    }
}
