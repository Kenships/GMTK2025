using System;
using ImprovedTimers;
using Obvious.Soap;
using PrimeTween;
using ScoreManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Level
{
    public class EndSequence : MonoBehaviour
    {
        [SerializeField] GameStateSO gameState;
        
        [SerializeField] ScriptableEventLevelDataSO onGameEnd;
        [SerializeField] ScoreManager.ScoreManager scoreManager;

        [SerializeField] private GameObject endScreen;
        [SerializeField] private IntVariable money;
        [SerializeField] private IntVariable moneyGained;
        [SerializeField] private IntVariable score;
        [SerializeField] private IntVariable overScore;

        private AudioSource audioSource;
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            onGameEnd.OnRaised += OnGameEndOnOnRaised;
        }

        private void OnDestroy()
        {
            onGameEnd.OnRaised -= OnGameEndOnOnRaised;
        }

        private void OnGameEndOnOnRaised(LevelDataSO LevelData)
        {
            audioSource.Play();
            ApplyFinalModifiers();

            if (LevelData.highScore.Value < score.Value)
            {
                LevelData.highScore.Value = score.Value;
            }
            
            overScore.Value = score.Value - LevelData.minimumWinScore;

            int totalMoneyGained = LevelData.endOfRoundCredits + OverScoreToMoney();
            
            moneyGained.Value = totalMoneyGained;
            
            money.Value += totalMoneyGained;

            if (overScore.Value >= 0)
            {
                gameState.currentLevel.Value++;
            }
            
            CountdownTimer countdownTimer = new CountdownTimer(3f);
            countdownTimer.OnTimerEnd += () => endScreen.SetActive(true);
            countdownTimer.Start();
        }

        private int OverScoreToMoney()
        {
            return overScore.Value;
        }

        private void ApplyFinalModifiers()
        {
            foreach (ModifierInstance modifier in scoreManager.modifiers) 
            {
                if (modifier.Modifier.Equals(ScoreModifierEnum.LastSongPlayed) && modifier.LifeTime.Value > 0)
                {
                    scoreManager.Score.Value *= 3;
                }
                else if (modifier.Modifier.Equals(ScoreModifierEnum.BandTogether) && modifier.LifeTime.Value > 3000)) 
                {
                    scoreManager.Score.Value = (int)(scoreManager.Score.Value * 1.2f);
                }
            }
        }
    }
}
