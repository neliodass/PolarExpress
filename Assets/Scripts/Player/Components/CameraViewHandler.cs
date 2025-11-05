using Player.Interfaces;
using UnityEngine;
namespace Player.Components

{
    [RequireComponent(typeof(Camera))]
    public class CameraViewHandler:MonoBehaviour, ICameraView
    {
        private Camera _camera;
        private float _defaultFov;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _defaultFov = _camera.fieldOfView;
        }

        public void SetFieldOfView(float fov)
        {
            _camera.fieldOfView = fov;
        }

        public void ResetFieldOfView()
        {
            _camera.fieldOfView = _defaultFov;
        }
    }
}