using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Random = UnityEngine.Random;

public class DateController : MonoBehaviour
{
    #region Fields

    [Header("Read only")]
    [SerializeField] protected float panicMeter = 0f;
    [SerializeField] protected bool isAttentive = true;

    [Header("Settings")]
    [SerializeField] private float panicDecreaseWhenAttentive = 0.01f;
    [SerializeField] protected Renderer skinRenderer;
    [SerializeField] protected Gradient panicGradient;
    [SerializeField] protected List<Transform> eyes;
    [SerializeField] private float unattentiveEyeRollAmount = 1f;
    [SerializeField] private float unattentiveEyeRollFrequency = 1f;

    protected DialogueRunner dialogueRunner;
    protected SceneManager sceneManager;
    protected bool isDisasterRunning;

    #endregion


    #region Property

    protected bool IsAttentive
    {
        get
        {
            if (DialogueRunner != null)
            {
                isAttentive = DialogueRunner.isDialogueRunning;
            }
            else
            {
                isAttentive = false;
            }

            return isAttentive;
        }
    }

    public DialogueRunner DialogueRunner
    {
        get
        {
            if (dialogueRunner == null)
            {
                dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
            }

            return dialogueRunner;
        }
    }

    public SceneManager SceneManager
    {
        get
        {
            if (sceneManager == null)
            {
                sceneManager = FindObjectOfType<SceneManager>();
            }

            return sceneManager;
        }
    }

    #endregion


    #region Unity methods

    protected void OnEnable()
    {
        if (SceneManager)
        {
            SceneManager.onDisasterStart.AddListener(OnDistasterStart);
            SceneManager.onDisasterEnd.AddListener(OnDistasterEnd);
        }
    }

    protected void OnDisable()
    {
        if (SceneManager)
        {
            SceneManager.onDisasterStart.RemoveListener(OnDistasterStart);
            SceneManager.onDisasterEnd.RemoveListener(OnDistasterEnd);
        }
    }

    protected void Update()
    {
        UpdatePanicMeter();
        UpdateSkinColor();
        UpdateEyes();
    }

    private void UpdateEyes()
    {
        foreach (var eye in eyes)
        {
            if (IsAttentive)
            {
                eye.LookAt(Camera.main.transform);
            }
            else
            {
                eye.rotation = Quaternion.Slerp(eye.rotation, UnityEngine.Random.rotation, Time.deltaTime * unattentiveEyeRollFrequency);
            }
        }
    }

    #endregion


    #region Date methods

    Vector3 AddNoiseOnAngle(float min, float max)
    {
        // Find random angle between min & max inclusive
        float xNoise = Random.Range(min, max);
        float yNoise = Random.Range(min, max);
        float zNoise = Random.Range(min, max);

        // Convert Angle to Vector3
        Vector3 noise = new Vector3(
          Mathf.Sin(2 * Mathf.PI * xNoise / 360),
          Mathf.Sin(2 * Mathf.PI * yNoise / 360),
          Mathf.Sin(2 * Mathf.PI * zNoise / 360)
        );
        return noise;
    }

    private void UpdatePanicMeter()
    {
        if (isDisasterRunning && !IsAttentive)
        {
            panicMeter += SceneManager.GetPanicRate() * Time.deltaTime;
        }

        if (IsAttentive)
        {
            panicMeter -= panicDecreaseWhenAttentive * Time.deltaTime;
        }

        panicMeter = Mathf.Clamp01(panicMeter);
    }

    private void UpdateSkinColor()
    {
        skinRenderer.material.color = panicGradient.Evaluate(panicMeter);
    }

    [ContextMenu("Start disaster")]
    protected void OnDistasterStart()
    {
        isDisasterRunning = true;
    }

    protected void OnDistasterEnd()
    {
        isDisasterRunning = false;
    }

    #endregion
}
