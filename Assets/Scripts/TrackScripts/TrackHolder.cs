using System;
using TrackScripts;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class TrackHolder : MonoBehaviour
    {
        [SerializeField] private TrackSO track;
        
        public TrackSO Track {get => track; set => track = value;}

        private void Start()
        {
            Image image = GetComponent<Image>();
            image.sprite = Track.albumCover;
        }
    }
}
