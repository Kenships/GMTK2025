using DefaultNamespace;
using System;
using UnityEngine;
using System.Collections.Generic;

public struct ModifierInstance 
{
    public int LifeTime;
    public ScoreModifierEnum Modifier;
}
public class ScoreManager : ScriptableObject
{
    public int Score { get; set; }
    public int PreviousScore { get; set; }

    public List<ModifierInstance> modifiers;

    public int ScorePoints(TrackSO track,int points) 
    {
        List<ModifierInstance> toRemove = new List<ModifierInstance>();
        foreach (ModifierInstance m in modifiers) 
        {
            points = ScoreModifiers.enumToModifier[m.Modifier](track, this, points);
            if (m.LifeTime <= 0) toRemove.Add(m);
        }
        foreach (ModifierInstance modifier in toRemove) modifiers.Remove(modifier);

        PreviousScore = Score;
        Score += points;
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
}
