using System;
using Audio.Data;
using Player.Interfaces;
using UnityEngine;

namespace Player.Components
{
    public class SurfaceDetector : MonoBehaviour, ISurfaceProvider
    {
        [Header("Raycast Settings")] [SerializeField]
        private Transform raycastOrigin;

        [SerializeField] private float raycastDistance = 1.1f;
        [SerializeField] private LayerMask groundMask;

        [SerializeField] private SurfaceMaterialType _defaultSurface;
        private SurfaceMaterialType _currentSurface;

        private void Awake()
        {
            if (raycastOrigin == null)
            {
                raycastOrigin = transform;
            }
        }

        private void Update()
        {
            if (Physics.Raycast(raycastOrigin.position, Vector3.down, out RaycastHit hit, raycastDistance, groundMask))
            {
                if (hit.collider.TryGetComponent<SurfaceIdentity>(out var surfaceIdentity))
                {
                    _currentSurface = surfaceIdentity.surfaceMaterial;
                }
                else
                {
                    _currentSurface = _defaultSurface;
                }
            }
            else
            {
                _currentSurface = null;
            }
        }

        public SurfaceMaterialType GetCurrentSurface()
        {
            return _currentSurface;
        }

        private void OnDrawGizmosSelected()
        {
            if (raycastOrigin == null) raycastOrigin = transform;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position + Vector3.down * raycastDistance);
        }
    }
}