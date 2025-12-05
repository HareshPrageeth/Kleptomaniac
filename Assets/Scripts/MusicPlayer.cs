using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip townMusic;
    [SerializeField] private AudioClip interiorMusic;
    [SerializeField] private AudioClip castleMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Optional fade duration
    [SerializeField] private float fadeTime = 0.75f;

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeToTrack(clip));
    }

    private IEnumerator FadeToTrack(AudioClip newTrack)
    {
        float startVolume = musicSource.volume;

        // Fade out
        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.clip = newTrack;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeTime);
            yield return null;
        }

        musicSource.volume = startVolume;
    }

    // === Public functions you can call anywhere ===
    public void PlayTownMusic() => PlayMusic(townMusic);
    public void PlayInteriorMusic() => PlayMusic(interiorMusic);
    public void PlayCastleMusic() => PlayMusic(castleMusic);
}
