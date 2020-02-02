using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkSpawner : MonoBehaviour
{
    public GameObject sharkPrefab;
    public Vector2 spawnDurationMinMax = new Vector2(2f, 8f);
    public float xRotationOffsetRange = 20f;
    public float yRotationOffsetRange = 20f;
    public Vector2 forceMinMax = new Vector2(10f,30f);


    private float nextSharkSpawnTime;
    private float originalZRotation;
    private float originalXRotation;
    private float originalYRotation;
   

    private void Start()
    {
        ResetSpawnTimer();
        var euler = transform.rotation.eulerAngles;
        originalXRotation = euler.x;
        originalYRotation = euler.y;
        originalZRotation = euler.z;
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
        nextSharkSpawnTime = Time.time + Random.Range(spawnDurationMinMax.x, spawnDurationMinMax.y);
    }

    private void SpawnShark()
    {
        // spawn shark
        transform.rotation = Quaternion.Euler(originalXRotation + Random.Range(-xRotationOffsetRange, xRotationOffsetRange),
                                              originalYRotation + Random.Range(-yRotationOffsetRange, yRotationOffsetRange),
                                              originalZRotation);
        var shark = Instantiate(sharkPrefab, transform.position, Quaternion.LookRotation(transform.forward));

        // take a good angle to shoot
        shark.GetComponent<Rigidbody>().AddForce(transform.forward * Random.Range(forceMinMax.x, forceMinMax.y), ForceMode.Impulse);
    }
}
