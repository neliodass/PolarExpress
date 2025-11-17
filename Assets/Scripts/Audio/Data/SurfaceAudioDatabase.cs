using System.Collections.Generic;
using UnityEngine;

namespace Audio.Data
{
    [CreateAssetMenu(fileName = "SurfaceAudioDatabase", menuName = "Audio/SurfaceAudioDatabase")]
    public class SurfaceAudioDatabase : ScriptableObject
    {
        [SerializeField] private List<SurfacedAudioData> surfaceAudioDataList;
        private Dictionary<SurfaceMaterialType, SurfacedAudioData> _dataMap;

        private void OnEnable()
        {
            _dataMap = new Dictionary<SurfaceMaterialType, SurfacedAudioData>();
            foreach (var data in surfaceAudioDataList)
            {
                if (data != null && data.SurfaceMaterial != null)
                {
                    _dataMap[data.SurfaceMaterial] = data;
                }
            }
        }
        public SurfacedAudioData FindDataForSurface(SurfaceMaterialType surface)
        {
            if (surface == null) return null;
            
            _dataMap.TryGetValue(surface, out var data);
            return data;
        }
    }
}
