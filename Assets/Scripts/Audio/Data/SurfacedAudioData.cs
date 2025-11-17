using UnityEngine;

namespace Audio.Data
{
    [CreateAssetMenu(fileName = "SurfaceAudio_New", menuName = "Audio/Surfaced Audio Data")]
    public class SurfacedAudioData:ScriptableObject
    {
        [Header("Identification")]
        [Tooltip("The surface material type associated with this audio data")]
        public SurfaceMaterialType SurfaceMaterial;

        [Header("Movement Audio Data")] [Tooltip("Audio data for movement sounds on this surface")]
        public MovementAudioData walkData;
        public MovementAudioData sprintData;
        public MovementAudioData crouchData;
        
        //
        // public MovementAudioData jumpData;
        // public MovementAudioData landData;

    }
}