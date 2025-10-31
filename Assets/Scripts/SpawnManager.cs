using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject enemyPrefab;
    public float spawnDelay = 3.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy()
    {
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
