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
        Prepare,
        Shooting,
        Stuck,
        Retracting,
        Holding,
    }

    protected const float maxShootingTime = 3f;

    [SerializeField] protected State currentState = State.In;
    [Space]
    [SerializeField] protected LineRenderer tongueLine;
    [SerializeField] protected Transform mouthStart;
    [SerializeField] private float tongueShootForce = 4f;
    [SerializeField] private float attachableShootForce = 4f;
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

        transform.parent.SetParent(null);
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
                        Retract();
                    }
                    //else
                    //{
                    //    if (Input.GetButtonDown("Fire1"))
                    //    {
                    //        Retract();
                    //    }
                    //    else
                    //    {
                    //        var input = Vector2.zero;
                    //        input.x = Input.GetAxis("Mouse X");
                    //        input.y = Input.GetAxis("Mouse Y");
                    //        var vectorMouthToEnd = transform.position - mouthStart.position;
                    //        rigidbody.AddForce(input, ForceMode.Force);
                    //    }
                    //}
                }
                break;
            case State.Retracting:
                {
                    var t = (Time.time - timeWhenRetractionStarted) / retractionDuration;
                    t = t * t;
                    transform.position = Vector3.Lerp(transform.position, mouthStart.position, t);
                    if (t >= 1f)
                    {
                        if (currentAttachable != null)
                        {
                            currentState = State.Holding;
                        }
                        else
                        {
                            currentState = State.In;

                            rigidbody.angularVelocity = Vector3.zero;
                            rigidbody.velocity = Vector3.zero;
                            rigidbody.isKinematic = true;
                        }

                        //if (currentAttachable != null && TryGetComponent<FixedJoint>(out var joint))
                        //{
                        //    Destroy(joint);
                        //    currentAttachable = null;
                        //}
                    }
                }
                break;
            case State.Prepare:
                break;
            case State.Holding:
                {
                    // Stick tongue to mouth.
                    transform.position = mouthStart.position;

                    if (Input.GetButtonDown("Fire1") && currentAttachable != null)
                    {
                        currentState = State.In;

                        if (TryGetComponent<FixedJoint>(out var joint))
                        {
                            Destroy(joint);
                        }
                        currentAttachable.Rigidbody.AddForce(Camera.main.transform.forward * attachableShootForce, ForceMode.Impulse);
                        currentAttachable = null;
                    }
                }
                break;
            default:
                break;
        }

        UpdateTongueRenderer();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter @ " + Time.frameCount + " with " + collision.collider.name);

        if (currentState != State.Shooting)
            return;

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
        var joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = currentAttachable.Rigidbody;
        joint.massScale = 0.01f;
        Debug.Break();
    }

    #endregion

    #region Tongue methods

    private void Retract()
    {
        if (currentState != State.Stuck && currentState != State.Shooting)
            return;

        currentState = State.Retracting;

        timeWhenRetractionStarted = Time.time;
    }

    private void Shoot()
    {
        if (currentState != State.In)
            return;

        currentState = State.Shooting;

        rigidbody.isKinematic = false;
        rigidbody.AddForce(Camera.main.transform.forward * attachableShootForce, ForceMode.Impulse);
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
