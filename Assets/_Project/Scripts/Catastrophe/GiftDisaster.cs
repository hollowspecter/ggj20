﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GiftDisaster : MonoBehaviour
{
    public List<GameObject> goodGifts = new List<GameObject>();
    public UnityEvent onGoodGiftReceived;
    public UnityEvent onBadGiftReceived;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<Attachable>(out var gift))
        {
            if (goodGifts.Contains(gift.gameObject))
            {
                onGoodGiftReceived?.Invoke();
            }
            else
            {
                onBadGiftReceived?.Invoke();
            }
        }
    }
}
