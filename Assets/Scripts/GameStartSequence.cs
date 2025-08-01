using System;
using ImprovedTimers;
using Obvious.Soap;
using UnityEngine;

public class GameStartSequence : MonoBehaviour
{
    [SerializeField] IntVariable countDownTime;

    [SerializeField]
    private ScriptableEventNoParam startGame;

    private CountdownTimer countDownTimer;

    private void Awake()
    {
        countDownTimer = new CountdownTimer(5f);
    }

    private void Start()
    {
        countDownTime.Value = 3;
        countDownTimer.OnTimerEnd += () => startGame?.Raise();
        countDownTimer.Start();
    }

    private void Update()
    {
        if (countDownTimer.CurrentTime <= 3f)
        {
            countDownTime.Value = Mathf.RoundToInt(countDownTimer.CurrentTime);
        }
    }
}
