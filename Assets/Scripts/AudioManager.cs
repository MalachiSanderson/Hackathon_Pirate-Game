using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sanavesa
{
    /// <summary>
    /// This class deals with everything audio-related. It is somewhat optimized to pool and recycle the audio sources to not lag the game.
    /// 
    /// <para>Attach this script to a single, empty Game Object at the beginning of your game and leave it be. If you have multiple scenes, put this ONLY in the first scene.</para>
    /// 
    /// <listheader>Types of audio:</listheader>
    /// <list type="bullet">
    /// <item>
    ///     <term>Sound</term>
    ///     <description>Short Audio (like SFX)</description>
    /// </item>
    /// <item>
    ///     <term>Music</term>
    ///     <description>Long Audio (like sound tracks)</description>
    /// </item>
    /// </list>
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        #region Data

        #region Inspector Variables

        [Header("Size")]
        [Tooltip("The maximum amount of concurrent sounds at a time. If set to 30, there can only be 30 concurrent sounds playing; rest are ignored.")]
        [SerializeField] private int m_numSoundSources = 30;
        [Tooltip("The maximum amount of concurrent music sources at a time. If set to 5, there can only be 5 concurrent music sources playing; rest are ignored.")]
        [SerializeField] private int m_numMusicSources = 5;

        #endregion

        #region Internal Variables

        private Stack<AudioSource> m_availableSoundSources = null;
        private Stack<AudioSource> m_availableMusicSources = null;
        private HashSet<AudioSource> m_inUseSoundSources = null;
        private HashSet<AudioSource> m_inUseMusicSources = null;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the maximum amount of concurrent sounds at a time. If set to 30, there can only be 30 concurrent sounds playing; rest are ignored.
        /// </summary>
        public int NumSoundSources => m_numSoundSources;

        /// <summary>
        /// Returns the maximum amount of concurrent music sources at a time. If set to 5, there can only be 5 concurrent music sources playing; rest are ignored.
        /// </summary>
        public int NumMusicSources => m_numMusicSources;

        /// <summary>
        /// Returns the number of available sound sources.
        /// </summary>
        public int NumAvailableSoundSources => m_availableSoundSources.Count;

        /// <summary>
        /// Returns the number of available music sources.
        /// </summary>
        public int NumAvailableMusicSources => m_availableMusicSources.Count;

        #endregion

        #endregion

        #region Behavior

        #region MonoBehaviour Methods

        private void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }

        #endregion

        #region Public API Methods

        /// <summary>
        /// Plays the specified audio clip with the specified attributes (volume, pitch, loop, delay).
        /// 
        /// <para>
        /// Also, returns via an the <i>out</i> parameter, the <i>AudioSource</i> instance that will play the sound.
        /// This will automatically free the audio source after it has completed, only if it is not looping. If looping, user must free the sound manually by calling <see cref="FreeSound(AudioSource)"/> or <see cref="FreeSoundAlll"/>, or by fading using <see cref="FadeSound(AudioSource, float)"/> or <see cref="FadeSoundAll(float)"/>.
        /// </para>
        /// 
        /// <para>Returns true if successful, or false if no sound sources are available.</para>
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="source">The audio source that will be used</param>
        /// <param name="volume">The volume of the clip</param>
        /// <param name="pitch">The pitch of the clip</param>
        /// <param name="loop">Whether to loop the sound or not</param>
        /// <param name="delay">Delay in seconds before playing the sound</param>
        /// <returns>Returns true if successful, or false if no sound sources are available</returns>
        public bool PlaySound(AudioClip clip, out AudioSource source, float volume = 1, float pitch = 1, bool loop = false, float delay = 0)
        {
            return PlaySourceInternal(m_availableSoundSources, m_inUseSoundSources, clip, out source, volume, pitch, loop, delay);
        }

        /// <summary>
        /// Plays the specified audio clip with the specified attributes (volume, pitch, delay).
        /// 
        /// <para>
        /// This will automatically free the audio source after it has completed, only if it is not looping. If looping, user must free the sound manually by calling <see cref="FreeSound(AudioSource)"/> or <see cref="FreeSoundAlll"/>, or by fading using <see cref="FadeSound(AudioSource, float)"/> or <see cref="FadeSoundAll(float)"/>.
        /// </para>
        /// 
        /// <para>User must free the sound manually by calling <see cref="FreeSoundAll"/>, or by fading using <see cref="FadeSoundAll(float)"/>.</para>
        /// 
        /// <para>Returns true if successful, or false if no sound sources are available.</para>
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume">The volume of the clip</param>
        /// <param name="pitch">The pitch of the clip</param>
        /// <param name="loop">Whether to loop the sound or not</param>
        /// <param name="delay">Delay in seconds before playing the sound</param>
        /// <returns>Returns true if successful, or false if no sound sources are available</returns>
        public bool PlaySound(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, float delay = 0)
        {
            return PlaySourceInternal(m_availableSoundSources, m_inUseSoundSources, clip, out _, volume, pitch, loop, delay);
        }

        public bool IsPlayingSound(AudioClip clip)
        {
            foreach (var source in m_inUseSoundSources)
            {
                if (source.clip == clip)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Frees the specified audio source, stopping it and making it free for future use.
        /// Only needed when the user played a sound with looping from <see cref="PlaySound(AudioClip, out AudioSource, float, float, bool, float)"/>.
        /// 
        /// <para>Returns true if successfuly freed sound, or false if an error occured.</para>
        /// </summary>
        /// <param name="source">The audio source to free</param>
        /// <returns>Returns true if successfuly freed sound, or false if an error occured.</returns>
        public bool FreeSound(AudioSource source)
        {
            return FreeSourceInternal(m_availableSoundSources, m_inUseSoundSources, source);
        }

        /// <summary>
        /// Frees all sound audio sources matching the predicate, stopping then and making them all free for future use.
        /// <param name="predicate">To determine which sounds to stop</param>
        /// </summary>
        public void FreeSoundAll(Predicate<AudioClip> predicate)
        {
            foreach (var source in m_inUseSoundSources)
            {
                if (predicate.Invoke(source.clip))
                    FreeSound(source);
            }
        }

        /// <summary>
        /// Frees all sound audio sources, stopping then and making them all free for future use.
        /// </summary>
        public void FreeSoundAll()
        {
            foreach (var source in m_inUseSoundSources)
            {
                FreeSound(source);
            }
        }

        /// <summary>
        /// Fades the specified sound audio source to silence over the specified duration in seconds.
        /// </summary>
        /// <param name="source">The audio source to fade</param>
        /// <param name="fadeDuration">Fade duration in seconds</param>
        public void FadeSound(AudioSource source, float fadeDuration)
        {
            StartCoroutine(FadeInternal(m_availableSoundSources, m_inUseSoundSources, source, fadeDuration));
        }

        /// <summary>
        /// Fades all sound audio sources to silence over the specified duration in seconds.
        /// </summary>
        /// <param name="fadeDuration">Fade duration in seconds</param>
        public void FadeSoundAll(float fadeDuration)
        {
            foreach (var source in m_inUseSoundSources)
            {
                FadeSound(source, fadeDuration);
            }
        }

        /// <summary>
        /// Fades all sound audio sources to silence over the specified duration in seconds.
        /// </summary>
        /// <param name="predicate">To determine sounds to fade</param>
        /// <param name="fadeDuration">Fade duration in seconds</param>
        public void FadeSoundAll(Predicate<AudioClip> predicate, float fadeDuration)
        {
            foreach (var source in m_inUseSoundSources)
            {
                if (predicate.Invoke(source.clip))
                    FadeSound(source, fadeDuration);
            }
        }

        /// <summary>
        /// Plays the specified audio clip with the specified attributes (volume, pitch, loop, delay).
        /// 
        /// <para>
        /// Also, returns via an the <i>out</i> parameter, the <i>AudioSource</i> instance that will play the music.
        /// This will automatically free the audio source after it has completed, only if it is not looping. If looping, user must free the music manually by calling <see cref="FreeMusic(AudioSource)"/> or <see cref="FreeMusicAll"/>, or by fading using <see cref="FadeMusic(AudioSource, float)"/> or <see cref="FadeMusicAll(float)"/>.
        /// </para>
        /// 
        /// <para>Returns true if successful, or false if no music sources are available.</para>
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="source">The audio source that will be used</param>
        /// <param name="volume">The volume of the clip</param>
        /// <param name="pitch">The pitch of the clip</param>
        /// <param name="loop">Whether to loop the music or not</param>
        /// <param name="delay">Delay in seconds before playing the music</param>
        /// <returns>Returns true if successful, or false if no music sources are available</returns>
        public bool PlayMusic(AudioClip clip, out AudioSource source, float volume = 1, float pitch = 1, bool loop = true, float delay = 0)
        {
            return PlaySourceInternal(m_availableMusicSources, m_inUseMusicSources, clip, out source, volume, pitch, loop, delay);
        }

        /// <summary>
        /// Plays the specified audio clip one-time with the specified attributes (volume, pitch, delay).
        /// 
        /// <para>User must free the music manually by calling <see cref="FreeMusicAll"/>, or by fading using <see cref="FadeMusicAll(float)"/>.</para>
        /// 
        /// <para>Returns true if successful, or false if no music sources are available.</para>
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume">The volume of the clip</param>
        /// <param name="pitch">The pitch of the clip</param>
        /// <param name="loop">Whether to loop the music or not</param>
        /// <param name="delay">Delay in seconds before playing the music</param>
        /// <returns>Returns true if successful, or false if no music sources are available</returns>
        public bool PlayMusic(AudioClip clip, float volume = 1, float pitch = 1, bool loop = true, float delay = 0)
        {
            return PlaySourceInternal(m_availableMusicSources, m_inUseMusicSources, clip, out _, volume, pitch, loop, delay);
        }

        /// <summary>
        /// Frees the specified audio source, stopping it and making it free for future use.
        /// Only needed when the user played a music source with looping from <see cref="PlayMusic(AudioClip, out AudioSource, float, float, bool, float)"/>.
        /// 
        /// <para>Returns true if successfuly freed music, or false if an error occured.</para>
        /// </summary>
        /// <param name="source">The audio source to free</param>
        /// <returns>Returns true if successfuly freed music, or false if an error occured.</returns>
        public bool FreeMusic(AudioSource source)
        {
            return FreeSourceInternal(m_availableMusicSources, m_inUseMusicSources, source);
        }

        /// <summary>
        /// Frees all music audio sources, stopping then and making them all free for future use.
        /// </summary>
        public void FreeMusicAll()
        {
            foreach (var source in m_inUseMusicSources)
            {
                FreeSound(source);
            }
        }

        /// <summary>
        /// Fades the specified music audio source to silence over the specified duration in seconds.
        /// </summary>
        /// <param name="source">The audio source to fade</param>
        /// <param name="fadeDuration">Fade duration in seconds</param>
        public void FadeMusic(AudioSource source, float fadeDuration)
        {
            StartCoroutine(FadeInternal(m_availableMusicSources, m_inUseMusicSources, source, fadeDuration));
        }

        /// <summary>
        /// Fades all music audio sources to silence over the specified duration in seconds.
        /// </summary>
        /// <param name="fadeDuration">Fade duration in seconds</param>
        public void FadeMusicAll(float fadeDuration)
        {
            foreach (var source in m_inUseMusicSources)
            {
                FadeMusic(source, fadeDuration);
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator FadeInternal(Stack<AudioSource> availableSources, HashSet<AudioSource> inUseSources, AudioSource source, float fadeDuration)
        {
            float elapsedTime = 0;
            float startVolume = source.volume;
            while (elapsedTime <= fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                source.volume = startVolume * Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
                yield return null;
            }
            source.volume = 0;

            FreeSourceInternal(availableSources, inUseSources, source);
        }

        private bool PlaySourceInternal(Stack<AudioSource> availableSources, HashSet<AudioSource> inUseSources, AudioClip clip, out AudioSource source, float volume = 1, float pitch = 1, bool loop = false, float delay = 0)
        {
            if (availableSources.Count == 0 || clip == null)
            {
                source = null;
                return false;
            }

            source = availableSources.Pop();
            inUseSources.Add(source);

            source.volume = volume;
            source.pitch = pitch;
            source.clip = clip;
            source.loop = loop;

            source.PlayDelayed(delay);

            if (!loop)
            {
                AudioSource audioSource = source;
                Execute(() => FreeSourceInternal(availableSources, inUseSources, audioSource), clip.length/pitch + delay);
            }

            return true;
        }

        private bool FreeSourceInternal(Stack<AudioSource> availableSources, HashSet<AudioSource> inUseSources, AudioSource source)
        {
            if (inUseSources.Remove(source))
            {
                source.Stop();
                availableSources.Push(source);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Initialize()
        {
            m_availableSoundSources = new Stack<AudioSource>(m_numSoundSources);
            m_inUseSoundSources = new HashSet<AudioSource>();

            m_availableMusicSources = new Stack<AudioSource>(m_numMusicSources);
            m_inUseMusicSources = new HashSet<AudioSource>();

            Transform soundRoot = new GameObject("Sound Sources Root").transform;
            soundRoot.SetParent(transform);

            Transform musicRoot = new GameObject("Music Sources Root").transform;
            musicRoot.SetParent(transform);

            for (int i = 0; i < m_numSoundSources; i++)
            {
                var go = new GameObject("Sound Source[" + (i + 1) + "]");
                go.transform.SetParent(soundRoot);
                m_availableSoundSources.Push(go.AddComponent<AudioSource>());
            }

            for (int i = 0; i < m_numMusicSources; i++)
            {
                var go = new GameObject("Music Source[" + (i + 1) + "]");
                go.transform.SetParent(musicRoot);
                m_availableMusicSources.Push(go.AddComponent<AudioSource>());
            }
        }

        private Coroutine Execute(Action action, float delay = 0)
        {
            return StartCoroutine(ExecuteInternal(action, delay));
        }

        private IEnumerator ExecuteInternal(Action action, float delay)
        {
            if (delay > 0)
                yield return new WaitForSecondsRealtime(delay);
            action?.Invoke();
        }

        #endregion

        #endregion

        #region Static

        private static AudioManager s_instance = null;

        /// <summary>
        /// The singleton access to <i>AudioManager</i> where all audio operations happen.
        /// </summary>
        public static AudioManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = GameObject.FindObjectOfType<AudioManager>();
                }

                if (s_instance == null)
                {
                    Debug.LogWarning("No AudioManager instance was found. Will generate a default one.");
                    var go = new GameObject("Generated Audio Manager");
                    s_instance = go.AddComponent<AudioManager>();
                }

                return s_instance;
            }

            private set
            {
                if (s_instance == null && value != null)
                {
                    s_instance = value;
                }
            }
        }

        #endregion
    }
}