using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private TMP_Text highScoreText;
    private void Start()
    {
        int highestWave = PlayerPrefs.GetInt("HighestWave", 0);
        Debug.Log("Loaded High Score: " + highestWave);
        highScoreText.text = $"Highest Wave: {highestWave}";
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
        PlayerPrefs.DeleteKey("HighestWave");
        PlayerPrefs.Save();
        highScoreText.text = "Highest Wave: 0";
    }
}