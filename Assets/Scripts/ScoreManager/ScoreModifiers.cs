
using DefaultNamespace;
using ScoreManager;
using System;
using TrackScripts;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreModifierEnum
{
    All, // For affecting ALL modifiers
    X2, ElectronicStreak, Lose5, RepeatNonWind, LastSongPlayed, SetToThree, IntstrumentType, GainNowLoseIfNoJoy, InvertGain
}
public enum ScoreContextEnum 
{
    TrackStart, TrackEnd, TimestampAction, BarStart
}
public class ScoreModifiers
{
    public static Dictionary<ScoreModifierEnum, Func<ModifierInstance, TrackSO, ScoreManager.ScoreManager, int, ScoreContextEnum, bool, int>> enumToModifier = new()
    {
        { ScoreModifierEnum.X2, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            self.LifeTime--;
            return scoredPoints * 2; 
        }}, //Example modifier
        { ScoreModifierEnum.SetToThree, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            self.LifeTime--;
            return 3;
        }},
        { ScoreModifierEnum.ElectronicStreak, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime <= 0) { scoreManager.TrackPlayer.SongEnd -= self.callback; return 0; }
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            if (track.tags.Contains(Tag.Electronic) || track.tags.Contains(Tag.MusicBox))
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
        { ScoreModifierEnum.Lose5, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime <= 0) { scoreManager.TrackPlayer.SongStart -= self.callback; return 0; }
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            scoredPoints -= 5;

            self.LifeTime--;
            if (self.LifeTime <= 0) scoreManager.TrackPlayer.SongStart -= self.callback;
            scoreManager.addPoints(scoredPoints);
            return scoredPoints;
        }},
        { ScoreModifierEnum.RepeatNonWind, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime <= 0) { scoreManager.TrackPlayer.SongStart -= self.callback; return 0; }
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
        }},
        { ScoreModifierEnum.LastSongPlayed, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime <= 0) { scoreManager.TrackPlayer.SongStart -= self.callback; return 0; }
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;

            self.LifeTime = 0;
            scoreManager.TrackPlayer.SongStart -= self.callback;

            return scoredPoints;
        }},
        { ScoreModifierEnum.IntstrumentType, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            List<Tag> instrumentTags = new List<Tag> { Tag.String, Tag.Wind, Tag.Percussion, Tag.Electronic };
            Tag myInstrumentTag = Tag.Null;
            foreach(Tag t in track.tags)
            {
                if (instrumentTags.Contains(t))
                {
                    myInstrumentTag = t;
                    break;
                }
            }

            List<TrackSO> history = scoreManager.TrackPlayer.trackHistory;
            if(history.Count-3 >= 0)
            {
                if (history[history.Count-3].tags.Contains(myInstrumentTag) || history[history.Count-3].tags.Contains(Tag.MusicBox))
                {
                    if (history[history.Count-2].tags.Contains(myInstrumentTag) || history[history.Count-2].tags.Contains(Tag.MusicBox))
                    {
                        scoredPoints = scoredPoints * 2;
                    }
                }
            }
            return scoredPoints;
        }},
        { ScoreModifierEnum.GainNowLoseIfNoJoy, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime <= 0) 
            {
                scoreManager.TrackPlayer.SongStart -= self.callback;
                if(self.counter < 2)
                {
                    scoreManager.addPoints(-20);
                }
                return 0;
            }
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            if (scoreManager.GetUpToDateTrack(track).tags.Contains(Tag.Joy) || scoreManager.GetUpToDateTrack(track).tags.Contains(Tag.MusicBox))
            {
                self.counter++;
            }
            if(self.counter >= 2) self.LifeTime = 0;
            if (self.LifeTime <= 0)
            {
                scoreManager.TrackPlayer.SongStart -= self.callback;
                if(self.counter < 2)
                {
                    scoreManager.addPoints(-20);
                }
                return 0;
            }
            return scoredPoints;
        }},
        { ScoreModifierEnum.InvertGain, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if(context.Equals(ScoreContextEnum.TrackEnd)) self.LifeTime--;
            if (self.LifeTime <= 0)
            {
                scoreManager.TrackPlayer.SongStart -= self.callback;
                return scoredPoints;
            }
            return scoredPoints * -1;
        }},
    };
}
