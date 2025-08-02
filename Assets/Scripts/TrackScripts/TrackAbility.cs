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
        RepeatNextNonWind,
        PermGainStringLosePerc,
        GainExpiredModifier,
        LoseOnPlayed,
        GainOnPlayed,
        LastSongPlayed,
        RemoveModifier,
        Shuffle,
        SetToThree,
        GainAfterFear,
        GainNowLoseIfNoJoy,
        InvertGain,
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
                    Action<TrackSO> callback = (track) => ScoreModifiers.enumToModifier[ScoreModifierEnum.ElectronicStreak](modifier, track, scoreManager, 0, ScoreContextEnum.TrackStart, false);
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
                    scoreManager.AffectModifierLifetime(ScoreModifierEnum.All, (_, lifetime) =>
                    {
                        if (lifetime < 999) return lifetime - 1;
                        return lifetime;
                    });
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
                    Action<TrackSO> callback = (track) => ScoreModifiers.enumToModifier[ScoreModifierEnum.Lose5](modifier, track, scoreManager, 0, ScoreContextEnum.TrackStart, false);
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
                    Action<TrackSO> callback = (track) => ScoreModifiers.enumToModifier[ScoreModifierEnum.RepeatNonWind](modifier, track, scoreManager, 0, ScoreContextEnum.TrackStart, false);
                    modifier.callback = callback;
                    scoreManager.TrackPlayer.SongStart += callback;
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.PermGainStringLosePerc, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    List<TrackSO> history = scoreManager.TrackPlayer.trackHistory;
                    if(history.Count-2 >= 0)
                    {
                        if (history[history.Count-2].tags.Contains(Tag.String) || history[history.Count-2].tags.Contains(Tag.MusicBox))
                        {
                            track.points += 1;
                        }
                        if (history[history.Count-2].tags.Contains(Tag.Percussion) || history[history.Count-2].tags.Contains(Tag.MusicBox))
                        {
                            track.points -= 2;
                        }
                    }
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.GainExpiredModifier, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.addPoints(scoreManager.expiredModifiers.Count * 3);
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.LoseOnPlayed, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.addPoints(10 - 2 * scoreManager.TrackPlayer.trackHistory.Count);
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.GainOnPlayed, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.addPoints(scoreManager.TrackPlayer.trackHistory.Count);
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.LastSongPlayed, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    ModifierInstance modifier = new ModifierInstance()
                    {
                        LifeTime = 1,
                        Modifier = ScoreModifierEnum.LastSongPlayed,
                    };
                    scoreManager.AddModifier(modifier);
                    Action<TrackSO> callback = (track) => ScoreModifiers.enumToModifier[ScoreModifierEnum.LastSongPlayed](modifier, track, scoreManager, 0, ScoreContextEnum.TrackStart, false);
                    modifier.callback = callback;
                    scoreManager.TrackPlayer.SongStart += callback;
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.RemoveModifier, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.modifiers[0].LifeTime = -1;
                    foreach(ModifierInstance m in scoreManager.modifiers)
                    {
                        if (m.LifeTime < 999 && m.LifeTime > 0)
                        {
                            m.LifeTime = -1;
                            ScoreModifiers.enumToModifier[m.Modifier](m, null, scoreManager, 0, ScoreContextEnum.TimestampAction, true);
                            break;
                        }
                    }
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.Shuffle, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.TrackPlayer.Shuffle();
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.SetToThree, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.AddModifier(new ModifierInstance()
                    {
                        LifeTime = 4,
                        Modifier = ScoreModifierEnum.SetToThree
                    });
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.GainAfterFear, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    List<TrackSO> history = scoreManager.TrackPlayer.trackHistory;
                    if(history.Count-2 >= 0)
                    {
                        if (history[history.Count-2].tags.Contains(Tag.Fear) || history[history.Count-2].tags.Contains(Tag.MusicBox))
                        {
                            scoreManager.addPoints(4);
                        }
                    }
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.GainNowLoseIfNoJoy, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.addPoints(10);
                    ModifierInstance modifier = new ModifierInstance()
                    {
                        LifeTime = 3,
                        Modifier = ScoreModifierEnum.GainNowLoseIfNoJoy,
                    };
                    scoreManager.AddModifier(modifier);
                    Action<TrackSO> callback = (track) => ScoreModifiers.enumToModifier[ScoreModifierEnum.GainNowLoseIfNoJoy](modifier, track, scoreManager, 0, ScoreContextEnum.TrackStart, false);
                    modifier.callback = callback;
                    scoreManager.TrackPlayer.SongStart += callback;
                },
                timestampActions = new List<TimestampAction>()
            }},
            {TrackAbilityEnum.InvertGain, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {

                },
                endAction = (scoreManager, track, playlist) =>
                {
                    ModifierInstance modifier = new ModifierInstance()
                    {
                        LifeTime = 4,
                        Modifier = ScoreModifierEnum.InvertGain,
                    };
                    scoreManager.AddModifier(modifier);
                    Action<TrackSO> callback = (track) => ScoreModifiers.enumToModifier[ScoreModifierEnum.InvertGain](modifier, track, scoreManager, 0, ScoreContextEnum.TrackEnd, false);
                    modifier.callback = callback;
                    scoreManager.TrackPlayer.SongEnd += callback;
                },
                timestampActions = new List<TimestampAction>()
            }},
        };
    }
}
