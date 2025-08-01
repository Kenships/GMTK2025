using System;
using PrimeTween;
using UnityEngine;

namespace TrackScripts
{
    public class SnapToParentObject : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;
        
        private PlaylistController m_PlaylistController;
        private RectTransform m_RectTransform;

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(Canvas canvas, PlaylistController playlistController)
        {
            this.canvas = canvas;
            m_PlaylistController = playlistController;
        }
        
        public void SnapToParent()
        {
            Tween.StopAll(m_RectTransform);
            transform.SetParent(m_PlaylistController.GetNextEmptySlot() ?? canvas.transform);
        
            Tween.LocalPosition(
                target: m_RectTransform,
                startValue: m_RectTransform.anchoredPosition,
                endValue: new Vector2(0, 0),
                duration: 0.5f,
                ease: Ease.InOutExpo,
                cycles:1
            );
        }
    }
}
