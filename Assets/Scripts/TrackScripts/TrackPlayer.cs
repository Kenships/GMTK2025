using System;
using System.Collections.Generic;
using DefaultNamespace;
using ImprovedTimers;
using Level;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Events;

namespace TrackScripts
{
    public class TrackPlayer : MonoBehaviour
    {
        [Header("Game Start/End Trigger")]
        [SerializeField] private ScriptableEventLevelDataSO startGame;
        [SerializeField] private ScriptableEventLevelDataSO endGame;

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

        public int currentBarNumber;
        
        public UnityAction OnDownBeat;

        private TrackSO currentTrack;
        
        private bool firstRun = true;

        public List<TrackSO> trackHistory = new();
        public Action<TrackSO> SongEnd;
        public Action<TrackSO> SongStart;
        
        private LevelDataSO levelData;


        private void Start()
        {
            backgroundSource.clip = backgroundClip;
            
            startGame.OnRaised += StartGame;
            
            Debug.Log(audioSource.clip);
        }

        private void OnDestroy()
        {
            startGame.OnRaised -= StartGame;
        }

        private void StartGame(LevelDataSO levelData)
        {
            this.levelData = levelData;
            firstRun = true;
            backgroundSource.Play();
            Play();
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
            scoreManager.NotifyTrackEnd(currentTrack);

            if (currentBarNumber >= levelData.numberOfBars)
            {
                endGame.Raise(levelData);
            }
            else
            {
                Play();
            }
        }

        private void Play()
        {
            
            audioSource.Stop();
            bool repeated = false;
            if (currentTrack != null && currentTrack.repeat)
            {
                currentTrack.repeat = false;
                repeated = true;
            }
            else
            {
                currentTrack = activePlaylist.GetNextInQueue();
            }

            if (!firstRun && !repeated)
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
            SongStart?.Invoke(currentTrack);
            Debug.Log(currentTrack.name + " has been played");
            trackHistory.Add(currentTrack);
            audioSource.clip = currentTrack.clip;

            audioSource.Play();

            float timeForOneBar = currentTrack.clip.length / 4f;
            
            Debug.Log(timeForOneBar);
            
            CountdownTimerRepeat scoreTimer = new CountdownTimerRepeat(timeForOneBar, currentTrack.bars);
            scoreTimer.OnTimerRaised += () =>
            {
                currentBarNumber++;
                OnDownBeat?.Invoke();
                scoreManager.addPoints(scoreManager.GetUpToDateTrack(currentTrack).points / scoreManager.GetUpToDateTrack(currentTrack).bars);
                scoreManager.ConsolidatePoints(currentTrack, ScoreContextEnum.BarStart);
            };

            scoreTimer.OnTimerEnd += OnSongEnd;

            if (TrackAbilities.EnumToAbility.TryGetValue(currentTrack.ability, out var ability))
            {
                List<TrackSO> allTracks = new List<TrackSO>();
                allTracks.AddRange(discoBall.GetAllTracks());
                allTracks.AddRange(activePlaylist.GetAllTracks());
                allTracks.AddRange(backupPlaylist.GetAllTracks());
                
                ability.startAction(scoreManager, currentTrack, allTracks);
                foreach (TimestampAction ta in ability.timestampActions)
                {
                    float adjustedTime = ta.audioTime;
                    
                    var taTimer = new CountdownTimer(adjustedTime);
                    taTimer.Start();
                    taTimer.OnTimerEnd += () => { ta.Action(scoreManager, currentTrack, allTracks); };
                }   
            }
            scoreTimer.Start();

        }

        private void Update()
        {
            if (currentTrack != null)
            {
                progress.Value = audioSource.time / (audioSource.clip.length * (currentTrack.bars / 4f)) * 100f;
            }
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
        
        public void Shuffle()
        {
            List<TrackSO> tracks = new List<TrackSO>();
            tracks.AddRange(discoBall.GetAllTracks());
            tracks.AddRange(activePlaylist.GetAllTracks());
            tracks.AddRange(backupPlaylist.GetAllTracks());
            
            System.Random random = new System.Random();
            int n = tracks.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (tracks[k], tracks[n]) = (tracks[n], tracks[k]);
            }
            
            discoBall.RemoveAll();
            activePlaylist.RemoveAll();
            backupPlaylist.RemoveAll();
            
            int i = 0;
            
            for (int j = 0; j < discoBall.Capacity; j++)
            {
                if(i >= tracks.Count) break;
            
                Debug.Log(discoBall.TryEnqueue(tracks[i]));
            
                i++;
            }
            
            for (int j = 0; j < activePlaylist.Capacity; j++)
            {
                if(i >= tracks.Count) break;
            
                Debug.Log(activePlaylist.TryEnqueue(tracks[i]));
            
                i++;
            }
            
            for (int j = 0; j < backupPlaylist.Capacity; j++)
            {
                if(i >= tracks.Count) break;
            
                Debug.Log(backupPlaylist.TryEnqueue(tracks[i]));
            
                i++;
            }

            firstRun = true;
        }
    }
}
