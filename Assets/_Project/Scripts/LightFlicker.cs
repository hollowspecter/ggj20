using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light light;

    private void Start()
    {
        light.DOIntensity(13.5f, .2f).SetLoops(-1, LoopType.Yoyo);
    }
}
