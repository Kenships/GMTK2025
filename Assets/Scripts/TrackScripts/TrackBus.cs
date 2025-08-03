using System;
using DefaultNamespace;
using Obvious.Soap;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TrackScripts
{
    public class TrackBus : MonoBehaviour
    {
        [SerializeField]
        private ScriptableEventNoParam moveTrack;
        
    
        [SerializeField]
        PlaylistController playlistControllerStart;
    
        [SerializeField]
        PlaylistController playlistControllerEnd;
        

        [SerializeField]
        private float duration = 0.5f;

        private TrackSO currentTrack;
        
        private void Start()
        {
            moveTrack.OnRaised += MoveTrackOnRaised;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            moveTrack.OnRaised -= MoveTrackOnRaised;
        }

        private void InputActionOnperformed(InputAction.CallbackContext obj)
        {
            MoveTrackOnRaised();
        }

        private void MoveTrackOnRaised()
        {
            gameObject.SetActive(true);

            TrackHolder trackHolder = playlistControllerStart.GetNextTrackHolderInQueue();
            
            
            if (!trackHolder)
            {
                return;
            }
            Vector3 startPosition = trackHolder.transform.position;
            transform.position = startPosition;

            if (playlistControllerStart.TryDequeue(out currentTrack))
            {
                Vector3 endPosition = playlistControllerEnd.GetLastChild().transform.position;
                if (duration == 0)
                {
                    playlistControllerEnd.TryEnqueue(currentTrack);
                }
                Tween.Position(
                    target: transform,
                    endPosition,
                    duration: duration == 0 ? 0.5f : duration,
                    ease: Ease.InOutExpo,
                    cycles: 1
                ).OnComplete(() =>
                {
                    if (duration > 0)
                    {
                        playlistControllerEnd.TryEnqueue(currentTrack);
                    }
                    gameObject.SetActive(false);
                });
            }
        }
    }
    
    
}
