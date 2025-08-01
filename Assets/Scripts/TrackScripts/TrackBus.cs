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
        Transform startPos;
    
        [SerializeField]
        Transform endPos;
    
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
            transform.position = startPos.position;
        }

        private void InputActionOnperformed(InputAction.CallbackContext obj)
        {
            MoveTrackOnRaised();
        }

        private void MoveTrackOnRaised()
        {
            gameObject.SetActive(true);

            

            if (playlistControllerStart.TryDequeue(out currentTrack))
            {
                if (duration == 0)
                {
                    playlistControllerEnd.TryEnqueue(currentTrack);
                }
                Tween.Position(
                    target: transform,
                    startPos.position,
                    endPos.position,
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
