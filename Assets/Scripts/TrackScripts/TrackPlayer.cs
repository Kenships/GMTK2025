using System;
using DefaultNamespace;
using ImprovedTimers;
using Obvious.Soap;
using UnityEngine;

namespace TrackScripts
{
    public class TrackPlayer : MonoBehaviour
    {
        [Header("Game Start Trigger")]
        [SerializeField] private ScriptableEventNoParam startGame;

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

        public FloatVariable PlayBackSpeed;

        private TrackSO currentTrack;

        private void Awake()
        {
            PlayBackSpeed.Value = 1;
        }

        private void Start()
        {
            backgroundSource.clip = backgroundClip;
            PlayBackSpeed.OnValueChanged += (value) =>
                                            {
                                                audioSource.pitch = value; 
                                                backgroundSource.pitch = value;
                                            };
            startGame.OnRaised += Play;
        }

        private void OnSongEnd()
        {
            backupToActiveEvent?.Raise();
            activeToDiscoEvent?.Raise();
            discoToBackup?.Raise();

            if (TrackAbilities.EnumToAbility.TryGetValue(currentTrack.ability, out var ability))
            {
                ability.endAction.Invoke(scoreManager, currentTrack, backupPlaylist);
            }
            

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


            if (TrackAbilities.EnumToAbility.TryGetValue(currentTrack.ability, out var ability))
            {
                ability.startAction(scoreManager, currentTrack, backupPlaylist);
                foreach (TimestampAction ta in ability.timestampActions)
                {
                    var taTimer = new CountdownTimer(ta.audioTime);
                    taTimer.Start();
                    taTimer.OnTimerEnd += () => { ta.Action(scoreManager, currentTrack, backupPlaylist); };
                }   
            }
        }

        private void Update()
        {
            if (currentTrack != null)
            {
                progress.Value = audioSource.time / (audioSource.clip.length * (currentTrack.bars / 4f)) * 100f;
            }

            if (audioSource.clip && !audioSource.isPlaying)
            {
                OnSongEnd();
            }
        }
    }
}
