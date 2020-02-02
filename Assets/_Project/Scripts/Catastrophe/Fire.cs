using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject firePrefab;
    public Vector2 spreadRangeMinmax = new Vector2(0.5f, 3f);
    public Vector2 spreadTimeMinMax = new Vector2(10f, 30f);
    public float size = 1f;
    public float increaseSpeed = 0.5f;
    public float extinguishThreshold = 0.5f;
    public Transform playerTransform;
    public LayerMask inflammableLayer;

    private float nextFireTime = 0f;

    private void Awake()
    {
        ResetSpreadTimer();

        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }
    }

    private void OnEnable()
    {
        FireDisaster.activeFires.Add(this);
    }

    private void OnDisable()
    {
        FireDisaster.activeFires.Remove(this);
    }

    void Update()
    {
        // increase slowly
        transform.localScale = transform.localScale + Vector3.one * increaseSpeed * Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            ResetSpreadTimer();
            SpawnFire();
        }
    }

    // on boop
    public void Reduce(float _value)
    {
        // reduce. if it is reduced to almost small enough, delete!
        transform.localScale = transform.localScale - Vector3.one * _value;

        if (transform.localScale.magnitude < extinguishThreshold)
        {
            Destroy(gameObject);
        }
    }

    private void ResetSpreadTimer()
    {
        nextFireTime = Time.time + Random.Range(spreadTimeMinMax.x, spreadTimeMinMax.y);
    }

    private void SpawnFire()
    {
        // get a new object to set on fire
        float radius = Random.Range(spreadRangeMinmax.x, spreadRangeMinmax.y);
        var hits = Physics.SphereCastAll(transform.position, radius, Vector3.one, 100f, inflammableLayer);
        if (hits.Length == 0) return;

        Vector3 spawnPosition = playerTransform.position;
        RaycastHit hit = new RaycastHit();
        while (Vector3.Distance(spawnPosition, playerTransform.position) < 2f)
        {
            // choose random 
            var pointToHit = hits[Random.Range(0, hits.Length)].point;
            // raycast from the player to that point to select spawn point.
            Physics.Raycast(playerTransform.position, (pointToHit - playerTransform.position), out hit, 100f, inflammableLayer);
            spawnPosition = hit.point;
        }

        // spawn a fire there
        var fireTransform = Instantiate(firePrefab, spawnPosition, Quaternion.identity, null).GetComponent<Transform>();
        fireTransform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
    }
}
