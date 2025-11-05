using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public LevelData[] allLevels;
    public LevelData CurrentLevel {  get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        CurrentLevel = allLevels[0];
    }

    public void LoadLevel(LevelData levelData)
    {
        CurrentLevel = levelData;
        SceneManager.LoadScene(levelData.levelName);
    }
    public int GetCurrentLevelIndex()
    {
        return System.Array.IndexOf(allLevels, CurrentLevel);
    }

    public void LoadNextLevel()
    {
        int currentIndex = GetCurrentLevelIndex();
        int nextIndex = currentIndex + 1;

        if (nextIndex < allLevels.Length)
        {
            LoadLevel(allLevels[nextIndex]);
        }
        else
        {
            // Nếu không còn level nào → quay lại MainMenu
            SceneManager.LoadScene("MainMenu");
        }
    }

}
