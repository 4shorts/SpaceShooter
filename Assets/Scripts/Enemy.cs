using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject[] _laserPrefab;
    private float _fireRate = 3f;
    private float _canFire = -1f;

    [SerializeField] private int _enemyID = 0;
    private float _distanceY;
    [SerializeField] private float _spawnTime;
    [SerializeField] private float _frequency;
    private float _phase;

    [SerializeField] private float _speed = 4.0f;

    private AudioSource _audioSource;
    private Player _player;
    private Animator _anim;
    
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }

        if (_anim == null)
        {
            Debug.LogError("Animator is NULL.");
        }

        if ( _audioSource == null)
        {
            Debug.LogError("Audio Source for Enemy is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 8f);
            _canFire = Time.time + _fireRate;
            int randomLaser = Random.Range(0, 3);
            GameObject enemyLaser = Instantiate(_laserPrefab[randomLaser], transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement()
    {
        switch(_enemyID)
        {
            case 0:
                {
                    _distanceY = 0;
                    transform.Translate(Vector3.down * _speed * Time.deltaTime);
                    break;
                }
            case 1:
                {
                    _distanceY = _speed * Mathf.Sin(_frequency * Time.time - _spawnTime + _phase) * Time.deltaTime;
                    transform.Translate(Vector3.right * _distanceY);
                    transform.Translate(Vector3.down * _speed * Time.deltaTime);
                    break;
                }
        }

        if (transform.position.y < -5.5f)
        {
            float randomX = Random.Range(-9f, 9f);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(this.gameObject, 2.8f);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(10);
            }

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _canFire = 100;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }

        if (other.tag == "TargetMissile")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _canFire = 100;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }
}
