using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    public AudioClip[] audioClips;

    public AudioClip bgmClip;       // Background music

    private AudioSource bgmSource;
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
        // Initialize background music source
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.spatialBlend = 0f;  // No 3D spatialization for background music
        // You can add more audio settings here (e.g., volume, pitch, etc.)

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
    // Play background music
    public void PlayBGM()
    {
        if (bgmClip != null)
        {
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("No background music assigned.");
        }
    }

    // Stop background music
    public void StopBGM()
    {
        bgmSource.Stop();
    }
}
