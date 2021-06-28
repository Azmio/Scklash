using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner enemySpawner;
    public GameObject rangedEnemy;
    public GameObject meleeEnemy;
    public GameObject flyBoy;

    [System.Serializable]
    public class EnemyWave
    {
        public string name; // name because why not
        public int count; // number of enemies that we want in each wave
       // public GameObject enemyPrefab; //useful if we want to add different type of enemies
        public float spawnDelay; // delay between each consecutive enemy spawn


    }

    //discarded enemy type class which was used by the spawnrate based spawner
    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab;
        public float SpawnRate;
    }

    public EnemyWave[] waves;
    public Transform[] spawnPoints;
    public float waveDelay = 3f;
    public int waveNumber = 0;
    public float nextWaveCountdown;
    public bool toLoop = false;

    public List<EnemyAI> enemiesInTheScene; // push in the spawned enemies in here

    public List<EnemyAI> UtilityStatesInTheScene;


    public enum SpawnerState { SPAWNING,WAITING,COUNTING}; // in order to check the current state of the spawner so that it wont mess up
    public SpawnerState state = SpawnerState.COUNTING;
    private void Start()
    {
        
        //check if we have at least one wave and one spawn point
        enemySpawner = this;
        spawnPoints = gameObject.GetComponentsInChildren<Transform>();
        nextWaveCountdown = waveDelay;
        CheckParameters();
    }
    
    void CheckParameters()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points found.");
        }
        if (waves.Length == 0)
        {
            Debug.LogError("No waves found.");
        }
    }

    private void Update()
    {
        if (state == SpawnerState.WAITING)
        {
            if (EnemyIsDead())
            {
                //begin new wave
                BegingNextWave();
            }
            else
                return;
        }
        if (nextWaveCountdown <= 0)
        {
            if (state != SpawnerState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[waveNumber]));
            }
        }
        else
        {
            nextWaveCountdown -= Time.deltaTime;
        }
    }


    void BegingNextWave()
    {
        Debug.Log("Wave Completed");
        state = SpawnerState.COUNTING;
        nextWaveCountdown = waveDelay;
        if (waveNumber + 1 >= waves.Length )
        {
            if (toLoop)
            {
                waveNumber = 0;
                Debug.Log("Looping through the waves again");
            }
            else
            {
                return;
            }

            // check if all the waves are done for and add code to loop the waves
        }
        else
        {
            waveNumber++;
        }

    }


    IEnumerator SpawnWave(EnemyWave wave)
    {
        Debug.Log("Spawning Wave" + wave.name);
        state = SpawnerState.SPAWNING;

        for(int i = 1; i <= wave.count; i++)
        {
            if (i % 5 == 0)
            {
                SpawnEnemy(rangedEnemy);
            }
            else if (i % 3 == 0)
            {
                SpawnEnemy(flyBoy);
            }
            else
            {
                SpawnEnemy(meleeEnemy);
            }
            
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        state = SpawnerState.WAITING; // wait until all the enemies die


        yield break;
    }


    //discarded enemy selector that's based on spawn rate
    /*
    public GameObject GetEnemyToSpawn(EnemyWave wave)
    {
        float chance = Random.value;
        foreach (EnemyType enemy in wave.enemyTypes)
        {
            if (chance> enemy.SpawnRate)
            {
                return enemy.enemyPrefab;
            }
        }
        return null;
    }
    */
    bool EnemyIsDead()
    {
        if (enemiesInTheScene.Count == 0)
        {
            Debug.Log("Enemy died");
            return true;
        }
        return false;
    }




    public void SpawnEnemy(GameObject _enemy)
    {
        Transform _t = spawnPoints[Random.Range(1, spawnPoints.Length)];
        Instantiate(_enemy, _t.position, _t.rotation);
        Debug.Log("Spawning Enemy" + _enemy.name);
    }
}
