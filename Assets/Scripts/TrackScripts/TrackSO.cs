using DefaultNamespace;
using UnityEngine;

namespace TrackScripts
{
    public enum Tag { }
    [CreateAssetMenu(fileName = "TrackSO", menuName = "Scriptable Objects/TrackSO")]
    public class TrackSO : ScriptableObject
    {
        public AudioClip clip;
        public Sprite albumCover;
        public float volumeOverride;
        public TrackAbilityEnum ability;
        public int points;
        public string description;
        public Tag tag;
        public int bars;
    }
}
