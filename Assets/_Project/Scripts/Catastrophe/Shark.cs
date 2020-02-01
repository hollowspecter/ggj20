using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shark : MonoBehaviour
{
    public float upForce = 3f;
    private Rigidbody rigidbody;
    private Transform playerTransform;
    private bool booped = false;

    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!booped)
            rigidbody.MoveRotation(Quaternion.LookRotation(playerTransform.position - transform.position));
        else
        {
            rigidbody.AddForce(Vector3.up * upForce);
            if (transform.position.y >= 100f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnBooped()
    {
        booped = true;
    }
}
