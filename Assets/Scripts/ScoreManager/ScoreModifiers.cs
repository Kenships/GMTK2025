using DefaultNamespace;
using System;
using TrackScripts;
using System.Collections.Generic;

public enum ScoreModifierEnum
{
    X2
}
public class ScoreModifiers
{
    public static Dictionary<ScoreModifierEnum, Func<TrackSO, TrackSO, ScoreManager, int, int>> enumToModifier = new()
    {
        { ScoreModifierEnum.X2, (trackType, trackValue, scoreManager, scoredPoints) => { return scoredPoints * 2; } } //Example modifier
    };
}