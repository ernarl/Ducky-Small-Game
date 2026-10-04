using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMeniu : BasicMeniu
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public override void Open()
    {
        // Show the saved volumes; without notify, so it doesn't call the setters below and save them again
        musicSlider.SetValueWithoutNotify(PersistantData.Instance.Volume);
        sfxSlider.SetValueWithoutNotify(PersistantData.Instance.VolumeSfx);
        base.Open();
    }

    public void SetSfxVolue(float _newVolume)
    {
        AudioManager.Instance.ChangeVolumeSfx(_newVolume);
    }

    public void SetMusicVolue(float _newVolume)
    {
        AudioManager.Instance.ChangeVolume(_newVolume);
    }
}
