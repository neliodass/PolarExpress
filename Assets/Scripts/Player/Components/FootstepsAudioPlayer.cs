using System;
using Player.Interfaces;
using UnityEngine;

public class FootstepsAudioPlayer : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _footstepEventProviderSource;
    private IFootstepEventProvider _footstepEventProvider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _footstepEventProvider = _footstepEventProviderSource.GetComponent<IFootstepEventProvider>();
        if (_footstepEventProvider == null)
        {
            Debug.LogError("FootstepEventProvider component not found on the specified source.");
            enabled = false;
            return;
        }
        _footstepEventProvider.OnStep += HandleFootstep;
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
        Debug.Log("Footstep sound played. "+DateTime.Now.ToString("HH:mm:ss"));
    }
}
