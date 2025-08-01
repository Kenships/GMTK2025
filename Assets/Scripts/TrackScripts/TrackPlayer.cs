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
        [Header("Game Start Trigger")]
        [SerializeField] ScriptableEventNoParam startGame;
        
        [Header("Track References")]
        [SerializeField] private GameSettingsSO settings;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioSource backgroundSource;
        [SerializeField] private AudioClip backgroundClip;
        [SerializeField] private ScoreManager scoreManager;
        

        [SerializeField] private PlaylistController discoBall;
        [SerializeField] private PlaylistController activePlaylist;
        [SerializeField] private PlaylistController backupPlaylist;
        
        [Header("Track Bus Events")]
        
        [SerializeField] private ScriptableEventNoParam backupToActiveEvent;
        [SerializeField] private ScriptableEventNoParam activeToDiscoEvent;
        [SerializeField] private ScriptableEventNoParam discoToBackup;
        

        [SerializeField] private FloatVariable progress;
        
        private TrackSO currentTrack;
        

        private void Start()
        {
            backgroundSource.clip = backgroundClip;
            startGame.OnRaised += Play;
        }
        

        private void OnSongEnd()
        {
            backupToActiveEvent?.Raise();
            activeToDiscoEvent?.Raise();
            discoToBackup?.Raise();
            currentTrack.ability.endAction(scoreManager, currentTrack, backupPlaylist);
            
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

            currentTrack.ability.startAction(scoreManager, currentTrack, backupPlaylist);
                foreach (TimestampAction ta in currentTrack.ability.timestampActions) 
                {
                    CountdownTimer taTimer = new CountdownTimer(ta.audioTime);
                    taTimer.OnTimerEnd += () => { ta.Action(scoreManager, currentTrack, backupPlaylist); };
                }
        }

        private void Update()
        {
            if (currentTrack != null)
            {
                progress.Value = audioSource.time / (audioSource.clip.length * (currentTrack.bars/4f)) * 100f;
            }
            
            if (audioSource.clip && !audioSource.isPlaying)
            {
                OnSongEnd();
            }
        }
    }
}
