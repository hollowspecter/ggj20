using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonFun : MonoBehaviour
{

    public ParticleSystem ps;
    public RectTransform rt;
    public float offset = 10F;
    public float duration = 0.4F;
    private Button _button;
    private Vector3 _defaultPos;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _defaultPos = rt.anchoredPosition;
    }


    public void HoverEnter()
    {
        var endVal = offset + _defaultPos.y;
        rt.DOAnchorPosY(endVal, duration);
        ps.Play();
    }

    public void HoverExit()
    {
        rt.DOAnchorPosY(_defaultPos.y, duration);
        ps.Stop();
    }
}
