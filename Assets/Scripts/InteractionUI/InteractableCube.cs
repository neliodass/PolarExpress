using UnityEngine;

namespace InteractionUI
{
    public class InteractableCube:MonoBehaviour,IInteractable
    {
        [SerializeField] private Transform interactionPoint;

        public void Start()
        {
            if (interactionPoint == null)
            {
                interactionPoint = transform;
            }
        }
        
        public Vector3 GetInteractionPoint()
        {
            return interactionPoint.position;
        }
    }
}