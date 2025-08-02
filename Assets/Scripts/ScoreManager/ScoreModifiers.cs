
using DefaultNamespace;
using ScoreManager;
using System;
using TrackScripts;
using System.Collections.Generic;


public enum ScoreModifierEnum
{
    All, // For affecting ALL modifiers
    X2, ElectronicStreak, Lose5, RepeatNonWind
}
public enum ScoreContextEnum 
{
    TrackStart, TrackEnd, TimestampAction, BarStart
}
public class ScoreModifiers
{
    public static Dictionary<ScoreModifierEnum, Func<ModifierInstance, TrackSO, ScoreManager.ScoreManager, int, ScoreContextEnum, int>> enumToModifier = new()
    {
        { ScoreModifierEnum.X2, (self, track, scoreManager, scoredPoints, context) => {
            self.LifeTime--;
            return scoredPoints * 2; 
        }}, //Example modifier
        { ScoreModifierEnum.ElectronicStreak, (self, track, scoreManager, scoredPoints, context) => {
            if(!context.Equals(ScoreContextEnum.TrackEnd)) return scoredPoints;
            if (scoreManager.GetUpToDateTrack(track).tags.Contains(Tag.Electronic))
            {
                self.LifeTime++;
                return scoredPoints;
            }
            scoredPoints += self.LifeTime;
            self.LifeTime = 0;
            if (self.LifeTime <= 0) scoreManager.TrackPlayer.SongEnd -= self.callback;
            return scoredPoints;
        }},
        { ScoreModifierEnum.Lose5, (self, track, scoreManager, scoredPoints, context) => {
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            scoredPoints -= 5;

            self.LifeTime--;
            if (self.LifeTime <= 0) scoreManager.TrackPlayer.SongStart -= self.callback;
            return scoredPoints;
        }},
        { ScoreModifierEnum.RepeatNonWind, (self, track, scoreManager, scoredPoints, context) => {
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;

            if(!track.repeat) track.repeat = true;
            self.LifeTime -= 1;

            if (self.LifeTime <= 0) scoreManager.TrackPlayer.SongStart -= self.callback;
            return scoredPoints;
        }}
    };
}
