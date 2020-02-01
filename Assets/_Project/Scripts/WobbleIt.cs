using UnityEngine;
using DG.Tweening;

public class WobbleIt : MonoBehaviour
{
    public float duration = 1;
    public Vector3 strength = new Vector3(1,1,1);
    public int vibrato = 18;
    [Range(0F,80F)]public float randomness = 50F;
    
    public void Wobble()
    {
        transform.DOShakeScale(duration, strength, vibrato, randomness, true);
    }
}
