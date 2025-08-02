using DefaultNamespace;
using UnityEngine;
using System.Collections.Generic;

namespace TrackScripts
{
    public enum Tag
    {
        PhaseOne,
        PhaseTwo,
        PhaseThree,
        Electronic,
        Wind,
        String,
        Percussion
    }
    [CreateAssetMenu(fileName = "TrackSO", menuName = "Scriptable Objects/TrackSO")]
    public class TrackSO : ScriptableObject
    {
        public AudioClip clip;
        public Sprite albumCover;
        public float volumeOverride;
        public TrackAbilityEnum ability;
        public int defaultPoints;
        public int points;
        public int price;
        public string description;
        public string trackName;
        public List<Tag> tags;
        public int bars;
        public bool repeat;
        public string trackName;
    }
}
