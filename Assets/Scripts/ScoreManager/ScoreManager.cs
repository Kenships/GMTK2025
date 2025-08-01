using DefaultNamespace;
using System;
using UnityEngine;
using System.Collections.Generic;
using ImprovedTimers;
using Obvious.Soap;
using TrackScripts;

public struct ModifierInstance 
{
    public int LifeTime;
    public ScoreModifierEnum Modifier;
}

public class ScoreManager : MonoBehaviour
{
    public TrackPlayer TrackPlayer;
    public IntVariable Score;
    public IntVariable PreviousScore;

    public List<ModifierInstance> modifiers;
    private Dictionary<TrackSO, TrackSO> trackTypeToModified = new () { };
    
    private List<CountdownTimer> timedEffects = new List<CountdownTimer>();

    public int ScorePoints(TrackSO track, int points) 
    {
        TrackSO actualTrack = GetUpToDateTrack(track);
        List<ModifierInstance> toRemove = new List<ModifierInstance>();
        foreach (ModifierInstance m in modifiers) 
        {
            points = ScoreModifiers.enumToModifier[m.Modifier](actualTrack, this, points);
            if (m.LifeTime <= 0) toRemove.Add(m);
        }
        foreach (ModifierInstance modifier in toRemove) modifiers.Remove(modifier);

        PreviousScore.Value = Score;
        Score.Value += points;
        return points;
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
            if (m.Modifier.Equals(modifierType)) 
            {
                m.LifeTime = action(this, m.LifeTime);
                if (m.LifeTime <= 0) toRemove.Add(m);
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
        if (!trackTypeToModified[track]) trackTypeToModified[track] = track;
        return trackTypeToModified[track];
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
}
