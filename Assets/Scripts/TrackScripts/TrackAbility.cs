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
        ScoreModify2xFor4Scores,
        ElectronicStreak,
        Extra3AfterWind,
        DecrementModifierLifetimes,
        IncreaseTrackPointBy2,
        Gain5Lose5,
        RepeatNextNonWind
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
                    scoreManager.addPoints(scoreManager.GetUpToDateTrack(track).points);
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
            }},
            {TrackAbilityEnum.ElectronicStreak, new TrackAbility() 
            {
                startAction = (scoreManager, track, playlist) =>
                {
                    
                },
                endAction = (scoreManager, track, playlist) =>
                {
                    ModifierInstance modifier = new ModifierInstance()
                    {
                        LifeTime = 1,
                        Modifier = ScoreModifierEnum.ElectronicStreak,
                    };
                    scoreManager.AddModifier(modifier);
                    Action<TrackSO> callback = (track) => ScoreModifiers.enumToModifier[ScoreModifierEnum.ElectronicStreak](modifier, track, scoreManager, 0, ScoreContextEnum.TrackStart);
                    modifier.callback = callback;
                    scoreManager.TrackPlayer.SongStart += callback;
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.Extra3AfterWind, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    List<TrackSO> history = scoreManager.TrackPlayer.trackHistory;
                    if(history.Count-2 >= 0)
                    {
                        if (history[history.Count-2].tags.Contains(Tag.Wind) || history[history.Count-2].tags.Contains(Tag.MusicBox))
                        {
                            scoreManager.addPoints(3);
                        } 
                    }
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.DecrementModifierLifetimes, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.AffectModifierLifetime(ScoreModifierEnum.All, (_, lifetime) => lifetime - 1);
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.IncreaseTrackPointBy2, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.GetUpToDateTrack(track).points += 2;
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.Gain5Lose5, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.addPoints(5);
                    ModifierInstance modifier = new ModifierInstance()
                    {
                        LifeTime = 1,
                        Modifier = ScoreModifierEnum.Lose5,
                    };
                    scoreManager.AddModifier(modifier);
                    Action<TrackSO> callback = (track) => ScoreModifiers.enumToModifier[ScoreModifierEnum.Lose5](modifier, track, scoreManager, 0, ScoreContextEnum.TrackStart);
                    modifier.callback = callback;
                    scoreManager.TrackPlayer.SongStart += callback;
                },
                endAction = (scoreManager, track, playlist) =>
                {
                    
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.RepeatNextNonWind, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    ModifierInstance modifier = new ModifierInstance()
                    {
                        LifeTime = 1,
                        Modifier = ScoreModifierEnum.RepeatNonWind,
                    };
                    scoreManager.AddModifier(modifier);
                    Action<TrackSO> callback = (track) => ScoreModifiers.enumToModifier[ScoreModifierEnum.RepeatNonWind](modifier, track, scoreManager, 0, ScoreContextEnum.TrackStart);
                    modifier.callback = callback;
                    scoreManager.TrackPlayer.SongStart += callback;
                },
                timestampActions = new List<TimestampAction>()
            }},

        };
    }
}
