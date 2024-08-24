using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMeniu : BasicMeniu
{
    public void SetSfxVolue(float _newVolume)
    {
        AudioManager.Instance.ChangeVolumeSfx(_newVolume);
    }

    public void SetMusicVolue(float _newVolume)
    {
        AudioManager.Instance.ChangeVolume(_newVolume);
    }
}
