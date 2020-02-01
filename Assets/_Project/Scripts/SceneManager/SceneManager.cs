using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneManager : MonoBehaviour
{
    public SceneData[] scenes;

    public UnityEvent onSceneStart;
    public UnityEvent onDisasterStart;
    public UnityEvent onDisasterEnd;
    public UnityEvent onSceneEnd;
    public int currentSceneIndex = 0;

    private bool waitForSceneStart = true;
    private Yarn.Unity.DialogueRunner dialogueRunner;

    private void Awake()
    {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();

        // load all the scripts
        for (int i=0; i < scenes.Length; ++i)
        {
            if (scenes[i].startDialogue != null)
                dialogueRunner.Add(scenes[i].startDialogue);
            if (scenes[i].endDialogue != null)
                dialogueRunner.Add(scenes[i].endDialogue);
            for(int j=0; j<scenes[i].distractions.Length; ++j)
            {
                if (scenes[i].distractions[j].distractionDialogue != null)
                    dialogueRunner.Add(scenes[i].distractions[j].distractionDialogue);
            }
        }
    }

    public void StartScene()
    {
        if (waitForSceneStart && currentSceneIndex < scenes.Length)
        {
            waitForSceneStart = false;
            StartCoroutine(RunScene());
        }
    }

    private IEnumerator RunScene()
    {
        // Fetch the next scene
        SceneData currentScene = scenes[currentSceneIndex];

        // On Scene Start
        Debug.Log($"Start Scene {currentScene}");
        currentScene.onSceneStart?.Invoke();
        onSceneStart?.Invoke();
        dialogueRunner.StartDialogue(currentScene.startNode);

        // Wait till dialogue is done
        while (dialogueRunner.isDialogueRunning) {
            yield return null;
        }

        // Invoke disaster start
        Debug.Log($"Start Disaster {currentScene}");
        onDisasterStart?.Invoke();
        currentScene.onDisasterStart?.Invoke();
        yield return null;

        // Wait till disaster is over
        while (currentScene.DisasterRunning)
        {
            yield return null;
        }

        // Invoke disaster end
        Debug.Log($"End Disaster {currentScene}");
        onDisasterEnd?.Invoke();
        currentScene.onDisasterEnd?.Invoke();

        // Wait till a current distraction is resolved
        while (dialogueRunner.isDialogueRunning)
        {
            yield return null;
        }

        // Trigger end dialogue
        Debug.Log($"Start End Dialogue for Scene {currentScene}");
        dialogueRunner.StartDialogue(currentScene.endNode);

        // Wait till its done
        while (dialogueRunner.isDialogueRunning)
        {
            yield return null;
        }

        // End of scene
        Debug.Log($"End of scene {currentScene}");
        onSceneEnd?.Invoke();
        currentScene.onSceneEnd?.Invoke();
        waitForSceneStart = true;
        currentSceneIndex++;
        yield return null;
    }

    public float GetPanicRate()
    {
        return scenes[currentSceneIndex].disasterMeter;
    }
}
