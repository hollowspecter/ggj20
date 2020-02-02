using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharknadoDisaster : MonoBehaviour
{
    public int numberOfSharks = 0;
    public SceneData sceneData;
    public float panicPerSharkPerSecond = 0.01f;
    public Transform[] points;
    public Transform sharknado;
    public float speed = 3f;

    private int currentPoint = 0;
    private float startLerpTime = 0f;
    private float lerpDuration = 0f;

    private void Start()
    {
        sharknado.transform.position = points[currentPoint].position;
    }

    private void Update()
    {
        sceneData.disasterMeter = numberOfSharks * panicPerSharkPerSecond;
        Debug.Log("Distance to Point " + currentPoint + ": " + Vector3.Distance(transform.position, points[currentPoint].position));
        var t = (Time.time - startLerpTime) / lerpDuration;
        transform.position = Vector3.Lerp(points[currentPoint - 1].position, points[currentPoint].position, EasingFunction.EaseInOutQuad(0f, 1f, t));

        if (t >= 1)
        {
            GoToNextPoint();
        }
    }

    public void GoToNextPoint()
    {
        currentPoint++;
        startLerpTime = Time.time;
        lerpDuration = Vector3.Distance(points[currentPoint].position, points[currentPoint - 1].position) / speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent<Shark>(out var shark)) {
            numberOfSharks++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent<Shark>(out var shark))
        {
            numberOfSharks--;
        }
    }
}