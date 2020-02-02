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
        if (giftRigidbody != null) return;

        if (other.TryGetComponent<Attachable>(out var gift))
        {
            Debug.Log("Received a gift! " + gift.name);
            Destroy(gift); // get rid of attachable
            giftRigidbody = other.attachedRigidbody;
            giftRigidbody.isKinematic = true;
            giftRigidbody.Sleep();
            // TODO this doesnt work :(
            giftRigidbody.velocity = Vector3.zero;

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
            giftRigidbody.WakeUp();
        }
    }
}
