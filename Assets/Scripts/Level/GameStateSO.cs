using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;

namespace Level
{
    [CreateAssetMenu(fileName = "GameStateSO", menuName = "ScriptableObject/GameStateSO", order = 0)]
    public class GameStateSO : ScriptableObject
    {
        public BoolVariable tutorialPlayed;

        public IntVariable currentLevel;
        
        public List<LevelDataSO> levelData;
        
        public void ResetGame()
        {
            currentLevel.Value = 0;
        }

        public LevelDataSO GetCurrentLevelData()
        {
            if (currentLevel.Value == 0 && tutorialPlayed.Value)
            {
                currentLevel.Value = 1;
            }
            
            return levelData[currentLevel.Value];
        }
    }
}
