
using DefaultNamespace;
using ImprovedTimers;
using Obvious.Soap;
using ScoreManager;
using System;
using System.Collections.Generic;
using TrackScripts;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public enum ScoreModifierEnum
{
    Null,//Error value
    All, // For affecting ALL modifiers
    X2, ElectronicStreak, Lose5, RepeatNonWind, LastSongPlayed, InstrumentType, GainNowLoseIfNoJoy, InvertGain, AngelicTouch, EchoOfDesperation, AddThree,
    MoodTuner, AmpStack, RubiksCube, BandTogether, Relief, AngelicLuck,
    ShuffleIn, HardCut, NegativeBeat
}
public enum ScoreContextEnum 
{
    TrackStart, TrackEnd, TimestampAction, BarStart, AbilityActivated
}
public class ScoreModifiers
{
    public static Dictionary<ScoreModifierEnum, Func<ModifierInstance, TrackSO, ScoreManager.ScoreManager, int, ScoreContextEnum, bool, int>> enumToModifier = new()
    {
        { ScoreModifierEnum.X2, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) return scoredPoints;
            if(self.counter == 0)
            {
                if(context.Equals(ScoreContextEnum.TrackEnd)) self.counter++;
                return scoredPoints;
            }
            if(context.Equals(ScoreContextEnum.TrackEnd))
            {
                self.LifeTime.Value--;
            }
            if (self.LifeTime.Value <= 0)
            {
                return scoredPoints;
            }
            return scoredPoints * 2;
        }},
        { ScoreModifierEnum.AddThree, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) return scoredPoints;
            if(self.counter == 0)
            {
                if(context.Equals(ScoreContextEnum.TrackEnd)) self.counter++;
                return scoredPoints;
            }
            if(context.Equals(ScoreContextEnum.TrackEnd))
            {
                self.LifeTime.Value--;
            }
            if (self.LifeTime.Value <= 0)
            {
                return scoredPoints;
            }
            return scoredPoints + 3;
        }},
        { ScoreModifierEnum.ElectronicStreak, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) { scoreManager.TrackPlayer.SongEnd -= self.callback; return 0; }
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            if (track.tags.Contains(Tag.Electronic) || track.tags.Contains(Tag.MusicBox))
            {
                self.LifeTime.Value++;
                return scoredPoints;
            }
            scoredPoints += self.LifeTime.Value;

            scoreManager.addPoints(scoredPoints);
            self.LifeTime.Value = 0;
            if (self.LifeTime.Value <= 0) scoreManager.TrackPlayer.SongEnd -= self.callback;
            return scoredPoints;
        }},
        { ScoreModifierEnum.Lose5, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) { scoreManager.TrackPlayer.SongStart -= self.callback; return 0; }
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            scoredPoints -= 5;

            self.LifeTime.Value--;
            if (self.LifeTime.Value <= 0) scoreManager.TrackPlayer.SongStart -= self.callback;
            scoreManager.addPoints(scoredPoints);
            return scoredPoints;
        }},
        { ScoreModifierEnum.RepeatNonWind, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) { scoreManager.TrackPlayer.SongStart -= self.callback; return 0; }
            if(self.counter == 0)
            {
                if(context.Equals(ScoreContextEnum.TrackEnd)) self.counter++;
                return scoredPoints;
            }
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            if( !track.repeat) {
                if (!track.tags.Contains(Tag.Wind) && !track.tags.Contains(Tag.MusicBox))
                {
                    track.repeat = true;
                    self.LifeTime.Value -= 1;
                }
            }

            if (self.LifeTime.Value <= 0) scoreManager.TrackPlayer.SongStart -= self.callback;

            return scoredPoints;
        }},
        { ScoreModifierEnum.LastSongPlayed, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) { scoreManager.TrackPlayer.SongStart -= self.callback; return 0; }
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;

            self.LifeTime.Value = 0;
            scoreManager.TrackPlayer.SongStart -= self.callback;

            return scoredPoints;
        }},
        { ScoreModifierEnum.InstrumentType, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) return scoredPoints;
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
            if (lifetimeNotification && self.LifeTime.Value <= 0)
            {
                scoreManager.TrackPlayer.SongStart -= self.callback;
            }
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints;
            self.LifeTime.Value--;
            if (track.tags.Contains(Tag.Joy))
            {
                self.counter++;
            }
            if(self.counter >= 2) self.LifeTime.Value = 0;
            if (self.LifeTime.Value <= 0)
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
            if (lifetimeNotification && self.LifeTime.Value <= 0) return scoredPoints;
            if(self.counter == 0)
            {
                if(context.Equals(ScoreContextEnum.TrackEnd)) self.counter++;
                return scoredPoints;
            }
            if(context.Equals(ScoreContextEnum.TrackEnd))
            {
                self.LifeTime.Value--;
            }
            if (self.LifeTime.Value <= 0)
            {
                return scoredPoints;
            }
            return scoredPoints * -1;
        }},
        { ScoreModifierEnum.AngelicTouch, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) { scoreManager.TrackPlayer.SongStart -= self.callback; return scoredPoints; }
            if(!context.Equals(ScoreContextEnum.TrackEnd)) return scoredPoints;
            if (self.counter == 0) { self.counter++; return scoredPoints; }
            self.LifeTime.Value--;
            if (track.tags.Contains(Tag.Joy))
            {
                int count = 0;
                foreach (ModifierInstance modifier in scoreManager.modifiers)
                {
                    if (modifier.Modifier.Equals(ScoreModifierEnum.AngelicLuck) && modifier.LifeTime.Value > 0)
                    {
                        count++;
                    }
                }
                float chance = 0.05f + count * 0.01f;
                if(UnityEngine.Random.value < chance)
                {
                    scoreManager.addPoints(9001 * scoreManager.GetUpToDateTrack(track).points);
                }
                return scoredPoints;
            }
            if (self.LifeTime.Value <= 0)
            {
                scoreManager.TrackPlayer.SongStart -= self.callback;
                return 0;
            }
            return scoredPoints;
        }},
        { ScoreModifierEnum.EchoOfDesperation, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) { scoreManager.TrackPlayer.SongStart -= self.callback; return scoredPoints; }
            if(self.counter == 0 && context.Equals(ScoreContextEnum.TrackStart))
            {
                self.counter++;
                return scoredPoints;
            }
            else if(self.counter == 0) return scoredPoints;
            if(!context.Equals(ScoreContextEnum.TrackStart)) return scoredPoints/2;
            self.LifeTime.Value--;
            if (self.LifeTime.Value <= 0)
            {
                scoreManager.TrackPlayer.SongStart -= self.callback;
                return scoredPoints;
            }
            return scoredPoints;
        }},
        { ScoreModifierEnum.MoodTuner, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            self.LifeTime.Value = 9999;
            if(context.Equals(ScoreContextEnum.TrackEnd))
            {
                List<Tag> emotionTags = new List<Tag> { Tag.Joy, Tag.Sadness, Tag.Anger, Tag.Fear, Tag.Envy };
                Tag myEmotionTag = Tag.Null;
                foreach(Tag t in track.tags)
                {
                    if (emotionTags.Contains(t))
                    {
                        myEmotionTag = t;
                        break;
                    }
                }

                List<TrackSO> history = scoreManager.TrackPlayer.trackHistory;
                if(history.Count-2 >= 0)
                {
                    if (history[history.Count-2].tags.Contains(myEmotionTag))
                    {
                        scoreManager.addPoints(2);
                    }
                }
            }
            return scoredPoints;
        }},
        { ScoreModifierEnum.AmpStack, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            self.LifeTime.Value = 9999;
            if(context.Equals(ScoreContextEnum.TrackEnd))
            {
                List<TrackSO> history = scoreManager.TrackPlayer.trackHistory;
                if(history.Count-2 >= 0)
                {
                    if (history[history.Count-2].tags.Contains(Tag.Percussion))
                    {
                        scoreManager.addPoints(1);
                    }
                }
            }
            return scoredPoints;
        }},
        { ScoreModifierEnum.RubiksCube, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            self.LifeTime.Value = 9999;
            if (context.Equals(ScoreContextEnum.TrackEnd))
            {
                List<Tag> emotionTags = new List<Tag> { Tag.Joy, Tag.Sadness, Tag.Anger, Tag.Fear, Tag.Envy };
                HashSet<Tag> foundEmotions = new HashSet<Tag>();

                List<TrackSO> history = scoreManager.TrackPlayer.trackHistory;

                foreach (TrackSO pastTrack in history)
                {
                    foreach (Tag tag in pastTrack.tags)
                    {
                        if (emotionTags.Contains(tag))
                        {
                            foundEmotions.Add(tag);
                        }
                    }
                }

                if (foundEmotions.Count == emotionTags.Count)
                {
                    scoreManager.addPoints(10);
                }
            }
            return scoredPoints;
        }},
        { ScoreModifierEnum.BandTogether, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if(self.LifeTime.Value < 2000) self.LifeTime.Value = 2000;
            if(!context.Equals(ScoreContextEnum.TrackEnd)) return scoredPoints;
            List<Tag> instrumentTags = new List<Tag> { Tag.String, Tag.Wind, Tag.Percussion, Tag.Electronic };
            Tag myInstrumentTag = Tag.Null;

            // Identify which instrument tag applies to the current track
            foreach (Tag t in track.tags)
            {
                if (instrumentTags.Contains(t))
                {
                    myInstrumentTag = t;
                    break;
                }
            }

            List<TrackSO> history = scoreManager.TrackPlayer.trackHistory;

            if (history.Count >= 3 && self.LifeTime.Value <= 2999)
            {
                bool match = true;
                for (int i = 1; i <= 3; i++)
                {
                    var tags = history[history.Count - i].tags;
                    if (!(tags.Contains(myInstrumentTag) || tags.Contains(Tag.MusicBox)))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    self.LifeTime.Value = 1000000;
                }
            }

            return scoredPoints;
        }},
        { ScoreModifierEnum.Relief, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if(self.LifeTime.Value < 2000) self.LifeTime.Value = 2000;
            if (!context.Equals(ScoreContextEnum.AbilityActivated)) return scoredPoints;
            if (self.LifeTime.Value <= 2000) 
            {
                self.LifeTime.Value = 2000;
                return scoredPoints;
            }
            foreach(ModifierInstance m in scoreManager.modifiers)
            {
                if (m.LifeTime.Value < 999 && m.LifeTime.Value > 0)
                {
                    m.LifeTime.Value = -1;
                    ScoreModifiers.enumToModifier[m.Modifier](m, null, scoreManager, 0, ScoreContextEnum.TimestampAction, true);
                    break;
                }
            }
            self.LifeTime.Value = 2000;
            CountdownTimer countdownTimer = new CountdownTimer(3f);
            countdownTimer.OnTimerEnd += () => self.LifeTime.Value = self.LifeTime.Value <= 0 ? self.LifeTime.Value : 100000;
            return scoredPoints;
        }},
        { ScoreModifierEnum.AngelicLuck, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if(self.LifeTime.Value < 2000) self.LifeTime.Value = 2000;
            return scoredPoints;
        }},
        { ScoreModifierEnum.ShuffleIn, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) return scoredPoints;
            if(!context.Equals(ScoreContextEnum.TrackEnd)) return scoredPoints;
            self.LifeTime.Value--;
            if (self.LifeTime.Value <= 0)
            {
                scoreManager.TrackPlayer.Shuffle();
            }
            return scoredPoints;
        }},
        { ScoreModifierEnum.HardCut, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) return scoredPoints;
            if(!context.Equals(ScoreContextEnum.TrackEnd)) return scoredPoints;
            self.LifeTime.Value--;
            scoreManager.TrackPlayer.abilityBlocked = true;
            return scoredPoints;
        }},
        { ScoreModifierEnum.NegativeBeat, (self, track, scoreManager, scoredPoints, context, lifetimeNotification) => {
            if (lifetimeNotification && self.LifeTime.Value <= 0) return scoredPoints;
            if(!context.Equals(ScoreContextEnum.TrackEnd)) return scoredPoints;
            self.LifeTime.Value--;
            if (self.LifeTime.Value <= 0)
            {
                ModifierInstance modifier = new ModifierInstance()
                    {
                        LifeTime = ScriptableObject.CreateInstance<IntVariable>(),
                        Modifier = ScoreModifierEnum.InvertGain,
                    };
                modifier.LifeTime.Value = 1;
                scoreManager.AddModifier(modifier, track.albumCover);
            }
            return scoredPoints;
        }}
    };
    public static Dictionary<ScoreModifierEnum, string> enumToDescription = new Dictionary<ScoreModifierEnum, string>() 
    {
        { ScoreModifierEnum.X2, "Doubled points!" },
        { ScoreModifierEnum.ElectronicStreak, "Break the streak to Cash in!" },
        { ScoreModifierEnum.Lose5, "You're about to lose 5 points" },
        { ScoreModifierEnum.RepeatNonWind, "Repeat track if not a Wind track" },
        { ScoreModifierEnum.LastSongPlayed, "Multiply final score by 3!" },
        { ScoreModifierEnum.InstrumentType, "Chain same instrument families\n to double points for the third \n and onwards" },
        { ScoreModifierEnum.GainNowLoseIfNoJoy, "Play 2 Joys to avoid losing 20 points" },
        { ScoreModifierEnum.InvertGain, "Multiply points by -1!" },
        { ScoreModifierEnum.AngelicTouch, "99% of gamblers quit before \n they make it BIG" },
        { ScoreModifierEnum.EchoOfDesperation, "Points are halved" },
        { ScoreModifierEnum.AddThree, "Gain 3 extra points on scoring!" },
        { ScoreModifierEnum.MoodTuner, "+2 points when repeating an emotion" },
        { ScoreModifierEnum.AmpStack, "+1 points on song end after Percussion" },
        { ScoreModifierEnum.RubiksCube, "+10 points after 5 emotions in a row" },
        { ScoreModifierEnum.BandTogether, "1.2× final score for \n4 same-instrument tracks in a row" },
        { ScoreModifierEnum.Relief, "Removes newest modifier" },
        { ScoreModifierEnum.AngelicLuck, "Angelic touch chance increases by 1%" },
        { ScoreModifierEnum.ShuffleIn, "About to shuffle the deck" },
        { ScoreModifierEnum.HardCut, "Abilities have been deactivated" },
        { ScoreModifierEnum.NegativeBeat, "Multiply scores by -1" }
    };
}
