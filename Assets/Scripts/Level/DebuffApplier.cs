using JetBrains.Annotations;
using Level;
using NUnit.Framework;
using Obvious.Soap;
using ScoreManager;
using System.Collections.Generic;
using UnityEditor.Rendering.Analytics;
using UnityEngine;

public class DebuffApplier : MonoBehaviour
{
    private List<ScoreModifierEnum> debuffs;
    private ScoreManager.ScoreManager scoreManager;
    private Dictionary<ScoreModifierEnum, Sprite> debuffToSprite = new Dictionary<ScoreModifierEnum, Sprite>();
    private int pointer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void init(LevelDataSO lvlData, ScoreManager.ScoreManager sm) 
    {
        scoreManager = sm;
        debuffs = lvlData.debuffs;
        if (debuffs.Count == 1)
        {
            ModifierInstance modifier = new ModifierInstance()
            {
                LifeTime = ScriptableObject.CreateInstance<IntVariable>(),
                Modifier = debuffs[0],
            };
            modifier.LifeTime.Value = 999;
            sm.AddDebuffModifier(modifier, debuffToSprite[modifier.Modifier], this);
        }
        else 
        {
            System.Random random = new System.Random();
            int n = debuffs.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                ScoreModifierEnum value = debuffs[k];
                debuffs[k] = debuffs[n];
                debuffs[n] = value;
            }
            ModifierInstance m1 = new ModifierInstance()
            {
                LifeTime = ScriptableObject.CreateInstance<IntVariable>(),
                Modifier = debuffs[0],
            };
            m1.LifeTime.Value = 3;
            sm.AddDebuffModifier(m1, debuffToSprite[m1.Modifier], this);
            ModifierInstance m2 = new ModifierInstance()
            {
                LifeTime = ScriptableObject.CreateInstance<IntVariable>(),
                Modifier = debuffs[1],
            };
            m2.LifeTime.Value = 3;
            sm.AddDebuffModifier(m2, debuffToSprite[m2.Modifier], this);
            pointer = 2 >= debuffs.Count ? 0 : 2;
        }
    }
    public void reqDebuff() 
    {
        ModifierInstance m1 = new ModifierInstance()
        {
            LifeTime = ScriptableObject.CreateInstance<IntVariable>(),
            Modifier = debuffs[pointer],
        };
        m1.LifeTime.Value = 3;
        scoreManager.AddDebuffModifier(m1, debuffToSprite[m1.Modifier], this);
        pointer++;
        pointer = pointer >= debuffs.Count ? 0 : pointer;
    }
}
