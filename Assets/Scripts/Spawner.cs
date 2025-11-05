using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }


    public static event Action<int> OnWaveChanged;
    public static event Action OnMissionComplete;

    [SerializeField] private WaveData[] waves;
    private int _currentWaveIndex = 0; // => 0 is Orc, 1 => Dragon
    private int _waveCounter = 0;
    public int WaveCompleted => _waveCounter;   // ✅ wave đã hoàn thành xong
    private WaveData CurrentWave => waves[_currentWaveIndex];
    public int CurrentWaveNumber => _waveCounter + 1; // ✅ wave đang chơi


    private float _spawnTimer;
    private float _spawnCounter;
    private int _enemiesRemoved;
    //private float _spawInterval = 1f;
    //public GameObject Prefab;

    [SerializeField] private ObjectPooler orcPool;
    [SerializeField] private ObjectPooler dragonPool;
    [SerializeField] private ObjectPooler kaijuPool;

    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;

    private float _timeBetweenWaves = 1f;
    private float _waveCooldown;
    private bool _isBeetweenWaves = false;

    private bool _isEndlessMode = false;

    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Orc, orcPool },
            { EnemyType.Dragon, dragonPool },
            { EnemyType.Kaiju, kaijuPool },
        };

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Start()
    {
        OnWaveChanged?.Invoke(_waveCounter); // label UI
    }
    // Update is called once per frame
    void Update()
    {
        if (_isBeetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;
            if (_waveCooldown <= 0f)
            {
                if (_waveCounter +1 >= LevelManager.Instance.CurrentLevel.wavesToWin && !_isEndlessMode)
                {
                    OnMissionComplete?.Invoke();
                    return;
                }

                _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length;
                _waveCounter++;
                OnWaveChanged?.Invoke(_waveCounter); // label UI

                _spawnCounter = 0;
                _enemiesRemoved = 0;
                _spawnTimer = 0f;
                _isBeetweenWaves = false;
            }
        }
        else
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0 && _spawnCounter < CurrentWave.enemiesPerWave)
            {
                _spawnTimer = CurrentWave.spawnInterval;
                SpawnEnemy();
                _spawnCounter++;
            }
            else if (_spawnCounter >= CurrentWave.enemiesPerWave && _enemiesRemoved >= CurrentWave.enemiesPerWave)
            {
                _isBeetweenWaves = true;
                _waveCooldown = _timeBetweenWaves; 
            }
        }
    }


    // match the Wave with value
    private void SpawnEnemy()
    {
        if (_poolDictionary.TryGetValue(CurrentWave.enemyType, out var pool))
        {
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position;

            float healthMultiplier = 1f + (_waveCounter * 0.1f); // 40% per wave
            Enemy enemy = spawnedObject.GetComponent<Enemy>();
            enemy.Initialize(healthMultiplier);

            spawnedObject.SetActive(true);

        }

        //GameObject spawnedObject = GameObject.Instantiate(Prefab);
        //spawnedObject.transform.position = transform.position;
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _enemiesRemoved++;
    }
    public void EnableEndlessMode()
    {
        _isEndlessMode = true;
    }

    private void UpdateHighScore()
    {
        int highestWave = PlayerPrefs.GetInt("HighestWave", 0);
        if (CurrentWaveNumber > highestWave)
        {
            PlayerPrefs.SetInt("HighestWave", CurrentWaveNumber);
            PlayerPrefs.Save();
            Debug.Log($"New High Score Saved: Wave {CurrentWaveNumber}");
        }
    }
}
