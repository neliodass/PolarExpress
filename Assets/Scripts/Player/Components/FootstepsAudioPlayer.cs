using System;
using Audio.Data;
using Player.Interfaces;
using UnityEngine;

namespace Player.Components
{
    public class FootstepsAudioPlayer : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour footstepEventProviderSource;
        private IFootstepEventProvider _footstepEventProvider;
        [SerializeField] private MonoBehaviour playerStateProviderSource;
        private IPlayerStateProvider _playerStateProvider;
        [SerializeField] private MonoBehaviour surfaceProviderSource;
        private ISurfaceProvider _surfaceProvider;
        [SerializeField] private MonoBehaviour landingEventSource;
        private ILandingEventProvider _landingEventProvider;


        [SerializeField] private SurfaceAudioDatabase audioDatabase;

        private AudioSource _audioSource;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            _footstepEventProvider = footstepEventProviderSource.GetComponent<IFootstepEventProvider>();
            if (_footstepEventProvider == null)
            {
                Debug.LogError("FootstepEventProvider component not found on the specified source.");
                enabled = false;
                return;
            }

            _playerStateProvider = playerStateProviderSource.GetComponent<IPlayerStateProvider>();
            if (_playerStateProvider == null)
            {
                Debug.LogError("PlayerStateProvider component not found on the specified source.");
                enabled = false;
                return;
            }

            _surfaceProvider = surfaceProviderSource.GetComponent<ISurfaceProvider>();
            if (_surfaceProvider == null)
            {
                Debug.LogError("ISurfaceProvider component not found on the specified source.");
                enabled = false;
                return;
            }

            _landingEventProvider = landingEventSource.GetComponent<ILandingEventProvider>();
            if (_landingEventProvider == null)
            {
                Debug.LogWarning("ILandingEventProvider component not found on the specified source.");
                return;
            }

            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _footstepEventProvider.OnStep += HandleFootstep;
            _landingEventProvider.OnLanded += HandleLand;
        }

        private void OnDestroy()
        {
            if (_footstepEventProvider != null)
            {
                _footstepEventProvider.OnStep -= HandleFootstep;
            }

            if (_landingEventProvider != null)
            {
                _landingEventProvider.OnLanded -= HandleLand;
            }
        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void HandleFootstep()
        {
            SurfaceMaterialType currentSurface = _surfaceProvider.GetCurrentSurface();
            if (currentSurface == null)
            {
                return;
            }

            SurfacedAudioData surfaceData = audioDatabase.FindDataForSurface(currentSurface);
            if (surfaceData == null)
            {
                Debug.LogWarning($"Brak SurfacedAudioData dla {currentSurface.name}");
                return;
            }
            MovementAudioData movementData = GetMovementDataForCurrentState(surfaceData);
            if (movementData == null)
            {
                Debug.LogWarning($"Brak MovementAudioData dla stanu gracza na {currentSurface.name}");
                return;
            }
            PlayAudio(movementData);
        }

        private void HandleLand()
        {
         
            SurfaceMaterialType currentSurface = _surfaceProvider.GetCurrentSurface();
            if (currentSurface == null)
            {
                Debug.LogWarning("Current surface is null on landing.");
                return;
            }

            SurfacedAudioData surfaceData = audioDatabase.FindDataForSurface(currentSurface);
            if (surfaceData == null)
            {
                Debug.LogWarning($"Brak SurfacedAudioData dla {currentSurface.name}");
                return;
            }
            Debug.Log("Landing");
            PlayAudio(surfaceData.sprintData);
        }
        
        private MovementAudioData GetMovementDataForCurrentState(SurfacedAudioData surfaceData)
        {
            if (_playerStateProvider.IsSprinting())
            {
                return surfaceData.sprintData;
            }

            if (_playerStateProvider.IsCrouching())
            {
                return surfaceData.crouchData;
            }
            return surfaceData.walkData;
        }

        private void PlayAudio(MovementAudioData movementData)
        {
            AudioClip clip = movementData.GetRandomFootstepClip();
            if (clip == null) return;
            _audioSource.pitch = movementData.GetRandomPitch();
            _audioSource.PlayOneShot(clip, movementData.Volume);
        }
    }
}