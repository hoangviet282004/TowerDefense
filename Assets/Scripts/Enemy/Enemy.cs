using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public EnemyData  Data =>  data;
    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event Action<Enemy> OnEnemyDestroyed;


    //[SerializeField] private float moveSpeed = 3f; // tao o vs default value
     private Path _currentPath;

    private Vector3 _targetPosition;
    private int _currentWaypoint;
    private float _lives;
    private float _maxLives;

    [SerializeField] private Transform healthBar;
    private Vector3 _healthBarOriginalScale;

    private bool _hasBeenCounted = false;


    private void Awake()
    {
        _currentPath = GameObject.Find("Path1").GetComponent<Path>();
        _healthBarOriginalScale = healthBar.localScale;
    }


    private void OnEnable()
    {
        //_targetPosition = new Vector3(4, 0, 0); //x,y,z
        _currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint);
        //_lives = data.lives;
        //UpdateHealthBar();
    }
    void Update()
    {
        if (_hasBeenCounted) return;
        // move towards target position
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.speed * Time.deltaTime);

        // when target reached, set new target position
        float relativeDistance = (transform.position - _targetPosition).magnitude;

        if (relativeDistance < 0.1f)
        {
            if (_currentWaypoint < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint);
            }
            else // reached last waypoint
            {
                _hasBeenCounted = true;
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }


    public void TakeDamage(float damage)
    {
        if (_hasBeenCounted) return;

        _lives -= damage;
        _lives = Math.Max(_lives, 0);
        UpdateHealthBar();

        if (_lives <= 0)
        {
            _hasBeenCounted = true;

            //// PLAY DEATH SOUND
            //if (AudioManager.instance != null && AudioManager.instance.HasPlayEffectSound())
            //{
            //    AudioManager.instance.DeathClip();
            //}

            OnEnemyDestroyed?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
    private void UpdateHealthBar()
    {
        float healthPercent = _lives / _maxLives;
        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }

    public void Initialize(float healthMultiplier)
    {
        _hasBeenCounted = false;
        _maxLives  = data.lives * healthMultiplier;
        _lives = _maxLives;
        UpdateHealthBar();
    }
}
