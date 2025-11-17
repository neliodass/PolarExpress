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
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _footstepEventProvider.OnStep += HandleFootstep;
        }
        private void OnDestroy()
        {
            if (_footstepEventProvider != null)
            {
                _footstepEventProvider.OnStep -= HandleFootstep;
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
            Debug.Log("Footstep event received.");
            SurfaceMaterialType currentSurface = _surfaceProvider.GetCurrentSurface();
            if (currentSurface == null)
            {
                return;
            }
            SurfacedAudioData surfaceData = audioDatabase.FindDataForSurface(currentSurface);
            if (surfaceData == null)
            {
                // Mamy nawierzchnię (np. z defaultu), ale nie mamy dla niej dźwięków w bazie
                Debug.LogWarning($"Brak SurfacedAudioData dla {currentSurface.name}");
                return;
            }

            // --- KROK 5: SPRAWDŹ STAN GRACZA ---
            MovementAudioData movementData = GetMovementDataForCurrentState(surfaceData);
            if (movementData == null)
            {
                // Np. mamy dane dla "Trawy", ale nie mamy "Trawa_Crouch"
                Debug.LogWarning($"Brak MovementAudioData dla stanu gracza na {currentSurface.name}");
                return;
            }

            // --- KROK 6: ODTWÓRZ DŹWIĘK ---
            PlayAudio(movementData);
        }
        /// <summary>
        /// Wybiera odpowiedni MovementAudioData na podstawie stanu gracza.
        /// </summary>
        private MovementAudioData GetMovementDataForCurrentState(SurfacedAudioData surfaceData)
        {
            // Pytamy IPlayerStateProvider (który ma teraz IsSprinting, IsCrouching)
            if (_playerStateProvider.IsSprinting())
            {
                return surfaceData.sprintData;
            }
            if (_playerStateProvider.IsCrouching())
            {
                return surfaceData.crouchData;
            }
            
            // Domyślnie zwracamy dźwięk chodzenia
            // Zakładamy, że stan Idle nie wywołuje eventu OnStep,
            // więc jeśli dostaliśmy event, to jest to co najmniej chód.
            return surfaceData.walkData;
        }

        /// <summary>
        /// Odtwarza dźwięk na podstawie wybranych danych.
        /// </summary>
        private void PlayAudio(MovementAudioData movementData)
        {
            AudioClip clip = movementData.GetRandomFootstepClip();
            if (clip == null) return;

            // Ustawienia AudioSource
            _audioSource.pitch = movementData.GetRandomPitch();
            
            // PlayOneShot jest najlepszy dla szybkich, powtarzających się dźwięków
            // Używa głośności z SO pomnożonej przez głośność na AudioSource
            _audioSource.PlayOneShot(clip, movementData.Volume);
        }
    
    }
}
