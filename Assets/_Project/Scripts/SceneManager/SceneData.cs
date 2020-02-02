using System.Collections;
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
    public float disasterMeter = 0.03f; // can be changed from afar :)
    public bool triggerBadDialogueByPanicMeter = false;
    public float panicMeterThresholdForBad = 0.7f;
    public string badDialogueEndNode = "";

    public UnityEvent onSceneStart;
    public UnityEvent onDisasterStart;
    public UnityEvent onDisasterEnd;
    public UnityEvent onSceneEnd;

    private bool disasterRunning = false;
    private bool endDisasterEarly = false;

    public void SetEndNode(string _endNode) => endNode = _endNode;

    public bool DisasterRunning => disasterRunning;

    private void Awake()
    {
        ToggleDistractions(false);

        // Turn on distraction
        onDisasterStart.AddListener(() => {
            ToggleDistractions(true);
            disasterRunning = true;
        });

        if (triggerBadDialogueByPanicMeter)
        {
            onDisasterEnd.AddListener(CheckPanicMeterForEndDialogue);
        }
    }

    private void Update()
    {
        if (disasterRunning)
        {
            disasterTimer -= Time.deltaTime;
            if (disasterTimer < 0f || endDisasterEarly)
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

    // call this to end the disaster early!
    public void EndDisasterEarly()
    {
        if (disasterRunning)
        {
            endDisasterEarly = true;
        }
    }

    private void CheckPanicMeterForEndDialogue()
    {
        if (FindObjectOfType<DateController>().PanicMeter > panicMeterThresholdForBad)
        {
            endNode = badDialogueEndNode;
        }
    }
}
