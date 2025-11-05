using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resourcesText;
    [SerializeField] private TMP_Text warningText;

    [SerializeField] private GameObject towerPanel;
    [SerializeField] private GameObject towerCardPrefab;
    [SerializeField] private Transform cardsContainer;

    [SerializeField] private TowerData[] towers;
    private List<GameObject> activeCards = new List<GameObject>();

    private Platform _currentPlatform;

    [SerializeField] private Button speed1Button;
    [SerializeField] private Button speed2Button;
    [SerializeField] private Button speed3Button;

    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color selectedButtonColor = Color.blue;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.white;

    [SerializeField] private GameObject pausePanel;
    private bool _isGamePaused = false;


    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text objectiveText;


    [SerializeField] private GameObject missionCompletePanel;
    [SerializeField] private Button nextLevelButton;


    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLivesText;
        GameManager.OnResourcesChanged += UpdateResourcesText;
        Platform.OnPlatformClicked += HandlePlatformClicked;
        TowerCard.OnTowerSelected += HandTowerSelected;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Spawner.OnMissionComplete += ShowMissonComplete;
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLivesText;
        GameManager.OnResourcesChanged -= UpdateResourcesText;
        Platform.OnPlatformClicked -= HandlePlatformClicked;
        TowerCard.OnTowerSelected -= HandTowerSelected;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Spawner.OnMissionComplete -= ShowMissonComplete;
    }

    private void Start()
    {
        nextLevelButton.interactable = false; // ban đầu khóa nút
        speed1Button.onClick.AddListener(() => SetGameSpeed(0.2f));
        speed2Button.onClick.AddListener(() => SetGameSpeed(1f));
        speed3Button.onClick.AddListener(() => SetGameSpeed(2f));

        HightLightSelectedButton(GameManager.Instance.GameSpeed);
        // *** Load saved SFX setting
        bool sfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
        AudioManager.instance.ToggleSFX(sfxOn);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (towerPanel.activeSelf)
            {
                HideTowerPanel();
            }
            else
            {
                TogglePause();
            }
        }
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }

    private void UpdateLivesText(int currentLives)
    {
        livesText.text = $"Lives: {currentLives}";

        if (currentLives <= 0)
        {
            // ShowGameOver
            ShowGameOver();
        }
    }
    private void UpdateResourcesText(int currentResources)
    {
        resourcesText.text = $"Resources: {currentResources}";
    }

    private void HandlePlatformClicked(Platform platform)
    {
        _currentPlatform = platform;
        ShowTowerPanel();
    }

    private void ShowTowerPanel()
    {
        towerPanel.SetActive(true);
        Platform.towerPanelOpen = true;
        GameManager.Instance.SetTimeScale(0f);
        PopulateTowerCard();
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        Platform.towerPanelOpen = false;
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
    }

    private void PopulateTowerCard()
    {
        foreach (var card in activeCards)
        {
            Destroy(card);
        }
        activeCards.Clear();

        foreach (var data in towers)
        {
            GameObject cardGameObject = Instantiate(towerCardPrefab, cardsContainer);
            TowerCard card = cardGameObject.GetComponent<TowerCard>();
            card.Initialize(data);
            activeCards.Add(cardGameObject);
        }
    }
    private void HandTowerSelected(TowerData towerData)
    {
        if (_currentPlatform.transform.childCount > 0)
        {
            HideTowerPanel();
            StartCoroutine(ShowWarningMessage("This flatform already has a tower!"));
            return;
        }

        if (GameManager.Instance.Resources >= towerData.cost)
        {
            GameManager.Instance.SpendResources(towerData.cost);
            _currentPlatform.PlaceTower(towerData);
        }
        else
        {
            StartCoroutine(ShowWarningMessage("Not Enough Resources!"));
        }
        HideTowerPanel();
    }
    private IEnumerator ShowWarningMessage(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        warningText.gameObject.SetActive(false);
    }
    private void SetGameSpeed(float timeScale)
    {
        HightLightSelectedButton(timeScale);
        GameManager.Instance.SetGameSpeed(timeScale);
    }

    private void UpadateButtonVisual(Button button, bool isSelected)
    {
        button.image.color = isSelected ? selectedButtonColor : normalButtonColor;

        TMP_Text text = button.GetComponentInChildren<TMP_Text>();

        if (text != null)
        {
            text.color = isSelected ? selectedTextColor : normalTextColor;
        }
    }
    private void HightLightSelectedButton(float selectedSpeed)
    {
        UpadateButtonVisual(speed1Button, selectedSpeed == 0.2f);
        UpadateButtonVisual(speed2Button, selectedSpeed == 1f);
        UpadateButtonVisual(speed3Button, selectedSpeed == 2f);
    }

    public void TogglePause()
    {
        if (towerPanel.activeSelf)
        {
            return;
        }
        if (_isGamePaused)
        {
            pausePanel.SetActive(false);
            _isGamePaused = false;
            GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);

            AudioListener.pause = false;
        }
        else
        {
            pausePanel.SetActive(true);
            _isGamePaused = true;
            GameManager.Instance.SetTimeScale(0f);

            AudioListener.pause = true;
        }
    }
    public void RestartLevel()
    {
        //GameManager.Instance.SetTimeScale(1f);
        //Scene currrentScene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(currrentScene.buildIndex);
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowGameOver()
    {
        GameManager.Instance.SetTimeScale(0f);
        gameOverPanel.SetActive(true);

        int finalWave = Spawner.Instance.WaveCompleted;   // ✅ wave đã survive
        int highestWave = PlayerPrefs.GetInt("HighestWave", 0);

        if (finalWave > highestWave)
        {
            PlayerPrefs.SetInt("HighestWave", finalWave);
            PlayerPrefs.Save();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(showObjective());
    }

    private IEnumerator showObjective()
    {
        objectiveText.text = $"Survive {LevelManager.Instance.CurrentLevel.wavesToWin} waves! ";
        objectiveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        objectiveText.gameObject.SetActive(false);
    }

    private void ShowMissonComplete()
    {
        missionCompletePanel.SetActive(true);
        GameManager.Instance.SetTimeScale(0f);
        // ✅ Bật lại nút “Next Level”
        nextLevelButton.interactable = true;
        // ✅ Cập nhật High Score khi hoàn thành tất cả wave
        int currentWave = Spawner.Instance.CurrentWaveNumber;
        int highestWave = PlayerPrefs.GetInt("HighestWave", 0);
        if (currentWave > highestWave)
        {
            PlayerPrefs.SetInt("HighestWave", currentWave);
            PlayerPrefs.Save();
            Debug.Log($"New high score saved on mission complete: Wave {currentWave}");
        }
    }

    public void EnterEndlessMode()
    {
        missionCompletePanel.SetActive(false);
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
        Spawner.Instance.EnableEndlessMode();
    }
    public void NextLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int nextSceneIndex = currentScene.buildIndex + 1;

        Debug.Log($"[NextLevel] CurrentScene: {currentScene.name} (index {currentScene.buildIndex}), NextSceneIndex: {nextSceneIndex}, Total: {SceneManager.sceneCountInBuildSettings}");

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            GameManager.Instance.SetTimeScale(1f);
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("[NextLevel] No next level found!");
            SceneManager.LoadScene("MainMenu");
        }
    }
    public void OnSFXToggleChanged(bool value)
    {
        AudioManager.instance.ToggleSFX(value);
        PlayerPrefs.SetInt("SFX", value ? 1 : 0);
    }

}
