using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public int wavesToWin;
    public int startingResources;
    public int startingLives;

    [Header("Difficulty Settings")]
    [Tooltip("Hệ số độ khó cho quái trong level này. 1 = bình thường, 1.5 = 50% trâu hơn.")]
    public float difficultyMultiplier = 1f;
    //public AudioClip backGroundMusic;
}
