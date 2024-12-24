using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractionNew : MonoBehaviour
{
    public float interactionDistance = 2f;
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;
    public KeyCode KeyInput = KeyCode.E;
    public string interactableTag = "Interactable"; // Customizable tag in the Inspector

    private IInteractable currentInteractable;

    private void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyInput))
        {
            currentInteractable.Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(interactableTag))
        {
            Debug.Log("Trigger Enter Detected");
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log("Interactable Component Found");
                interactionText.text = interactable.GetDescription();
                interactionUI.SetActive(true);
                currentInteractable = interactable;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(interactableTag))
        {
            Debug.Log("Trigger Exit Detected");
            interactionUI.SetActive(false);
            interactionText.text = "";
            if (currentInteractable != null && other.GetComponent<IInteractable>() == currentInteractable)
            {
                currentInteractable = null;
            }
        }
    }
}
