using System;
using TMPro;
using TrackScripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Image))]
    public class TrackHolder : MonoBehaviour
    {
        [SerializeField] private Image albumCover;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TrackSO track;
        
        public TrackSO Track {get => track; set => track = value;}

        private void Start()
        {
            albumCover ??= GetComponent<Image>();

            if (Track)
            {
                albumCover.sprite = Track.albumCover;
                if (text)
                {
                    text.text = Track.name;
                }
            }
            GetComponent<Tooltip>().shopTooltip = true;
            GetComponent<Tooltip>().track = track;
        }
    }
}
