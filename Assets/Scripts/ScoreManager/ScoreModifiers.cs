
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
            self.LifeTime--;
            return scoredPoints * 2; 
        }}, //Example modifier
        { ScoreModifierEnum.ElectronicStreak, (self, track, scoreManager, scoredPoints, context) => {
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            if (scoreManager.GetUpToDateTrack(track).tags.Contains(Tag.Electronic) || scoreManager.GetUpToDateTrack(track).tags.Contains(Tag.MusicBox))
            {
                self.LifeTime++;
                return scoredPoints;
            }
            scoredPoints += self.LifeTime;

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

            if( !track.repeat) {
                if (!track.tags.Contains(Tag.Wind) && !track.tags.Contains(Tag.MusicBox))
                {
                    Debug.Log("setting repeat to true");

                    Debug.Log(track.name);
                    Debug.Log(track.tags[0]);

                    Debug.Log(track.tags[1]);
                    track.repeat = true;
                    self.LifeTime -= 1;
                }
            }

            if (self.LifeTime <= 0) scoreManager.TrackPlayer.SongStart -= self.callback;

            return scoredPoints;
        }}
    };
}
