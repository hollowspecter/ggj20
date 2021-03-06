﻿using System.Collections;
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

    private void Awake()
    {
        sharknado.transform.position = points[0].position;
    }

    private void Update()
    {
        sceneData.disasterMeter = numberOfSharks * panicPerSharkPerSecond;
        var t = (Time.time - startLerpTime) / lerpDuration;
        sharknado.transform.position = Vector3.Lerp(points[currentPoint - 1].position, points[currentPoint].position, t);
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