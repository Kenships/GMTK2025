using System;
using Obvious.Soap;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

namespace Level
{
    public class EndSequence : MonoBehaviour
    {
        
        [SerializeField] ScriptableEventLevelDataSO onGameEnd;
        [SerializeField] ScoreManager.ScoreManager scoreManager;

        [SerializeField] private GameObject endScreen;
        [SerializeField] private IntVariable money;
        [SerializeField] private IntVariable moneyGained;
        [SerializeField] private IntVariable score;
        [SerializeField] private IntVariable overScore;
        
        
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
            ApplyFinalModifiers();

            if (LevelData.highScore.Value < score.Value)
            {
                LevelData.highScore.Value = score.Value;
            }
            
            overScore.Value = score.Value - LevelData.minimumWinScore;

            int totalMoneyGained = LevelData.endOfRoundCredits + OverScoreToMoney();
            
            moneyGained.Value = totalMoneyGained;
            
            money.Value += totalMoneyGained;
            
            endScreen.SetActive(true);
        }

        private int OverScoreToMoney()
        {
            return overScore.Value;
        }

        private void ApplyFinalModifiers()
        {
            
        }
    }
}
