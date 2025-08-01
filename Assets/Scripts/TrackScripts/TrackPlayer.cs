using System;
using DefaultNamespace;
using ImprovedTimers;
using UnityEngine;
using UnityEngine.Events;

namespace TrackScripts
{
    public class TrackPlayer : MonoBehaviour
    {
        [SerializeField] GameSettingsSO settings;
        [SerializeField] AudioSource audioSource;

        [SerializeField] private PlaylistController activePlaylist;
        [SerializeField] private PlaylistController backupPlaylist;
        [SerializeField] private ScoreManager scoreManager;

        private TrackSO currentTrack;
        
        public UnityAction onSongEnd;

        private void Start()
        {
            StartSequence();
            Play();
            
            onSongEnd += OnSongEnd;
        }

        private void StartSequence()
        {
            for (int i = 0; i < 2; i++)
            {
                activePlaylist.TryEnqueue(backupPlaylist.GetNextInQueue());
            }
        }

        private void OnSongEnd()
        {
            currentTrack.ability.endAction(scoreManager, currentTrack, backupPlaylist);
            backupPlaylist.TryEnqueue(currentTrack);
            
            Play();
        }

        private void Play()
        {
            if (activePlaylist.TryDequeue(out TrackSO track))
            {
                currentTrack = track;
                
                CountdownTimer timer = new CountdownTimer(track.clip.length);
                timer.OnTimerEnd += () => onSongEnd?.Invoke();
                audioSource.clip = track.clip;
                audioSource.Play();
                timer.Start();
                currentTrack.ability.startAction(scoreManager, currentTrack, backupPlaylist);
                foreach (TimestampAction ta in currentTrack.ability.timestampActions) 
                {
                    CountdownTimer taTimer = new CountdownTimer(ta.audioTime);
                    taTimer.OnTimerEnd += () => { ta.Action(scoreManager, currentTrack, backupPlaylist); };
                }
            }
            
            if (backupPlaylist.TryDequeue(out track))
            {
                activePlaylist.TryEnqueue(track);
            }
        }
    }
}
