using System.Collections.Generic;
using System;
using ImprovedTimers;
using TrackScripts;

namespace DefaultNamespace
{
    public enum TrackAbilityEnum
    {
        None,
        AudioSpeedBoost1_2x
    }

    public struct TimestampAction 
    {
        public readonly float audioTime;
        public Action<ScoreManager, TrackSO, PlaylistController> Action;
    }
    public class TrackAbility 
    {
        public Action<ScoreManager, TrackSO, PlaylistController> startAction;
        public Action<ScoreManager, TrackSO, PlaylistController> endAction;
        public List<TimestampAction> timestampActions;
    }

    public class TrackAbilities 
    {
        public static Dictionary<TrackAbilityEnum, TrackAbility> EnumToAbility = new() 
        {
            {TrackAbilityEnum.AudioSpeedBoost1_2x, new TrackAbility()
            {
                startAction = (scoreManager, track, playlist) =>
                {
                    
                }, 
                endAction = (scoreManager, track, playlist) =>
                {
                    scoreManager.TrackPlayer.PlayBackSpeed.Value = 1.2f;

                    scoreManager.AddTimedEffect(10f, () => scoreManager.TrackPlayer.PlayBackSpeed.Value = 1f);
                }, 
                timestampActions = new List<TimestampAction>()
            }}
        };
    }
}
