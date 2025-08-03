using System;
using PrimeTween;
using UnityEngine;

public class StartScreenAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject disc;

    [SerializeField]
    private Transform circle;

    [SerializeField]
    private Transform start;
    
    [SerializeField]
    private Transform end;
    

    private void Start()
    {
        Sequence.Create(Tween.Position(
            target: circle,
            startValue: start.position,
            endValue: end.position,
            duration: 0.5f,
            ease: Ease.InOutExpo,
            cycles: 1,
            startDelay: 0.5f)).ChainCallback(() => disc.SetActive(false)).Chain(
            Tween.Scale(
                target: circle,
                startValue: circle.localScale,
                endValue: end.localScale,
                duration: 0.5f,
                cycles: 1
            )).OnComplete(() => circle.gameObject.SetActive(false));
    }
}
