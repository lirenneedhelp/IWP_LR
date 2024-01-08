using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    public AudioClip[] audioClips;
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // Load audio clips and assign them to AudioSource components
        foreach (var clip in audioClips)
        {
            GameObject audioObject = new GameObject(clip.name);
            audioObject.transform.parent = transform;

            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;  // Full 3D spatialization

            // You can add more audio settings here (e.g., min/max distance, rolloff, etc.)
        }
    }

    // Play sound at a specific position
    public void PlaySound(string soundName, Vector3 position)
    {
        AudioSource audioSource = transform.Find(soundName).GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.transform.position = position;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found.");
        }
    }
}
