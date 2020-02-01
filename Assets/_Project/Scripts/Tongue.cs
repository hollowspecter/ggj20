using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Tongue : MonoBehaviour
{
    public enum State
    {
        In,
        Shooting,
        Stuck,
        Retracting,
        Prepare
    }

    protected const float maxShootingTime = 3f;

    [SerializeField] protected State currentState = State.In;
    [Space]
    [SerializeField] protected LineRenderer tongueLine;
    [SerializeField] protected Transform mouthStart;
    [SerializeField] private float shootForce = 4f;
    [SerializeField] protected float timeUntilRetraction = 0.2f;
    [SerializeField] protected float retractionDuration = 0.5f;
    [SerializeField] protected bool doRetractAttachablesAutomatically = true;

    protected float timeWhenShot;
    protected float timeWhenCollided;
    protected float timeWhenRetractionStarted;
    protected new Rigidbody rigidbody;
    protected Attachable currentAttachable;

    #region Unity methods

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.In:
                {
                    transform.position = mouthStart.position;

                    if (Input.GetButtonDown("Fire1"))
                    {
                        Shoot();
                    }
                }
                break;
            case State.Shooting:
                {
                    var currentShootingTime = Time.time - timeWhenShot;
                    if (currentShootingTime > maxShootingTime)
                    {
                        Retract();
                    }
                }
                break;
            case State.Stuck:
                {
                    if (doRetractAttachablesAutomatically || currentAttachable == null)
                    {
                        if (Time.time > timeWhenCollided + timeUntilRetraction)
                        {
                            Retract();
                        }
                    }
                    else
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            Retract();
                        }
                        else
                        {
                            var input = Vector2.zero;
                            input.x = Input.GetAxis("Mouse X");
                            input.y = Input.GetAxis("Mouse Y");
                            var vectorMouthToEnd = transform.position - mouthStart.position;
                            rigidbody.AddForce(input, ForceMode.Force);
                        }
                    }
                }
                break;
            case State.Retracting:
                {
                    var t = (Time.time - timeWhenRetractionStarted) / retractionDuration;
                    t = Mathf.Clamp01(t * t);
                    transform.position = Vector3.Lerp(transform.position, mouthStart.position, t);
                    if (Mathf.Approximately(t, 1f))
                    {
                        currentState = State.In;

                        rigidbody.angularVelocity = Vector3.zero;
                        if (currentAttachable != null && TryGetComponent<FixedJoint>(out var joint))
                        {
                            Destroy(joint);
                        }
                    }
                }
                break;
            case State.Prepare:
                break;
            default:
                break;
        }

        UpdateTongueRenderer();
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState = State.Stuck;
        timeWhenCollided = Time.time;

        if (collision.collider.attachedRigidbody != null)
        {
            if (collision.collider.TryGetComponent<Boopable>(out var boopable))
            {
                boopable.Boop(this);
            }
        }
    }

    #endregion

    #region Public methods

    public void AttachAttachable(Attachable _attachable)
    {
        currentAttachable = _attachable;
        currentState = State.Stuck;

        var joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = currentAttachable.Rigidbody;
    }

    #endregion

    #region Tongue methods

    private void Retract()
    {
        currentAttachable = null;
        timeWhenRetractionStarted = Time.time;
        currentState = State.Retracting;
    }

    private void Shoot()
    {
        currentState = State.Shooting;
        rigidbody.isKinematic = false;
        rigidbody.AddForce(Camera.main.transform.forward * shootForce, ForceMode.Impulse);
        timeWhenShot = Time.time;
    }

    #endregion

    private void UpdateTongueRenderer()
    {
        var tonguePositions = new Vector3[2];
        tonguePositions[0] = mouthStart.position;
        tonguePositions[1] = transform.position;
        tongueLine.SetPositions(tonguePositions);
    }
}
