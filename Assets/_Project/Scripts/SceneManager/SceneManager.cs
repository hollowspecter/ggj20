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

    private bool waitForSceneStart = true;
    private int currentSceneIndex = 0;
    private Yarn.Unity.DialogueRunner dialogueRunner;

    private void Awake()
    {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();

        // load all the scripts
        for (int i=0; i < scenes.Length; ++i)
        {
            dialogueRunner.Add(scenes[i].startDialogue);
            dialogueRunner.Add(scenes[i].endDialogue);
            for(int j=0; j<scenes[i].distractions.Length; ++j)
            {
                dialogueRunner.Add(scenes[i].distractions[j].distractionDialogue);
            }
        }
    }

    public void StartScene()
    {
        if (waitForSceneStart)
        {
            waitForSceneStart = false;
        }
    }

    private IEnumerator RunScene()
    {
        // Fetch the next scene
        SceneData currentScene = scenes[currentSceneIndex++];

        // On Scene Start
        currentScene.onSceneStart?.Invoke();
        onSceneStart?.Invoke();
        dialogueRunner.StartDialogue(currentScene.startNode);

        // Wait till dialogue is done
        while (dialogueRunner.isDialogueRunning) {
            yield return null;
        }

        // Invoke disaster start
        onDisasterStart?.Invoke();
        currentScene.onDisasterStart?.Invoke();

        // Wait till disaster is over
        while (currentScene.DisasterRunning)
        {
            yield return null;
        }

        // Invoke disaster end
        onDisasterEnd?.Invoke();
        currentScene.onDisasterEnd?.Invoke();

        // Wait till a current distraction is resolved
        while (dialogueRunner.isDialogueRunning)
        {
            yield return null;
        }

        // Trigger end dialogue
        dialogueRunner.StartDialogue(currentScene.endNode);

        // Wait till its done
        while (dialogueRunner.isDialogueRunning)
        {
            yield return null;
        }

        // End of scene
        onSceneEnd?.Invoke();
        currentScene.onSceneEnd?.Invoke();
        waitForSceneStart = true;
        yield return null;
    }
}
