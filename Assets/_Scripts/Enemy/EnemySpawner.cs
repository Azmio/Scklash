using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;


    [System.Serializable]
    public class EnemyWave
    {
        public string name; // name because why not
        
        public List<EnemyType> enemiesInTheWave;    
        
        public float spawnDelay; // delay between each consecutive enemy spawn
       
        public int minDifficultyForNextWave;

        public float timeRequiredToClear=30f;
        // public GameObject enemyPrefab; //useful if we want to add different type of enemies
        // public int count; // number of enemies that we want in each wave
        // public List<GameObject> enemies;
    }

    //discarded enemy type class which was used by the spawnrate based spawner
    [System.Serializable]
    public class EnemyType
    {
        public string name;
        public GameObject enemyPrefab;
        public int count;
    }

    [Header("Diffculty Settings")]
    public int currentSceneDifficulty;
    public int globalNextWaveDifficulty;
    float timeAtWhichWaveSpawned;

    public bool useGlobalDifficulty;


    [Header("Wave Details")]
    public float waveDelay = 3f;
    public int waveNumber = 0;
    float nextWaveCountdown;
    public bool toLoop = false;

    public EnemyWave[] waves;

    [Header("Scene Entity Details")]
    public List<EnemyAI> enemiesInTheScene; // push in the spawned enemies in here

    public List<EnemyAI> UtilityStatesInTheScene;


    public enum SpawnerState { SPAWNING,WAITING,COUNTING}; // in order to check the current state of the spawner so that it wont mess up

    [Header("Spawner State")]
    public SpawnerState state = SpawnerState.COUNTING;

    [Header("Enemies Spawned By Soulk 4")]
    public List<GameObject> prefabsForUtilityThief;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }
    private void Start()
    {
        //check if we have at least one wave and one spawn point
        
        nextWaveCountdown = waveDelay;
        CheckParameters();
    }
    
    void CheckParameters()
    {
        if (waves.Length == 0)
        {
            Debug.LogError("No waves found.");
        }
    }

    private void Update()
    {
        GetSceneDifficulty();
        if (state == SpawnerState.WAITING)
        {
            int tempDifficulty=globalNextWaveDifficulty;

            if (!useGlobalDifficulty)
            {
                tempDifficulty = waves[waveNumber].minDifficultyForNextWave;
            }


            bool timeOver = false;
            if(Time.time>=(timeAtWhichWaveSpawned+waves[waveNumber].timeRequiredToClear))
            {
                timeOver = true;
                //if time is over then spawn the next wave
            }


            if (EnemyIsDead()||currentSceneDifficulty<tempDifficulty||timeOver)
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

    private void GetSceneDifficulty()
    {
        currentSceneDifficulty = 0;
        foreach(EnemyAI enemy in enemiesInTheScene)
        {
            //Only calculate difficulty if the enemies are not utility thieves
            if(enemy.enemyType != EnemyAI.Type.UtilityThief)
                currentSceneDifficulty += enemy.enemyCombat.soulValue;
        }
    }

    void BegingNextWave()
    {
       // Debug.Log("Wave Completed");
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
                state = SpawnerState.WAITING;
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

        foreach(EnemyType enemyToSpawn in wave.enemiesInTheWave)
        {
            for(int i = 1; i <= enemyToSpawn.count; i++)
            {
                SpawnEnemy(enemyToSpawn.enemyPrefab.GetComponent<EnemyAI>());
                yield return new WaitForSeconds(wave.spawnDelay);
            }
        }
        timeAtWhichWaveSpawned = Time.time;
        state = SpawnerState.WAITING; // wait until all the enemies die


        yield break;
    }

    /*
     * older spawn wave
     *  IEnumerator SpawnWave(EnemyWave wave)
    {
        Debug.Log("Spawning Wave" + wave.name);
        state = SpawnerState.SPAWNING;

        for(int i = 1; i <= wave.count; i++)
        {
            if (i % 5 == 0)
            {
                SpawnEnemy(rangedEnemy);
            }
            else if (i % 4 == 0)
            {
                SpawnEnemy(bigChungus);
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
        //at the moment the game thinks that utility state thieves are enemies too and wont spawn more enemies until we have killed the utility state enemies
        if (enemiesInTheScene.Count == 0)
        {
           // Debug.Log("Enemy died");
            return true;
        }
        return false;
    }




    public void SpawnEnemy(EnemyAI _enemy)
    {
        GameObject spawnedEnemy=Instantiate(_enemy.gameObject,NavMeshLocation(_enemy.minSpawnDistanceFromPlayer), Quaternion.identity);
        //spawnedEnemy.SetActive(true);
        Debug.Log("Spawning Enemy" + _enemy.name);
    }

    public Vector3 NavMeshLocation(float radius)
    {
        Vector3 direction = Random.onUnitSphere * radius;
        direction += GameController.instance.Player.transform.position;

        NavMeshHit hit;

        while(!NavMesh.SamplePosition(direction, out hit, 1f, NavMesh.AllAreas))
        {
            direction = Random.onUnitSphere * radius;
            direction += GameController.instance.Player.transform.position;
        }

        return hit.position;
    }
}
/*
      
        //todo fix navmesh location function because it's geh


        Vector3 direction = Random.insideUnitSphere * radius;
        direction += GameController.instance.Player.transform.position;

        
        Vector3 temp=Vector3.one;
        bool changeLocation = true;
        while(changeLocation)
        {
            direction = Random.insideUnitSphere * radius;
            direction += GameController.instance.Player.transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(direction, out hit, 1f, NavMesh.AllAreas))
            {
                temp = hit.position;
                changeLocation = false;
                break;
            }
            
        }

        return temp;
 */