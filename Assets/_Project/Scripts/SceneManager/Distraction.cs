﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Boopable))]
public class Distraction : MonoBehaviour
{
    public YarnProgram distractionDialogue;
    public string distractionNode;
    public UnityEvent onDistractionStart;
    public UnityEvent onDistractionEnd;

    private Boopable boopable;
    private Yarn.Unity.DialogueRunner dialogueRunner;
    private bool wasStarted = false;

    Transform playerT;

    private void Awake()
    {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        boopable = GetComponent<Boopable>();
        boopable.onBooped.AddListener(StartDialogue);
    }

    private void Update()
    {
        if (wasStarted && !dialogueRunner.isDialogueRunning)
        {
            onDistractionEnd?.Invoke();
            Destroy(gameObject);
        }

        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.Rotate(Vector3.up, 180.0f, Space.Self);
    }

    private void StartDialogue()
    {
        if (dialogueRunner.isDialogueRunning) return;
        wasStarted = true;
        dialogueRunner.StartDialogue(distractionNode);
    }
}
