using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkSpawner : MonoBehaviour
{
    public GameObject sharkPrefab;
    public Vector2 spawnDurationMinMax = new Vector2(2f, 8f);

    private float nextSharkSpawnTime;

    private void Start()
    {
        ResetSpawnTimer();
    }

    private void Update()
    {
        if (Time.time > nextSharkSpawnTime)
        {
            ResetSpawnTimer();
            SpawnShark();
        }
    }

    private void ResetSpawnTimer()
    {
        nextSharkSpawnTime = Random.Range(spawnDurationMinMax.x, spawnDurationMinMax.y);
    }

    private void SpawnShark()
    {
        // spawn shark

        // take a good angle to shoot

        // shoot with force
    }
}
