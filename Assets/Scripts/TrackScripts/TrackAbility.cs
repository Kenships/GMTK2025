using System.Collections.Generic;
using System;
namespace DefaultNamespace
{
    public enum TrackAbilityEnum
    {
        None
    }

    public struct TimestampAction 
    {
        public readonly float audioTime;
        public Action<ScoreManager, TrackSO, PlaylistController> Action;
    }
    public struct TrackAbility 
    {
        public Action<ScoreManager, TrackSO, PlaylistController> startAction;
        public Action<ScoreManager, TrackSO, PlaylistController> endAction;
        public List<TimestampAction> timestampActions;
    }

    public class TrackAbilities 
    {
        public static Dictionary<TrackAbilityEnum, TrackAbility> enumToAbility = new() 
        {

        };
    }
}
