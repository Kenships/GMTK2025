using System;
using System.Collections.Generic;
using DefaultNamespace;
using ImprovedTimers;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Events;

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

        [SerializeField] private ScoreManager.ScoreManager scoreManager;

        [SerializeField] private PlaylistController discoBall;

        [SerializeField] private PlaylistController activePlaylist;

        [SerializeField] private PlaylistController backupPlaylist;

        [Header("Track Bus Events")]
        [SerializeField] private ScriptableEventNoParam backupToActiveEvent;

        [SerializeField] private ScriptableEventNoParam activeToDiscoEvent;

        [SerializeField] private ScriptableEventNoParam discoToBackup;
        
        [SerializeField] private ScriptableEventNoParam discoToActive;
        

        [SerializeField] private FloatVariable progress;

        public UnityAction OnDownBeat;

        public FloatVariable PlayBackSpeed;

        private TrackSO currentTrack;
        
        private bool firstRun = true;

        public List<TrackSO> trackHistory = new();
        public Action<TrackSO> SongEnd;
        public Action<TrackSO> SongStart;

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
            startGame.OnRaised += () =>
            {
                firstRun = true;
                backgroundSource.Play();
                Play();
            };
            
            Debug.Log(audioSource.clip);
        }

        private void OnSongEnd()
        {
            if (TrackAbilities.EnumToAbility.TryGetValue(currentTrack.ability, out var ability))
            {
                List<TrackSO> allTracks = new List<TrackSO>();
                allTracks.AddRange(discoBall.GetAllTracks());
                allTracks.AddRange(activePlaylist.GetAllTracks());
                allTracks.AddRange(backupPlaylist.GetAllTracks());
                
                ability.endAction.Invoke(scoreManager, currentTrack, allTracks);
            }
            SongEnd?.Invoke(currentTrack);
            Play();
        }

        private void Play()
        {
            
            
            audioSource.Stop();
        
            currentTrack = activePlaylist.GetNextInQueue();
            
            if (!firstRun)
            {
                if (backupPlaylist.TrackCount > 0)
                {
                    activeToDiscoEvent?.Raise();
                    backupToActiveEvent?.Raise();
                    discoToBackup?.Raise();
                }
                else if (activePlaylist.TrackCount > 0)
                {
                    activeToDiscoEvent?.Raise();
                    discoToActive?.Raise();
                }
                else
                {
                    currentTrack = discoBall.GetNextInQueue();
                }
            }
            else
            {
                currentTrack = discoBall.GetNextInQueue();
                firstRun = false;
            }
            Debug.Log(currentTrack.name + " has been played");
            trackHistory.Add(currentTrack);
            audioSource.clip = currentTrack.clip;

            audioSource.Play();
            SongStart?.Invoke(currentTrack);

            float timeForOneBar = currentTrack.clip.length / 4f / PlayBackSpeed;
            
            
            CountdownTimerRepeat scoreTimer = new CountdownTimerRepeat(timeForOneBar, currentTrack.bars);
            scoreTimer.OnTimerRaised += () =>
            {
                OnDownBeat?.Invoke();
                scoreManager.ScorePoints(currentTrack, scoreManager.GetUpToDateTrack(currentTrack).points / scoreManager.GetUpToDateTrack(currentTrack).bars, ScoreContextEnum.BarStart);
            };

            scoreTimer.OnTimerEnd += OnSongEnd;
            
            scoreTimer.Start();

            if (TrackAbilities.EnumToAbility.TryGetValue(currentTrack.ability, out var ability))
            {
                List<TrackSO> allTracks = new List<TrackSO>();
                allTracks.AddRange(discoBall.GetAllTracks());
                allTracks.AddRange(activePlaylist.GetAllTracks());
                allTracks.AddRange(backupPlaylist.GetAllTracks());
                
                ability.startAction(scoreManager, currentTrack, allTracks);
                foreach (TimestampAction ta in ability.timestampActions)
                {
                    float adjustedTime = ta.audioTime / PlayBackSpeed.Value;
                    
                    var taTimer = new CountdownTimer(adjustedTime);
                    taTimer.Start();
                    taTimer.OnTimerEnd += () => { ta.Action(scoreManager, currentTrack, allTracks); };
                }   
            }
            
        }

        private void Update()
        {
            if (currentTrack != null)
            {
                progress.Value = audioSource.time / (audioSource.clip.length * (currentTrack.bars / 4f)) * 100f;
            }

            // if (audioSource.clip && !audioSource.isPlaying)
            // {
            //     OnSongEnd();
            // }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            backgroundSource.Pause();
            audioSource.Pause();
            
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            backgroundSource.UnPause();
            audioSource.UnPause();
        }
    }
}
