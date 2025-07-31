using System;
using DefaultNamespace;
using ImprovedTimers;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Events;

namespace TrackScripts
{
    public class TrackPlayer : MonoBehaviour
    {
        [SerializeField] GameSettingsSO settings;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioSource backgroundSource;
        [SerializeField] AudioClip backgroundClip;

        [SerializeField] private PlaylistController activePlaylist;
        [SerializeField] private PlaylistController backupPlaylist;
        
        [SerializeField] private ScriptableEventNoParam enqueueEvent;
        [SerializeField] private ScriptableEventNoParam dequeueEvent;
        
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
            dequeueEvent?.Raise();
            enqueueEvent?.Raise();
            
            Play();
        }

        private void Play()
        {
            audioSource.Stop();
            backgroundSource.Stop();
            
            TrackSO track = activePlaylist.GetNextInQueue();

            audioSource.clip = track.clip;
            
            audioSource.Play();
            backgroundSource.Play();
        }

        private void Update()
        {
            if (!audioSource.isPlaying)
            {
                OnSongEnd();
            }
        }
    }
}
