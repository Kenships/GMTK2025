using System;
using System.Collections.Generic;
using ImprovedTimers;
using Obvious.Soap;
using TrackScripts;
using Unity.Hierarchy;
using UnityEngine;

namespace ScoreManager
{
    public class ModifierInstance : IEquatable<ModifierInstance>
    {
        public int LifeTime;
        public ScoreModifierEnum Modifier;
        public Action<TrackSO> callback;
        public bool Equals(ModifierInstance other)
        {
            return LifeTime == other.LifeTime && Modifier == other.Modifier;
        }

        public override bool Equals(object obj)
        {
            return obj is ModifierInstance other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LifeTime, (int)Modifier);
        }

        public static bool operator ==(ModifierInstance left, ModifierInstance right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModifierInstance left, ModifierInstance right)
        {
            return !left.Equals(right);
        }
    }

    public class ScoreManager : MonoBehaviour
    {
        public TrackPlayer TrackPlayer;
        public IntVariable Score;
        public IntVariable PreviousScore;
        private int cachedScore;

        public List<ModifierInstance> modifiers;
        public List<ModifierInstance> expiredModifiers;
        private Dictionary<TrackSO, TrackSO> trackTypeToModified = new () { };
    
        private List<CountdownTimer> timedEffects = new List<CountdownTimer>();

        private void Awake()
        {
            modifiers = new List<ModifierInstance>();
            expiredModifiers = new List<ModifierInstance>();
        }

        public int ConsolidatePoints(TrackSO track, ScoreContextEnum context, bool useModifier = true) 
        {
            if (cachedScore == 0) return 0;
            if (useModifier)
            {
                TrackSO actualTrack = GetUpToDateTrack(track);
                List<ModifierInstance> toRemove = new List<ModifierInstance>();
                foreach (ModifierInstance m in modifiers) 
                {
                    cachedScore = ScoreModifiers.enumToModifier[m.Modifier](m, actualTrack, this, cachedScore, context);
                    if (m.LifeTime <= 0)
                    {
                        toRemove.Add(m);
                        expiredModifiers.Add(m);
                    }
                }
                foreach (ModifierInstance modifier in toRemove) modifiers.Remove(modifier);
            }

            PreviousScore.Value = Score;
            Score.Value += cachedScore;

            cachedScore = 0;

            return Score.Value;
        }

        public bool AddModifier(ModifierInstance modifier) 
        {
            modifiers.Add(modifier);
            return true;//Incase we want to reject modifiers for some reason (player has 100) and notify some function
        }

        public void AffectModifierLifetime(ScoreModifierEnum modifierType, Func<ScoreManager, int, int> action) 
        {
            List<ModifierInstance> toRemove = new List<ModifierInstance>();
            for (int i = 0; i < modifiers.Count; i++) 
            {
                ModifierInstance m = modifiers[i];
                if (m.Modifier.Equals(modifierType) || m.Modifier.Equals(ScoreModifierEnum.All)) 
                {
                    m.LifeTime = action(this, m.LifeTime);
                    if (m.LifeTime <= 0)
                    {
                        toRemove.Add(m);
                        expiredModifiers.Add(m);
                    }
                }
            }
            foreach (ModifierInstance m in toRemove) 
            {
                modifiers.Remove(m);
            }
        }
        public void AddTrackModifier(TrackSO track, Func<TrackSO, TrackSO> modification) 
        {
            trackTypeToModified[track] = modification(trackTypeToModified[track]);
        }

        public TrackSO GetUpToDateTrack(TrackSO track)
        {
            if (trackTypeToModified.TryGetValue(track, out TrackSO existingTrack))
            {
                return existingTrack;
            }

            TrackSO newTrack = ScriptableObject.CreateInstance<TrackSO>();

            newTrack.clip = track.clip;
            newTrack.albumCover = track.albumCover;
            newTrack.volumeOverride = track.volumeOverride;
            newTrack.ability = track.ability;
            newTrack.defaultPoints = track.defaultPoints;
            newTrack.points = track.points;
            newTrack.price = track.price;
            newTrack.description = track.description;
            newTrack.trackName = track.trackName;
            newTrack.tags = new List<Tag>(track.tags);
            newTrack.bars = track.bars;
            newTrack.repeat = track.repeat;
            trackTypeToModified[track] = newTrack;
            return newTrack;
        }

        public void AddTimedEffect(float duration, Action action)
        {
            CountdownTimer countdownTimer = new CountdownTimer(duration);
            timedEffects.Add(countdownTimer);
            countdownTimer.OnTimerEnd += () =>
            {
                Debug.Log("End");
                timedEffects.Remove(countdownTimer);
                action.Invoke();
            };
            countdownTimer.Start();
            Debug.Log("Start");
        }
        public void addPoints(int points) 
        {
            cachedScore += points;
        }
    }
}
