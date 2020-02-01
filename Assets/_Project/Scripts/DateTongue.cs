using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateTongue : MonoBehaviour
{
    [SerializeField] private LineRenderer tongueRenderer;
    [SerializeField] private List<Transform> tongueRopeTransforms = new List<Transform>();
    [SerializeField] private Transform frenchKissTarget;
    [SerializeField] private float tongueSpeed = 1f;

    private bool isFrenchKissing;

    private Transform TongueStart
    {
        get => tongueRopeTransforms.Count > 0 ? tongueRopeTransforms[0] : null;
    }

    private Transform TongueEnd
    {
        get => tongueRopeTransforms.Count > 0 ? tongueRopeTransforms[tongueRopeTransforms.Count - 1] : null;
    }

    private void Update()
    {
        if (TongueStart && TongueEnd)
        {
            UpdatePositions();
            UpdateRenderer();
        }
    }

    private void UpdateRenderer()
    {
        var showTongue = isFrenchKissing;
        tongueRenderer.forceRenderingOff = !showTongue;
        if (showTongue)
        {
            tongueRenderer.positionCount = tongueRopeTransforms.Count;
            tongueRenderer.SetPositions(tongueRopeTransforms.Select(x => x.position).ToArray());
        }
    }

    private void UpdatePositions()
    {
        if (isFrenchKissing)
        {
            TongueEnd.position = Vector3.Lerp(TongueEnd.position, frenchKissTarget.position, Time.deltaTime * tongueSpeed);
            foreach (var item in tongueRopeTransforms)
            {
                if (item != TongueStart && item != TongueEnd && item.TryGetComponent<Rigidbody>(out var rigidbody))
                {
                    rigidbody.isKinematic = false;
                }
            }
        }
        else
        {
            foreach (var item in tongueRopeTransforms)
            {
                if (item.TryGetComponent<Rigidbody>(out var rigidbody))
                {
                    rigidbody.MovePosition(TongueStart.position);
                    rigidbody.isKinematic = true;
                }
            }
        }
    }

    [ContextMenu("Start French Kiss")]
    public void StartFrenchKiss()
    {
        isFrenchKissing = true;
    }
}
