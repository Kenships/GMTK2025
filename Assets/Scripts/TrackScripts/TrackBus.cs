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
        InputAction inputAction;

        private TrackSO currentTrack;
        
        private void Start()
        {
            inputAction.Enable();
            inputAction.performed += InputActionOnperformed;
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
                Tween.Position(
                    target: transform,
                    startPos.position,
                    endPos.position,
                    duration: 0.5f,
                    ease: Ease.InOutExpo,
                    cycles: 1
                ).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    playlistControllerEnd.TryEnqueue(currentTrack);
                });
            }
        }
    }
    
    
}
