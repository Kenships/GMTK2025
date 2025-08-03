
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIAnimation
{
    public enum LoopMode
    {
        None,
        Repeat,
        Boomerang
    }
    
    public class FrameAnimator : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Image target;
        
        [Header("Frames")]
        [SerializeField] private List<Sprite> frames;

        [Header("Animation Settings")]
        [SerializeField] private int framesPerSecond;
        [SerializeField] private LoopMode loopMode;
        
        private WaitForSeconds _waitObject;
        
        private float FrameDelay => 1f/framesPerSecond;

        private int _currentFrame;

        private int _frameDirection = 1;

        private void Start()
        {
            StartCoroutine(Play());
        }

        IEnumerator Play()
        {
            _waitObject = new WaitForSeconds(FrameDelay);
            
            if (loopMode == LoopMode.None)
            {
                yield return PlayOnce();
                yield break;
            }

            while (true)
            {
                target.sprite = frames[_currentFrame];

                switch (loopMode)
                {
                    case LoopMode.Repeat:
                        _currentFrame = (_currentFrame + 1) % frames.Count;
                        break;
                    case LoopMode.Boomerang:
                        _currentFrame += _frameDirection;
                        _currentFrame = Mathf.Clamp(_currentFrame, 0, frames.Count - 1);
                        _frameDirection = _currentFrame == 0 || _currentFrame == frames.Count-1 
                            ? -_frameDirection : _frameDirection;
                        break;
                }
                
                yield return _waitObject;
            }
        }

        IEnumerator PlayOnce()
        {
            for (int i = 0; i < frames.Count; i++)
            {
                target.sprite = frames[i];
                yield return _waitObject;
            }
        }
    }
}
