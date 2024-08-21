using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantData : MonoBehaviour
{
    public static PersistantData Instance { get; private set; }
    [Header("GameData")]
    public float Volume = 0.3f;
    public float VolumeSfx = 0.3f;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
            Instance = this;
            LoadPlayerPrefs();
        }
    }
    private void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(VolumeKey))
        {
            Volume = PlayerPrefs.GetFloat(VolumeKey);
        }
        if (PlayerPrefs.HasKey(SfxVolumeKey))
        {
            VolumeSfx = PlayerPrefs.GetFloat(SfxVolumeKey);
        }
    }

    // PlayerPrefs keys for storing the volume data
    private const string VolumeKey = "MasterVolume";
    private const string SfxVolumeKey = "SfxVolume";

    public void SaveVolume()
    {
        // Save the volume data to PlayerPrefs
        PlayerPrefs.SetFloat(VolumeKey, Volume);
        PlayerPrefs.Save();
    }

    public void SaveVolumeSFX()
    {
        // Save the SFX volume data to PlayerPrefs
        PlayerPrefs.SetFloat(SfxVolumeKey, VolumeSfx);
        PlayerPrefs.Save();
    }
}
