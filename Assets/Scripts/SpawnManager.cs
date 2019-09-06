using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Manager<SpawnManager>
{
    public bool UsePoolManager = true;

    [SerializeField] private int objectsPerSpawn;
    [SerializeField] private float timeBetweenSpawns;
    private float countdownToNextSpawn;
    [SerializeField] private Vector3 spawnOrigin;
    [SerializeField] private float spawnRadius;
    [SerializeField] private List<GameObject> prefabs;

    private void Start()
    {
        countdownToNextSpawn = timeBetweenSpawns;
    }

    private void Update()
    {
        countdownToNextSpawn -= Time.deltaTime;

        if (countdownToNextSpawn <= 0)
        {
            long timeStart = System.Diagnostics.Stopwatch.GetTimestamp();

            for (int i = 0; i < objectsPerSpawn; i++)
            {
                SpawnOneObject();
            }

            long timeEnd = System.Diagnostics.Stopwatch.GetTimestamp();

            HUDManager.Instance.AddSPS((timeEnd - timeStart) / 1000000f);

            countdownToNextSpawn = timeBetweenSpawns;
        }
    }

    private void SpawnOneObject()
    {
        int prefabIndex = UnityEngine.Random.Range(0, prefabs.Count);
        GameObject prefab = prefabs[prefabIndex];

        if (UsePoolManager)
        {
            PoolManager.Instance.Instantiate(prefab, Utility.GetRandomPoint(spawnOrigin, spawnRadius, false), Quaternion.identity, transform);
        }
        else
        {
            GameObject.Instantiate(prefab, Utility.GetRandomPoint(spawnOrigin, spawnRadius, false), Quaternion.identity, transform);
        }
    }
}
