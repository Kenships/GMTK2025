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
using UnityEngine.UI;

public class DebuffApplier : MonoBehaviour
{
    [SerializeField] ScriptableEventLevelDataSO startGame;
    
    [SerializeField] SerializedDictionary<ScoreModifierEnum, Sprite> debuffToSprite;

    [SerializeField] private FloatVariable debuffCooldown;
    [SerializeField] private StringVariable nextDebuffName;

    [SerializeField] private GameObject nextDebuffTooltip;
    [SerializeField] private ScoreManager.ScoreManager scoreManager;
    
    private List<ScoreModifierEnum> debuffs;
    
    private CountdownTimerRepeat countdownTimer;
    
    private int currentIndex;

    private int debuffLifeTime;

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
        if (countdownTimer != null)
        {
            debuffCooldown.Value = countdownTimer.Progress * 100f;
        }
        
        
    }

    private void StartTimer(LevelDataSO obj)
    {
        countdownTimer = new CountdownTimerRepeat(obj.debuffCooldown, 99999);
        debuffs = obj.debuffs;
        ShuffleDebuffs();
        nextDebuffTooltip.GetComponent<Tooltip>().Message = ScoreModifiers.enumToDescription[debuffs[currentIndex]];
        nextDebuffTooltip.GetComponent<Image>().sprite = debuffToSprite[debuffs[currentIndex]];
        
        debuffLifeTime = obj.debuffLifeTime;
        countdownTimer.OnTimerRaised += ApplyDebuff;
        countdownTimer.Start();
    }

    
    

    private void ApplyDebuff()
    {
        ModifierInstance modifier = new ModifierInstance()
        {
            LifeTime = ScriptableObject.CreateInstance<IntVariable>(),
            Modifier = debuffs[currentIndex]
        };

        modifier.LifeTime.Value = debuffLifeTime;
        
        scoreManager.AddModifier(modifier, debuffToSprite[modifier.Modifier]);
        
        currentIndex = (currentIndex + 1) % debuffs.Count;
        
        nextDebuffName.Value = debuffToSprite[debuffs[currentIndex]].name;
        nextDebuffTooltip.GetComponent<Tooltip>().Message = ScoreModifiers.enumToDescription[debuffs[currentIndex]];
        nextDebuffTooltip.GetComponent<Image>().sprite = debuffToSprite[debuffs[currentIndex]];
    }

    private void ShuffleDebuffs()
    {
        int n = debuffs.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (debuffs[i], debuffs[j]) = (debuffs[j], debuffs[i]);
        }
    }
}
