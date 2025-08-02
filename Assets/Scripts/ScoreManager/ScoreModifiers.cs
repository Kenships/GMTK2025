
using DefaultNamespace;
using ScoreManager;
using System;
using TrackScripts;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log("Modifier loses life" + self.LifeTime);
            self.LifeTime--;
            return scoredPoints * 2; 
        }}, //Example modifier
        { ScoreModifierEnum.ElectronicStreak, (self, track, scoreManager, scoredPoints, context) => {
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            Debug.Log("Electronic Streak");
            if (scoreManager.GetUpToDateTrack(track).tags.Contains(Tag.Electronic) || scoreManager.GetUpToDateTrack(track).tags.Contains(Tag.MusicBox))
            {
                self.LifeTime++;
                Debug.Log("Electronic Streak yay");
                return scoredPoints;
            }
            scoredPoints += self.LifeTime;

            Debug.Log("Electronic Streak nay" + scoredPoints);
            scoreManager.addPoints(scoredPoints);
            self.LifeTime = 0;
            if (self.LifeTime <= 0) scoreManager.TrackPlayer.SongEnd -= self.callback;
            return scoredPoints;
        }},
        { ScoreModifierEnum.Lose5, (self, track, scoreManager, scoredPoints, context) => {
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            scoredPoints -= 5;

            self.LifeTime--;
            if (self.LifeTime <= 0) scoreManager.TrackPlayer.SongStart -= self.callback;
            scoreManager.addPoints(scoredPoints);
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
