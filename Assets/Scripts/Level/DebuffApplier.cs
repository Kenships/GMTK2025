using System;
using JetBrains.Annotations;
using Level;
using NUnit.Framework;
using Obvious.Soap;
using ScoreManager;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using ImprovedTimers;
using UnityEditor.Rendering.Analytics;
using UnityEngine;

public class DebuffApplier : MonoBehaviour
{
    [SerializeField] ScriptableEventLevelDataSO startGame;
    
    [SerializeField] SerializedDictionary<ScoreModifierEnum, Sprite> debuffToSprite;

    [SerializeField]
    private FloatVariable debuffCooldown;
    
    private List<ScoreModifierEnum> debuffs;
    private ScoreManager.ScoreManager scoreManager;
    private CountdownTimerRepeat countdownTimer;
    
    private int currentIndex;

    private void Start()
    {
        startGame.OnRaised += StartTimer;
    }
    
    private void OnDestroy()
    {
        countdownTimer.Dispose();
        startGame.OnRaised -= StartTimer;
    }
    
    private void Update()
    {
        debuffCooldown.Value = countdownTimer.Progress;
    }

    private void StartTimer(LevelDataSO obj)
    {
        countdownTimer = new CountdownTimerRepeat(obj.debuffCooldown, 99999);
        countdownTimer.OnTimerRaised += ApplyDebuff;
        countdownTimer.Start();
    }

    

    private void ApplyDebuff()
    {
        ModifierInstance modifier = new ModifierInstance()
        {
            LifeTime = ScriptableObject.CreateInstance<IntVariable>(),
            Modifier = debuffs[currentIndex],
        };
        scoreManager.AddModifier(modifier, debuffToSprite[modifier.Modifier]);
        
        currentIndex = (currentIndex + 1) % debuffs.Count;
    }

    
}
