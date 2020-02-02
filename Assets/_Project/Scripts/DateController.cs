using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Random = UnityEngine.Random;
using DG.Tweening;

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
    [SerializeField] protected List<Transform> defaultEyes;

    [Range(0.0f, 45.0f)] [SerializeField] private float unattentiveEyeRollMin = 0.0f;
    [Range(0.0f, 45.0f)] [SerializeField] private float unattentiveEyeRollMax = 45.0f;
    [Range(0.0f, 10.0f)] [SerializeField] private float unattentiveEyeRollIntervall = 1f;
    [Range(0.0f, 3.0f)] [SerializeField] private float unattentiveEyeRollDuration = 0.5f;
    private float unattentiveEyeRollCooldown = 0.0f;

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
        bool lookAround = Mathf.Approximately(unattentiveEyeRollCooldown, unattentiveEyeRollIntervall);

        for (int i = 0; i < eyes.Count; ++i)
        {
            var eye = eyes[i];
            var defaultEye = defaultEyes[i];
            if (IsAttentive && lookAround)
            { 
                eye.DOLookAt(CameraManager.instance.currentControlledCamera.transform.position, unattentiveEyeRollDuration).SetEase(Ease.InOutSine);
            }
            else if (lookAround)
            {
                float randomX = Random.Range(unattentiveEyeRollMin, unattentiveEyeRollMax);
                if (Random.Range(0.0f, 1.0f) >= .5f)
                    randomX *= -1.0f;
                Quaternion x = Quaternion.AngleAxis(randomX, Vector3.up);


                float randomY = Random.Range(unattentiveEyeRollMin, unattentiveEyeRollMax);
                if (Random.Range(0.0f, 1.0f) >= .5f)
                    randomY *= -1.0f;

                Quaternion y = Quaternion.AngleAxis(randomY, Vector3.right);

                Quaternion q = defaultEye.rotation * y * x;
                eye.DORotate(q.eulerAngles, unattentiveEyeRollDuration).SetEase(Ease.InOutSine);
            }
        }
        if (lookAround)
        {
            lookAround = false;
            unattentiveEyeRollCooldown = 0.0f;
        }
        else
        {
            unattentiveEyeRollCooldown = Mathf.Clamp(unattentiveEyeRollCooldown + Time.deltaTime, 0.0f, unattentiveEyeRollIntervall);
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
