﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneData : MonoBehaviour
{
    public YarnProgram startDialogue;
    public string startNode;
    public Distraction[] distractions;
    public YarnProgram endDialogue;
    public string endNode;
    public float disasterTimer = 60f;

    public UnityEvent onSceneStart;
    public UnityEvent onDisasterStart;
    public UnityEvent onDisasterEnd;
    public UnityEvent onSceneEnd;

    private bool disasterRunning = false;

    public bool DisasterRunning => disasterRunning;

    private void Awake()
    {
        ToggleDistractions(false);

        // Turn on distraction
        onDisasterStart.AddListener(() => {
            ToggleDistractions(true);
            disasterRunning = true;
        });
    }

    private void Update()
    {
        if (disasterRunning)
        {
            disasterTimer -= Time.deltaTime;
            if (disasterTimer < 0f)
            {
                disasterRunning = false;
                onDisasterEnd.AddListener(() => ToggleDistractions(false));
            }
        }
    }

    public void ToggleDistractions(bool _isActive)
    {
        foreach(var distraction in distractions)
        {
            if (distraction != null)
                distraction.gameObject.SetActive(_isActive);
        }
    }
}
