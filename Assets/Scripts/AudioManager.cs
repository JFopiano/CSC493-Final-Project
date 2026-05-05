using UnityEngine;


// This script manages the audio for the game, allowing for smooth transitions between ambience and chase music.
public class AudioManager : MonoBehaviour
{   

    // Music sources for ambience and chase music, set in the Unity Inspector.
    [Header("Music Sources")]
    public AudioSource ambienceSource;
    public AudioSource chaseSource;

    // Volume settings for both music types, adjustable in the Unity Inspector.
    [Header("Volumes")]
    [Range(0f, 1f)] public float ambienceVolume = 0.4f;
    [Range(0f, 1f)] public float chaseVolume = 0.7f;


    // Fade speed for transitioning between music, adjustable in the Unity Inspector.
    [Header("Fade")]
    public float fadeSpeed = 1.5f;

    // Flag to track whether chase music is active.
    private bool chaseActive = false;

    // Initialize audio sources and start playing both, with chase music initially silent.
    private void Start()
    {
        if (ambienceSource != null)
        {
            ambienceSource.loop = true;
            ambienceSource.volume = ambienceVolume;
            ambienceSource.Play();
        }

        if (chaseSource != null)
        {
            chaseSource.loop = true;
            chaseSource.volume = 0f;
            chaseSource.Play();
        }
    }

    // Update is called once per frame to handle fading between music tracks based on the chaseActive flag.
    private void Update()
    {   
        // Determine target volumes based on whether chase music is active.
        float targetAmbienceVolume = chaseActive ? 0f : ambienceVolume;
        float targetChaseVolume = chaseActive ? chaseVolume : 0f;

        // Fade both audio sources towards their target volumes.
        FadeAudio(ambienceSource, targetAmbienceVolume);
        FadeAudio(chaseSource, targetChaseVolume);
    }

    // Helper method to fade an audio source towards a target volume smoothly over time.
    private void FadeAudio(AudioSource source, float targetVolume)
    {
        if (source == null) return;

        source.volume = Mathf.MoveTowards(
            source.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );
    }

    // Public method to set the chase music active or inactive, which will trigger the fading logic in Update.
    public void SetChaseMusic(bool active)
    {
        chaseActive = active;
    }
}