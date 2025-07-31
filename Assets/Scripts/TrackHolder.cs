using UnityEngine;

namespace DefaultNamespace
{
    public class TrackHolder : MonoBehaviour
    {
        [SerializeField] private TrackSO track;
        
        public TrackSO Track => track;
    }
}
