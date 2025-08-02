using DefaultNamespace;
using UnityEngine;

namespace TrackScripts
{
    public enum Tag
    {
        PhaseOne,
        PhaseTwo,
        PhaseThree
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
        public Tag tag;
        public int bars;

        
    }
}
