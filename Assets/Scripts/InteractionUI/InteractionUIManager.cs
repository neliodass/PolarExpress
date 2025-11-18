using UnityEngine;

namespace InteractionUI
{
    public class InteractionUIManager:MonoBehaviour
    {
        private Transform _cameraTransform;

        private void Start()
        {
            if (Camera.main != null) _cameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + _cameraTransform.rotation* Vector3.forward,
                _cameraTransform.rotation * Vector3.up);
            
        }

        public void ShowUi(Vector3 position)
        {
            this.gameObject.SetActive(true);
            this.transform.position = position;
        }
        public void HideUi()
        {
            this.gameObject.SetActive(false);
        }
    }
}