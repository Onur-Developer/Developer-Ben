using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HeadCount : MonoBehaviour
{
    private float _headCount;
    public Slider slider;
    private Tween _flagAnim;
    [SerializeField] private Transform flag;

    private void OnEnable()
    {
        flag.localScale = Vector3.one;
        slider.value = 0;
    }

    private void OnDisable()
    {
        _headCount = 0;
        if (_flagAnim != null)
            _flagAnim.Kill();
        slider.value = 0;
        slider.maxValue = 0;
    }

    public void IncreaseValue()
    {
        _headCount++;
        slider.value = _headCount;
        if (slider.value >= slider.maxValue)
            FlagAnimation();
    }

    void FlagAnimation()
    {
        _flagAnim = flag.DOScale(Vector2.one * 1.5f, 1f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo).OnKill((() =>
            {
                _flagAnim = null;
            }));
    }
}