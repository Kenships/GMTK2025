using System;
using DefaultNamespace;
using ImprovedTimers;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace TrackScripts
{
    public class TrackPlayer : MonoBehaviour
    {
        [SerializeField] GameSettingsSO settings;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioSource backgroundSource;
        [SerializeField] AudioClip backgroundClip;

        [SerializeField]
        private PlaylistController discoBall;
        [SerializeField]
        private PlaylistController activePlaylist;
        
        [SerializeField] private ScriptableEventNoParam backupToActiveEvent;
        [SerializeField] private ScriptableEventNoParam activeToDiscoEvent;
        [SerializeField] private ScriptableEventNoParam discoToBackup;

        [SerializeField]
        private FloatVariable progress;
        
        private TrackSO currentTrack;
        
        public UnityAction onSongEnd;

        private void Start()
        {
            backgroundSource.clip = backgroundClip;
            StartSequence();
            Play();
            
            onSongEnd += OnSongEnd;
        }

        private void StartSequence()
        {
            
        }

        private void OnSongEnd()
        {
            backupToActiveEvent?.Raise();
            activeToDiscoEvent?.Raise();
            discoToBackup?.Raise();
            
            Play();
        }

        private void Play()
        {
            audioSource.Stop();
            backgroundSource.Stop();
            
            currentTrack = discoBall.GetNextInQueue() ?? activePlaylist.GetNextInQueue();
            
            audioSource.clip = currentTrack.clip;
            
            audioSource.Play();
            backgroundSource.Play();
        }

        private void Update()
        {
            if (currentTrack != null)
            {
                progress.Value = audioSource.time / (audioSource.clip.length * (currentTrack.bars/4f)) * 100f;
            }
            
            
            Debug.Log(progress);
            if (!audioSource.isPlaying)
            {
                OnSongEnd();
            }
        }
    }
}
