using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
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

    [SerializeField] private State currentState = State.In;
    [Space]
    [SerializeField] protected LineRenderer tongueLine;
    public Transform mouthStart;
    [SerializeField] private float tongueShootForce = 4f;
    [SerializeField] private float tongueMaxLength = 10f;
    [SerializeField] private float attachableShootForce = 4f;
    [SerializeField] protected float retractionDuration = 0.5f;
    [SerializeField] protected LayerMask boopableLayerMask;
    [SerializeField] protected List<Transform> tongueRopeTransforms = new List<Transform>();
    public CinemachineVirtualCamera vcamNormal;
    public CinemachineVirtualCamera vcamPreparing;
    [SerializeField] protected float timeScalePreparing = 0.8f;
    [SerializeField] protected Canvas canvasReticle;
    protected bool doRetractAttachablesAutomatically = true;
    private float tongueShootDuration;
    protected float timeWhenShot;
    protected float timeWhenCollided;
    protected float timeWhenRetractionStarted;
    protected new Rigidbody rigidbody;
    protected Attachable currentAttachable;
    private Vector3 tongueTargetPosition;
    private Boopable boopableThatWeAreGoingToHit;
    private Quaternion previousMouthRotation;

    #region Unity methods

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        transform.SetParent(null);
        vcamPreparing.enabled = false;
        vcamNormal.enabled = true;
        canvasReticle.enabled = false;
    }

    private void FixedUpdate()
    {
        if (rigidbody.IsSleeping())
            rigidbody.WakeUp();
    }

    private void Update()
    {
        canvasReticle.enabled = currentState == State.Prepare;

        switch (currentState)
        {
            case State.In:
                {
                    transform.position = mouthStart.position;

                    if (Input.GetButtonDown("Fire1"))
                    {
                        Shoot();
                    }
                    else if (Input.GetButtonDown("Fire2"))
                    {
                        currentState = State.Prepare;

                        Time.timeScale = timeScalePreparing;
                        vcamPreparing.enabled = true;
                        vcamNormal.enabled = false;
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
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        Shoot();
                    }
                    else if (Input.GetButtonUp("Fire2"))
                    {
                        currentState = State.In;
                    }

                    // Exit state.
                    if (currentState != State.Prepare)
                    {
                        Time.timeScale = 1f;
                        vcamPreparing.enabled = false;
                        vcamNormal.enabled = true;
                    }
                }
                break;
            case State.Holding:
                {
                    // Stick tongue to mouth.
                    transform.position = mouthStart.position;
                    transform.rotation = transform.rotation * (mouthStart.rotation * Quaternion.Inverse(previousMouthRotation));

                    if (Input.GetButtonDown("Fire1") && currentAttachable != null)
                    {
                        currentState = State.In;

                        if (TryGetComponent<FixedJoint>(out var joint))
                        {
                            Destroy(joint);
                        }
                        currentAttachable.Rigidbody.AddForce(CameraManager.instance.currentControlledCamera.transform.forward * attachableShootForce, ForceMode.Impulse);
                        currentAttachable = null;
                    }
                }
                break;
            default:
                break;
        }

        UpdateTongueRenderer();

        previousMouthRotation = mouthStart.rotation;
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
        print("cam: " + CameraManager.instance.currentControlledCamera);
        print("forward: " + CameraManager.instance.currentControlledCamera.transform.forward);
        if (currentState != State.In && currentState != State.Prepare)
            return;

        currentState = State.Shooting;

        if (Physics.Raycast(mouthStart.position, CameraManager.instance.currentControlledCamera.transform.forward, out var raycastHit, tongueMaxLength, boopableLayerMask, QueryTriggerInteraction.Collide))
        {
            //Debug.Log("Hit object: " + raycastHit.collider.name + " @ " + Time.frameCount);
            if (raycastHit.collider.TryGetComponent<Boopable>(out var boopable))
            {
                //Debug.Log("Booping boopable " + boopable.name);
                boopableThatWeAreGoingToHit = boopable;
            }

            tongueTargetPosition = raycastHit.point;
        }
        else
        {
            //Debug.Log("Missed shot @ " + Time.frameCount);
            tongueTargetPosition = mouthStart.position + CameraManager.instance.currentControlledCamera.transform.forward * tongueMaxLength;
        }

        for (int i = 1; i < tongueRopeTransforms.Count - 1; i++)
        {
            if (tongueRopeTransforms[i].TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = false;
            }
        }

        tongueShootDuration = Vector3.Distance(mouthStart.position, tongueTargetPosition) / tongueShootForce;
        timeWhenShot = Time.time;
    }

    #endregion

    private void UpdateTongueRenderer()
    {
        var isTongueMoving = currentState == State.Shooting || currentState == State.Retracting;
        if (isTongueMoving)
        {
            tongueLine.positionCount = tongueRopeTransforms.Count;
            tongueLine.SetPositions(tongueRopeTransforms.Select(x => x.position).ToArray());
        }

        tongueLine.forceRenderingOff = !isTongueMoving;

        // Set tongue rope rigidbody kinematic state.
        for (int i = 1; i < tongueRopeTransforms.Count - 1; i++)
        {
            if (tongueRopeTransforms[i].TryGetComponent<Rigidbody>(out var rb))
            {
                if (!isTongueMoving)
                {
                    rb.MovePosition(mouthStart.position);
                }
                rb.isKinematic = !isTongueMoving;
            }
        }
    }
}
