using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music Sources")]
    public AudioSource ambienceSource;
    public AudioSource chaseSource;

    [Header("Volumes")]
    [Range(0f, 1f)] public float ambienceVolume = 0.4f;
    [Range(0f, 1f)] public float chaseVolume = 0.7f;

    [Header("Fade")]
    public float fadeSpeed = 1.5f;

    private bool chaseActive = false;

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

    private void Update()
    {
        float targetAmbienceVolume = chaseActive ? 0f : ambienceVolume;
        float targetChaseVolume = chaseActive ? chaseVolume : 0f;

        FadeAudio(ambienceSource, targetAmbienceVolume);
        FadeAudio(chaseSource, targetChaseVolume);
    }

    private void FadeAudio(AudioSource source, float targetVolume)
    {
        if (source == null) return;

        source.volume = Mathf.MoveTowards(
            source.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );
    }

    public void SetChaseMusic(bool active)
    {
        chaseActive = active;
    }
}