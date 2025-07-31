using DefaultNamespace;
using System;
using System.Collections.Generic;

public enum ScoreModifierEnum
{
    X2
}
public class ScoreModifiers
{
    public static Dictionary<ScoreModifierEnum, Func<TrackSO, ScoreManager, int, int>> enumToModifier = new()
    {
        { ScoreModifierEnum.X2, (track, scoreManager, scoredPoints) => { return scoredPoints * 2; } } //Example modifier
    };

    

}