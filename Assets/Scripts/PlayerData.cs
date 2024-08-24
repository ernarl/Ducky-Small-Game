using System;
using UnityEngine.Rendering;

[System.Serializable]
public class PlayerData
{
    public bool[] LevelFinished = new bool[100];
    public int[] LevelStars = new int[100];
    public PlayerData(int[] levelStars)
    {
        this.LevelStars = levelStars;
    }

    public PlayerData() { }
}

