using UnityEngine;

public class TriggerZone1 : MonoBehaviour
{
    public bool isMovingLever = false;
    public Animator animator;

    void Start()
    {
        // Pastikan animator sudah di-assign di inspector
        if (animator == null)
        {
            Debug.LogError("Animator not assigned. Please assign an Animator component.");
        }
    }

    // Fungsi ini akan dipanggil saat objek dengan tag "Player" memasuki collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("IsMovingLeverPath2", true);
            Debug.Log("ANIMATOR TRUE_Player has entered the trigger zone!");
        }
    }

    // Fungsi ini akan dipanggil saat objek dengan tag "Player" keluar dari collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has exited the trigger zone!");
            animator.SetBool("IsMovingLeverPath2", true);
        }
    }
}
