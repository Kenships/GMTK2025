using Obvious.Soap;
using PrimeTween;
using UnityEngine;

public class GameScreenAnimation : MonoBehaviour
{
    [SerializeField] private ScriptableEventNoParam startCountDownEvent;

    [SerializeField] private Transform disc;
    [SerializeField] private Transform discEnd;
    [SerializeField] private Transform circle;
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    
    void Start()
    {
        Sequence.Create(Tween.Position(
            target: circle,
            startValue: start.position,
            endValue: end.position,
            duration: 0.5f,
            ease: Ease.InOutExpo,
            cycles: 1,
            startDelay: 0.5f)).ChainCallback(() => disc.SetParent(circle.parent)).Chain(
            Sequence.Create(Tween.Scale(
                target: circle,
                startValue: circle.localScale,
                endValue: end.localScale,
                duration: 0.5f,
                ease: Ease.InOutExpo,
                cycles: 1
            )).Group(
                Tween.Scale(
                    target: disc,
                    startValue: disc.localScale,
                    endValue: discEnd.localScale,
                    duration: 0.5f,
                    ease: Ease.InOutExpo,
                    cycles: 1
                    )
                )).OnComplete(() =>
        {
            circle.gameObject.SetActive(false);
            disc.gameObject.SetActive(false);
            startCountDownEvent.Raise();
        });
    }
}
