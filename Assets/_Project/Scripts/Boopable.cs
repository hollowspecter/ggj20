using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boopable : MonoBehaviour
{
    public UnityEvent onBooped;

    protected Tongue attachedTongue;

    public Tongue AttachedTongue { get => attachedTongue; set => attachedTongue = value; }

    public virtual void Boop(Tongue _tongue)
    {
        onBooped?.Invoke();
    }
}