using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceSfx;
    [SerializeField] private Sound[] clipsMusic;
    [SerializeField] private Sound[] clipsSfx;

    // Every sfx that plays at the same time needs its own source, otherwise changing the pitch for one changes the others too
    private const int SFX_SOURCE_COUNT = 8;

    private AudioSource[] sfxSources;
    private int nextSfxSource = 0;
    private float sfxSound;

    private bool playWithSound = true;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // MainScene gets loaded again when going back to the menu, keep the first AudioManager
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        Instance = this;
        CreateSfxSources();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Start()
    {
        if (Instance != this)
        {
            return;
        }

        ChangeVolume(PersistantData.Instance.Volume);
        ChangeVolumeSfx(PersistantData.Instance.VolumeSfx);

        sfxSound = PersistantData.Instance.VolumeSfx;

        // Started once here: the AudioManager stays between scenes, so the music keeps looping without restarting
        PlayMusic(SoundNames.MainMusic);

        // The scene this starts in can't be read yet in Awake (adding the sounds twice is skipped)
        AddButtonSounds(SceneManager.GetActiveScene());
    }

    private void CreateSfxSources()
    {
        sfxSources = new AudioSource[SFX_SOURCE_COUNT];
        sfxSources[0] = audioSourceSfx;
        for (int i = 1; i < sfxSources.Length; i++)
        {
            sfxSources[i] = gameObject.AddComponent<AudioSource>();
            sfxSources[i].playOnAwake = false;
            sfxSources[i].outputAudioMixerGroup = audioSourceSfx.outputAudioMixerGroup;
            sfxSources[i].spatialBlend = audioSourceSfx.spatialBlend;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddButtonSounds(scene);
    }

    // Gives every button in the scene click / hover sounds, so they don't have to be added to each button by hand
    private void AddButtonSounds(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                if (button.GetComponent<ButtonSound>() == null)
                {
                    button.gameObject.AddComponent<ButtonSound>();
                }
            }
        }
    }

    // Safe to call from anywhere: does nothing when the game was started from a scene without the AudioManager
    // (it's in MainScene, so e.g. when a level is opened directly in the editor)
    public static void Play(string sound)
    {
        if (Instance != null)
        {
            Instance.PlaySfx(sound);
        }
    }

    public void ChangeVolume(float volume)
    {
        audioSource.volume = volume;
        PersistantData.Instance.Volume = volume;

        PersistantData.Instance.SaveVolume();
    }
    public void ChangeVolumeSfx(float volume)
    {
        sfxSound = volume;
        PersistantData.Instance.VolumeSfx = volume;

        PersistantData.Instance.SaveVolumeSFX();
    }
    public void PlayMusic(string sound)
    {
        Sound s = Array.Find(clipsMusic, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Music: " + sound + " not found!");
            return;
        }
        audioSource.clip = s.clip;
        audioSource.loop = s.loop; // Set the loop property based on the sound's shouldLoop flag
        audioSource.Play();
    }

    public void PlaySfx(string sound)
    {
        Sound s = Array.Find(clipsSfx, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }

        if (!playWithSound)
        {
            return;
        }

        AudioSource source = sfxSources[nextSfxSource];
        nextSfxSource = (nextSfxSource + 1) % sfxSources.Length;

        // A bit of random volume and pitch, so sounds that repeat a lot (jumps, clicks) don't get tiring
        float volumeModifier = 1f + Random.Range(s.volumeVariance * -1, s.volumeVariance);
        float pitchModifier = Random.Range(s.pitchVariance * -1, s.pitchVariance);

        source.clip = s.clip;
        source.volume = sfxSound * s.volume * volumeModifier;
        source.pitch = s.pitch + pitchModifier;
        source.Play();
    }

    private IEnumerator MuteAudioListenerForDuration(float duration)
    {
        playWithSound = false;

        yield return new WaitForSeconds(duration);

        playWithSound = true;

    }
    public void StopAudio()
    {
        audioSource.Stop();
        foreach (AudioSource source in sfxSources)
        {
            source.Stop();
        }
    }
    public bool IsMusicPlaying()
    {
        return audioSource.isPlaying;
    }
    public void MuteAudioFor(float duration = 0.5f)
    {
        StartCoroutine(MuteAudioListenerForDuration(duration));
    }
}
