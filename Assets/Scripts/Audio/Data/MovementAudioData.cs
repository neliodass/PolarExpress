using UnityEngine;

namespace Audio.Data
{
    [CreateAssetMenu(fileName = "AudioData_New", menuName = "Audio/Movement Audio Data")]
    public class MovementAudioData:ScriptableObject
    {
        
        [Tooltip ("list of audioclips to play for footsteps on this surface")]
        public AudioClip[] FootstepClips;
        
        [Header("Audio Settings")]
        [Range(0f,1f)]
        public float Volume = 0.5f;
        [Range(0.1f,3f)]
        public float PitchMin = 1.0f;
        [Range(0.1f,3f)]
        public float PitchMax = 1.0f;
        
        public AudioClip GetRandomFootstepClip()
        {
            if (FootstepClips == null || FootstepClips.Length == 0)
            {
                Debug.LogWarning($"No Footstep Clips defined for {this.name}");
                return null;
            }
            int index = Random.Range(0, FootstepClips.Length);
            return FootstepClips[index];
        }

        public float GetRandomPitch()
        {
            return Random.Range(PitchMin, PitchMax);
        }
        
    }
}