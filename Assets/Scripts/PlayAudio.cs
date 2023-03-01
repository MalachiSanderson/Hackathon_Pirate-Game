using Sanavesa;
using UnityEditor;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public AudioClip clip;
    [Range(0, 1)]
    public float volume = 1.0f;
    public float pitch = 1.0f;
    public float delay = 0.0f;
    public bool loop = false;
    public bool isMusic = false;
    public bool playOnAwake = false;

    [HideInInspector]
    public AudioSource source;

    private void Start()
    {
        if (playOnAwake)
            Play();
    }

    public void Play()
    {
        if (isMusic)
            AudioManager.Instance.PlayMusic(clip, out source, volume, pitch, loop, delay);
        else
            AudioManager.Instance.PlaySound(clip, out source, volume, pitch, loop, delay);
    }

    public void Fade(float duration = 0.5f)
    {
        if (source == null) return;

        if (isMusic)
            AudioManager.Instance.FadeMusic(source, duration);
        else
            AudioManager.Instance.FadeSound(source, duration);
    }

    public void Stop()
    {
        if (source == null) return;

        if (isMusic)
            AudioManager.Instance.FreeMusic(source);
        else
            AudioManager.Instance.FreeSound(source);
    }

    public bool IsPlaying => source != null && source.isPlaying;
}