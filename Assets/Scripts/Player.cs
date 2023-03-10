using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _speedMultiplier = 2f;
    [SerializeField] private float _minSpeed = 5f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _fuel = 100f;

    [SerializeField] private int _shieldHits = 0;
    [SerializeField] private GameObject _shields;
    [SerializeField] private GameObject _rightEngine;
    [SerializeField] private GameObject _leftEngine;
    public CameraShake cameraShake;

    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _targetMissilePrefab;
    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField] private int _ammoCount = 15;
    [SerializeField] private AudioClip _noAmmo;

    [SerializeField] private int _score;

    [SerializeField] private int _lives = 3;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private bool _isTripleShotActive;
    private bool _isShieldsActive;
    private bool _isTargetMissileActive;

    [SerializeField] private AudioClip _laserSoundClip;
    private AudioSource _audioSource;
  
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source on Player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        ThrusterBoost();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            if (_ammoCount == 0)
            {
                AudioSource.PlayClipAtPoint(_noAmmo, transform.position);
                return;
            }
            FireWeapon();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 1), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }

        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void ThrusterBoost()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _speed = _maxSpeed;

            if (_fuel > 0)
            {
                _fuel -= 20f * Time.deltaTime;
            }
            else if (_fuel <= 0)
            {
                _speed = _minSpeed;
            }
        }
        else
        {
            _speed = _minSpeed;
            if (_fuel < 100)
            {
                _fuel += 5f * Time.deltaTime;
            }
        }
        _uiManager.UpdateThrusterBar(_fuel);
    }

    void FireWeapon()
    {
        AmmoCount(-1);
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }

        else if (_isTargetMissileActive == true)
        {
            Instantiate(_targetMissilePrefab, transform.position, Quaternion.identity);
        }

        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();
    }

    public void AmmoCount(int bullets)
    {
        _ammoCount += bullets;
        _uiManager.UpdateAmmoCount(_ammoCount);
    }

    public void AmmoRefill()
    {
        _ammoCount += 15;
        if (_ammoCount > 50)
        {
            _ammoCount = 50;
        }
        _uiManager.UpdateAmmoCount(_ammoCount);
    }

    public void Damage()
    {
        if (_isShieldsActive == true)
        {
            _shieldHits++;
            switch (_shieldHits)
            {
                case 1:
                    StartCoroutine(cameraShake.Shake(0.5f, 0.05f));
                    _shields.GetComponent<SpriteRenderer>().material.color = Color.green; 
                    break;
                case 2:
                    StartCoroutine(cameraShake.Shake(0.5f, 0.15f));
                    _shields.GetComponent<SpriteRenderer>().material.color = Color.red;
                    break;
                case 3:
                    StartCoroutine(cameraShake.Shake(0.5f, 0.25f));
                    _isShieldsActive = false;
                    _shields.SetActive(false);
                    break;
            }
            return;
        }

        _lives--;

        if (_lives == 2)
        {
            StartCoroutine(cameraShake.Shake(0.5f, 0.5f));
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            StartCoroutine(cameraShake.Shake(0.5f, 0.75f));
            _leftEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void Health()
    {
        _lives = 3;
        _rightEngine.SetActive(false);
        _leftEngine.SetActive(false);
        _uiManager.UpdateLives(_lives);
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _fuel = 100f;
    }

    public void ShieldsActive()
    {
        _isShieldsActive = true;
        _shields.SetActive(true);
        _shieldHits = 0;
        _shields.GetComponent<SpriteRenderer>().material.color = Color.cyan;
    }

    public void TargetMissileActive()
    {
        _isTargetMissileActive = true;
        StartCoroutine(TargetMissilePowerdownRoutine());
    }

    IEnumerator TargetMissilePowerdownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTargetMissileActive = false;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
