using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;

    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject[] _enemies;

    private bool _stopSpawning;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            SpawnRoutine();
            //Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7.5f, 0);
            //GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            //newEnemy.transform.parent = _enemyContainer.transform;
            //yield return new WaitForSeconds(5.0f);
        }      
    }

    void SpawnRoutine()
    {
        float randomX = Random.Range(-9f, 9f);
        float randomY = Random.Range(1f, 5f);
        int randomEnemy = GenerateEnemyIndex(Random.Range(0, 101));

        Vector3 enemySpawn = new Vector3(randomX, 7f, 0);
        Vector3 sataliteEnemySpawn = new Vector3(-11f, randomY, 0);
        Vector3 sataliteEnemy2Spawn = new Vector3(11f, randomY, 0);
        //Vector3 enemyNewMovementSpawn = new Vector3(randomX, 7f, 0f);

        switch(randomEnemy)
        {
            case 0:
                GameObject newEnemy = Instantiate(_enemies[0], enemySpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                break;
            case 1:
                GameObject newEnemyNewMovement = Instantiate(_enemies[1], enemySpawn, Quaternion.identity);
                newEnemyNewMovement.transform.parent = _enemyContainer.transform;
                break;
            case 2:
                GameObject newAgressiveEnemy = Instantiate(_enemies[2], enemySpawn, Quaternion.identity);
                newAgressiveEnemy.transform.parent = _enemyContainer.transform;
                break;
            case 3:
                GameObject newShieldEnemy = Instantiate(_enemies[3], enemySpawn, Quaternion.identity);
                newShieldEnemy.transform.parent = _enemyContainer.transform;
                break;
            case 4:
                GameObject newSataliteEnemy = Instantiate(_enemies[4], sataliteEnemySpawn, Quaternion.identity);
                newSataliteEnemy.transform.parent = _enemyContainer.transform;
                break;
            case 5:
                GameObject newSatilateEnemy2 = Instantiate(_enemies[5], sataliteEnemy2Spawn, Quaternion.identity);
                newSatilateEnemy2.transform.parent = _enemyContainer.transform;
                break;
            case 6:
                GameObject newSmartEnemy = Instantiate(_enemies[6], enemySpawn, Quaternion.identity);
                newSmartEnemy.transform.parent = _enemyContainer.transform;
                break;
            default:
                break;
        }
    }

    int GenerateEnemyIndex(int random)
    {
        if (random >= 0 && random < 30)
        {
            return 0;
        }
        else if (random >= 30 && random < 55)
        {
            return 1;
        }
        else if (random >= 55 && random < 65)
        {
            return 2;
        }
        else if (random >= 65 && random < 85)
        {
            return 3;
        }
        else if (random >= 85 && random < 90)
        {
            return 4;
        }
        else if (random >= 90 && random < 95)
        {
            return 5;
        }
        else
        {
            return 6;
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(10.0f);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7.5f, 0);
            int randomPowerup = Random.Range(0, 6);
            Instantiate(_powerups[randomPowerup], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
