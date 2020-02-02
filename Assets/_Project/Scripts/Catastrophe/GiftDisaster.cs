using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GiftDisaster : MonoBehaviour
{
    public List<GameObject> goodGifts = new List<GameObject>();
    public UnityEvent onGoodGiftReceived;
    public UnityEvent onBadGiftReceived;

    private Rigidbody giftRigidbody;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Attachable>(out var gift))
        {
            Debug.Log("Received a gift! " + gift.name);
            Destroy(gift); // get rid of attachable
            other.attachedRigidbody.isKinematic = true;
            giftRigidbody = other.attachedRigidbody;

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

    public void LetGoOfGift()
    {
        if (giftRigidbody != null)
        {
            giftRigidbody.isKinematic = false;
        }
    }
}
