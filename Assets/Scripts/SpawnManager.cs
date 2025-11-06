using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;



public class SpawnManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("spawn settings")]
    public GameObject enemyPrefab;
    public float spawnDelay = 3.0f;
    public int enemiesPerWave = 5;
    public List<Transform> spawnPoints = new List<Transform>();
    public GameObject[] enemytypes;
    public Transform enemyParent;

    private Dictionary<GameObject, GameObject> _enemyToFab = new Dictionary<GameObject, GameObject>();

    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _spawnedEnemies.RemoveAll(e => e == null);
        //remove all destroyed enemies 
    }

    public void SpawnEnemy()
    {
        StartCoroutine(SpawnAfterDelay(1));
    }

    public void SpawnWave(int count)
    {
        StartCoroutine(SpawnAfterDelay(count));
    }

    public void DestroyAllEnemies()
    {
        Debug.Log("Destroying all enemies");
        foreach (var enemy in _spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        _spawnedEnemies.Clear();
        _enemyToFab.Clear();
    }

    private IEnumerator SpawnAfterDelay(int count)
    {
        yield return new WaitForSeconds(spawnDelay);
        EnemySpawning();
        ///*for (int i = 0; i < count; i++)
        //{
        //    Vector3 spawnPos = transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));

        //    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        //    _spawnedEnemies.Add(enemy);
        //    _enemyToFab[enemy] = enemyPrefab;
        //}*/
    }

    private void EnemySpawning()
    {
        int nrOfEnem = Random.Range(1, 6);
        for (int i = 0; i < nrOfEnem; i++)
        {
            if (spawnPoints.Count == 0)
            {
                return;
            }
            int index = Random.Range(0, spawnPoints.Count);
            Transform spawnP = spawnPoints[index];
            int enemytype = Random.Range(0, enemytypes.Length);
            GameObject prefabtype = enemytypes[enemytype];
            Debug.Log("Spawning at " + spawnP);
            Instantiate(prefabtype, spawnP.position, spawnP.rotation, enemyParent);

        }

    }

    #region saving
    public void Save(ref SceneEnemyData data)
    {
        List<EnemySaveData> enemySaveDataList = new List<EnemySaveData>();

        for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (_spawnedEnemies[i] != null)
            {

                GameObject enemy = _spawnedEnemies[i];
                EnemyController ec = enemy.GetComponent<EnemyController>();

                EnemySaveData saveData = new EnemySaveData
                {
                    Position = enemy.transform.position,
                    EnemyPrefab = _enemyToFab[enemy],
                    Health = ec.GetCurrentHealth()
                };
                enemySaveDataList.Add(saveData);
            }
            else
            {
                _spawnedEnemies.RemoveAt(i);
            }
        }
        data.Enemies = enemySaveDataList.ToArray();
    }

    public void Load(SceneEnemyData data)
    {
        DestroyAllEnemies();
        foreach (var enemyData in data.Enemies)
        {
            if (enemyData.EnemyPrefab != null)
            {
                GameObject spawnedEnem = Instantiate(enemyData.EnemyPrefab, enemyData.Position, Quaternion.identity);
                EnemyController ec = spawnedEnem.GetComponent<EnemyController>();
                ec.SetCurrentHealth(enemyData.Health);
                _spawnedEnemies.Add(spawnedEnem);
                _enemyToFab[spawnedEnem] = enemyData.EnemyPrefab;
            }
        }
        #endregion

    }
}
[System.Serializable]
public struct SceneEnemyData
{
    public EnemySaveData[] Enemies;
}
[System.Serializable]
public struct EnemySaveData
{
    public Vector3 Position;
    public GameObject EnemyPrefab;
    public float Health;
}
