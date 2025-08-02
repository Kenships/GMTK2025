using System.Collections.Generic;
using System;
using System.Linq;
using ImprovedTimers;
using ScoreManager;
using TrackScripts;
using UnityEngine;

namespace DefaultNamespace
{
    public enum TrackAbilityEnum
    {
        None,
        AudioSpeedBoost1_2x,
        ScoreAgainOnEnd,
        ScoreModify2xFor4Scores
    }

    public struct TimestampAction 
    {
        public readonly float audioTime;
        public Action<ScoreManager.ScoreManager, TrackSO, List<TrackSO>> Action;
        
        public TimestampAction(float audioTime, Action<ScoreManager.ScoreManager, TrackSO, List<TrackSO>> action)
        {
            this.audioTime = audioTime;
            this.Action = action;
        }
    }
    public struct TrackAbility 
    {
        public Action<ScoreManager.ScoreManager, TrackSO, List<TrackSO>> startAction;
        public Action<ScoreManager.ScoreManager, TrackSO, List<TrackSO>> endAction;
        public List<TimestampAction> timestampActions;
    }

    public class TrackAbilities 
    {
        public static Dictionary<TrackAbilityEnum, TrackAbility> EnumToAbility = new() 
        {
            {TrackAbilityEnum.ScoreAgainOnEnd, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {
                    
                }, 
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.ScorePoints(track, track.points);
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.ScoreModify2xFor4Scores, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {
                    
                }, 
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.AddModifier(new ModifierInstance()
                    {
                        LifeTime = 4,
                        Modifier = ScoreModifierEnum.X2
                    });
                },
                timestampActions = new List<TimestampAction>()
            }}
            
        };
    }
}
