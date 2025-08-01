using UnityEngine;
using DefaultNamespace;

namespace DefaultNamespace
{
    public enum Tag { }
    [CreateAssetMenu(fileName = "TrackSO", menuName = "Scriptable Objects/TrackSO")]
    public class TrackSO : ScriptableObject
    {
        public AudioClip clip;
        public Sprite albumCover;
        public float volumeOverride;
        public TrackAbility ability;
        public int points;
        public int price;
        public string description;
        public string name;
        public Tag tag;
    }
}
