using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceSfx;
    [SerializeField] private Sound[] clipsMusic;
    [SerializeField] private Sound[] clipsSfx;

    private float sfxSound;

    private bool playWithSound = true;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }
    private void Start()
    {

        ChangeVolume(PersistantData.Instance.Volume);
        ChangeVolumeSfx(PersistantData.Instance.VolumeSfx);

        sfxSound = PersistantData.Instance.VolumeSfx;

    }
    public void ChangeVolume(float volume)
    {
        audioSource.volume = volume;
        PersistantData.Instance.Volume = volume;

        PersistantData.Instance.SaveVolume();
    }
    public void ChangeVolumeSfx(float volume)
    {
        audioSourceSfx.volume = volume;
        sfxSound = volume;
        PersistantData.Instance.VolumeSfx = volume;

        PersistantData.Instance.SaveVolumeSFX();
    }
    public void PlayMusic(string sound)
    {
        Sound s = Array.Find(clipsMusic, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Music: " + name + " not found!");
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

        AudioSource audioSource = audioSourceSfx;

        // Copy properties from audioSourceSfx to the new audioSource
        audioSource.clip = audioSourceSfx.clip;
        audioSource.volume = audioSourceSfx.volume;
        audioSource.pitch = audioSourceSfx.pitch;
        // Add any other properties you want to synchronize

        // Calculate pitch and volume for this specific sound
        float soundVolume = playWithSound ? sfxSound * s.volume : 0f;
        float pitchModifier = Random.Range(s.pitchVariance * -1, s.pitchVariance);
        float finalPitch = s.pitch + pitchModifier;

        audioSource.PlayOneShot(s.clip, s.volume);
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
        audioSourceSfx.Stop();
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
