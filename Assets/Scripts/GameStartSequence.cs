using System;
using System.Collections.Generic;
using System.Linq;
using ImprovedTimers;
using Level;
using Obvious.Soap;
using TrackScripts;
using UnityEngine;

public class GameStartSequence : MonoBehaviour
{
    [SerializeField] GameStateSO gameState;
    
    [SerializeField] IntVariable countDownTime;

    [SerializeField] private ScriptableEventLevelDataSO startGame;
    
    [SerializeField] List<PlaylistController> playlists;
    
    [SerializeField] private ScriptableListTrackSO inventory;

    private CountdownTimer countDownTimer;

    private void Awake()
    {
        countDownTimer = new CountdownTimer(5f);
    }

    private void Start()
    {
        List<TrackSO> tracks = new List<TrackSO>(inventory);

        foreach (TrackSO track in tracks)
        {
            track.clip.LoadAudioData();
        }

        Shuffle(tracks);
        
        int i = 0;

        foreach (PlaylistController playlist in playlists)
        {
            for (int j = 0; j < playlist.Capacity; j++)
            {
                if(i >= tracks.Count) break;
                
                playlist.TryEnqueue(tracks[i]);
                
                i++;
            }
        }
        
        countDownTime.Value = 3;
        countDownTimer.OnTimerEnd += () => startGame?.Raise(gameState.GetCurrentLevelData());
        countDownTimer.Start();
    }

    private void Shuffle(List<TrackSO> tracks)
    {
        System.Random random = new System.Random();
        int n = tracks.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (tracks[k], tracks[n]) = (tracks[n], tracks[k]);
        }
    }

    private void Update()
    {
        if (countDownTimer.CurrentTime <= 3f)
        {
            countDownTime.Value = Mathf.RoundToInt(countDownTimer.CurrentTime);
        }
    }
}
