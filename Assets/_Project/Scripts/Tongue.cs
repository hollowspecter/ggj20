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
    [SerializeField] private float tongueMaxLength = 10f;
    [SerializeField] private float attachableShootForce = 4f;
    [SerializeField] protected float retractionDuration = 0.5f;
    [SerializeField] protected LayerMask boopableLayerMask;
    protected bool doRetractAttachablesAutomatically = true;
    private float tongueShootDuration;
    protected float timeWhenShot;
    protected float timeWhenCollided;
    protected float timeWhenRetractionStarted;
    protected new Rigidbody rigidbody;
    protected Attachable currentAttachable;
    private Vector3 tongueTargetPosition;
    private Boopable boopableThatWeAreGoingToHit;

    #region Unity methods

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        transform.SetParent(null);
    }

    private void FixedUpdate()
    {
        if (rigidbody.IsSleeping())
            rigidbody.WakeUp();
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
                    var t = (Time.time - timeWhenShot) / tongueShootDuration;
                    transform.position = Vector3.Lerp(mouthStart.position, tongueTargetPosition, EasingFunction.EaseOutBounce(0f, 1f, t));
                    if (t >= 1f)
                    {
                        // Exit state.
                        if (boopableThatWeAreGoingToHit != null)
                        {
                            boopableThatWeAreGoingToHit.Boop(this);
                            boopableThatWeAreGoingToHit = null;
                        }

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
                        }
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

    #endregion

    #region Public methods

    public void AttachAttachable(Attachable _attachable)
    {
        currentAttachable = _attachable;
        var joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = currentAttachable.Rigidbody;
        joint.massScale = 0.01f;
    }

    #endregion

    #region Tongue methods

    private void Retract()
    {
        if (currentState != State.Stuck && currentState != State.Shooting)
            return;

        currentState = State.Retracting;

        timeWhenRetractionStarted = Time.time;

        rigidbody.isKinematic = true;
    }

    private void Shoot()
    {
        if (currentState != State.In)
            return;

        currentState = State.Shooting;

        if (Physics.Raycast(mouthStart.position, Camera.main.transform.forward, out var raycastHit, tongueMaxLength, boopableLayerMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log("Hit object: " + raycastHit.collider.name + " @ " + Time.frameCount);
            if (raycastHit.collider.TryGetComponent<Boopable>(out var boopable))
            {
                Debug.Log("Booping boopable " + boopable.name);
                boopableThatWeAreGoingToHit = boopable;
            }

            tongueTargetPosition = raycastHit.point;
        }
        else
        {
            Debug.Log("Missed shot @ " + Time.frameCount);
            tongueTargetPosition = mouthStart.position + Camera.main.transform.forward * tongueMaxLength;
        }

        tongueShootDuration = Vector3.Distance(mouthStart.position, tongueTargetPosition) / tongueShootForce;
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
