using InteractionUI;
using UnityEngine;

namespace Player.Components
{
    public class PlayerInteractor:MonoBehaviour
    {
        [SerializeField] private InteractionUIManager uiManagerPrefab;
        [SerializeField] private LayerMask interactionLayer;
        [SerializeField] private float maxInteractionDistance = 10f;
        
        private InteractionUIManager interactionUIInstance;
        private IInteractable currentInteractable;

        void Start()
        {
            interactionUIInstance = Instantiate(uiManagerPrefab);
            Debug.Log(uiManagerPrefab);
            interactionUIInstance.HideUi();
        }

        void Update()
        {
            RaycastHit hit;
            IInteractable interactable = null;
            if (Physics.Raycast(transform.position, Vector3.forward, out hit, maxInteractionDistance, interactionLayer))
            {
                hit.collider.TryGetComponent<IInteractable>(out interactable);
            }
            if (interactable != currentInteractable)
            {
                currentInteractable = interactable;
                if (currentInteractable != null)
                {
                    interactionUIInstance.ShowUi(currentInteractable.GetInteractionPoint());
                }
                else
                {
                    interactionUIInstance.HideUi();
                }
            }
            else if (currentInteractable != null)
            {
                interactionUIInstance.ShowUi(currentInteractable.GetInteractionPoint());
               
            }
        }
    }
}