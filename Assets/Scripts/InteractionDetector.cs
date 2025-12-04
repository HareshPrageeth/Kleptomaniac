using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private Interactable interactableInRange;
    private GameObject currentInteractionObject = null, interactionDots;
   void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            interactableInRange?.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Interactable interactable) && interactable.CanInteract())
        {
            if (currentInteractionObject != null)
            {
                interactionDots.SetActive(false);                
            }
            interactableInRange = interactable;
            currentInteractionObject = collision.gameObject;
            interactionDots = collision.gameObject.transform.Find("Dots").gameObject;
            if(interactionDots != null)
            {
                interactionDots.SetActive(true);
            }
        }
    }    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Interactable interactable) && interactable.CanInteract())
        {
            if (currentInteractionObject != null)
            {
                interactionDots.SetActive(false);                
            }
            interactableInRange = null;
            currentInteractionObject = null;
            interactionDots = null;
        }
    }
}
