using UnityEngine;

public class ShowNarative : MonoBehaviour
{
    [SerializeField] private GameObject narrativeObject; // GameObject yang akan ditampilkan

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowNarrative();
        }
    }

    private void ShowNarrative()
    {
        if (narrativeObject != null)
        {
            narrativeObject.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Narative object is not assigned.");
        }
    }
}
