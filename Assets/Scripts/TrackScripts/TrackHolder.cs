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
            GetComponent<Tooltip>().Message = "Points: " + track.points + "\n" +
                                              "Bar Count: " + track.bars + "\n" + 
                                              "Price: " + track.price + "\n" +
                                              "Resell Price: " + track.price/2 + "\n" +
                                              "Tags: " + track.tags[0] + ", " + track.tags[1] + "\n" + "\n" + 
                                              
                                              track.description;

        }
    }
}
