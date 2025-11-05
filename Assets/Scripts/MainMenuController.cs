using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private TMP_Text highScoreText;
    private void Start()
    {
        int highestLevel = PlayerPrefs.GetInt("HighestLevel", 1);
        Debug.Log("Loaded Highest Level: " + highestLevel);
        highScoreText.text = $"Highest Level: {highestLevel}";
    }

    public void StartNewGame()
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.allLevels[0]);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighestLevel");
        PlayerPrefs.Save();
        highScoreText.text = "Highest Level: 1";
    }
}