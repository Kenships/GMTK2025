using System;
using System.Collections.Generic;
using ImprovedTimers;
using Obvious.Soap;
using PrimeTween;
using TMPro;
using TrackScripts;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreManager
{
    public class ModifierInstance : IEquatable<ModifierInstance>
    {
        public IntVariable LifeTime;
        public ScoreModifierEnum Modifier;
        public Action<TrackSO> callback;
        public int counter;//If modifier needs to count something
        public bool Equals(ModifierInstance other)
        {
            return LifeTime.Value == other.LifeTime.Value && Modifier == other.Modifier;
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
        [SerializeField] private GameObject modifierGrid;
        [SerializeField] private GameObject modIconPrefab;
        [SerializeField] private GameObject itemModIconPrefab;
        [SerializeField] private Sprite defaultIconSprite;
        [SerializeField] List<Sprite> modIconSprites;
        public Dictionary<ModifierInstance, GameObject> modToIcon = new Dictionary<ModifierInstance, GameObject>() {};
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
            Debug.Log("Score Context:" + context);
            Debug.Log("Score Points:" + cachedScore);
            if (useModifier)
            {
                foreach (ModifierInstance m in modifiers) 
                {
                    if (m.LifeTime.Value > 0) //Incase the lifetime was modified outside of here
                    {
                        cachedScore = ScoreModifiers.enumToModifier[m.Modifier](m, track, this, cachedScore, context, false);
                    }
                }
            }

            PreviousScore.Value = Score;
            Score.Value += cachedScore;

            Debug.Log("Scored Points:" + cachedScore);
            cachedScore = 0;

            return Score.Value;
        }
        public void NotifyTrackEnd(TrackSO track) 
        {
            foreach (ModifierInstance m in modifiers)
            {
                if (m.LifeTime.Value > 0) //Incase the lifetime was modified outside of here
                {
                    ScoreModifiers.enumToModifier[m.Modifier](m, track, this, cachedScore, ScoreContextEnum.TrackEnd, false);
                }
            }
        }

        public bool AddModifier(ModifierInstance modifier, Sprite display)
        {
            GameObject prefab = modifier.LifeTime >= 999 ? itemModIconPrefab : modIconPrefab;
            modifiers.Add(modifier);
            if(modifier.Modifier.Equals(ScoreModifierEnum.Relief)) return true;
            GameObject modIcon = Instantiate(prefab, modifierGrid.transform, false);

            modIcon.GetComponent<Image>().sprite = display;
            modIcon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + modifier.LifeTime.Value;
            if (modifier.LifeTime >= 999) 
            {
                int count = 0;
                foreach (ModifierInstance m in modifiers)
                {
                    if(m.Modifier.Equals(modifier.Modifier)) count++;
                }
                modIcon.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x" + count;
            }
            Debug.Log("Displaying modifier" + modifier);
            modIcon.GetComponent<Tooltip>().Message = ScoreModifiers.enumToDescription[modifier.Modifier];
            modToIcon.Add(modifier, modIcon);
            modifier.LifeTime.OnValueChanged += (v) => 
            {
                if (v <= 0 && modIcon != null)
                {
                    modIcon.transform.GetChild(0).gameObject.SetActive(false);
                    //Nothing matters except true since we jsut want to tell it its lifetime was updated, and to cancel callbacks if necessary
                    ScoreModifiers.enumToModifier[modifier.Modifier](modifier, null, this, cachedScore, ScoreContextEnum.TimestampAction, true);

                    LoseModifierAnim(modifier, () => RemoveModifier(modifier));
                }
                else if(modIcon != null)
                {
                    modIcon.GetComponentInChildren<TextMeshProUGUI>().text = "" + v;
                }
            };
            return true;//Incase we want to reject modifiers for some reason (player has 100) and notify some function
        }
        public bool RemoveModifier(ModifierInstance modifier) 
        {
            expiredModifiers.Add(modifier);
            modifiers.Remove(modifier);
            modToIcon.Remove(modifier);
            return true;
        }
        public void AffectModifierLifetime(ScoreModifierEnum modifierType, Func<ScoreManager, int, int> action) 
        {
            List<ModifierInstance> toRemove = new List<ModifierInstance>();
            for (int i = 0; i < modifiers.Count; i++) 
            {
                ModifierInstance m = modifiers[i];
                if (m.Modifier.Equals(modifierType) || m.Modifier.Equals(ScoreModifierEnum.All)) 
                {
                    m.LifeTime.Value = action(this, m.LifeTime.Value);
                }
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
        public void LoseModifierAnim(ModifierInstance m, Action callback) 
        {
            GameObject g = modToIcon[m];
            Image image = g.GetComponent<Image>();
            RectTransform rectTransform = g.GetComponent<RectTransform>();
            Sequence.Create().Group(Tween.UIAnchoredPosition(rectTransform, rectTransform.anchoredPosition + Vector2.down * 10, 0.5f)
                .Group(Tween.Alpha(image, image.color.a, 0f, 0.5f))).OnComplete(() => { Destroy(g); callback(); });
        }
    }
}
