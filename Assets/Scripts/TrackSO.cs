using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "TrackSO", menuName = "ScriptableObjects/TrackSO")]
    public class TrackSO : ScriptableObject
    {
        public AudioClip clip;
        public float volumeOverride;
    }
}
