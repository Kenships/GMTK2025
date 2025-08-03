using Obvious.Soap;
using UnityEngine;
using System.Collections.Generic;

namespace Level
{
    [CreateAssetMenu(fileName = "LevelDataSO", menuName = "Scriptable Objects/LevelDataSO")]
    public class LevelDataSO : ScriptableObject
    {
        public string levelName;
        public int minimumWinScore;
        public int numberOfBars;
        public int endOfRoundCredits;
        public List<ScoreModifierEnum> debuffs;
        public IntVariable highScore;
    }
}
