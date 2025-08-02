using System;
using System.Collections.Generic;
using ImprovedTimers;
using Obvious.Soap;
using TrackScripts;
using UnityEngine;

public class GameStartSequence : MonoBehaviour
{
    [SerializeField] IntVariable countDownTime;

    [SerializeField]
    private ScriptableEventNoParam startGame;
    
    [SerializeField] List<PlaylistController> playlists;
    
    [SerializeField] private InventorySO inventory;

    private CountdownTimer countDownTimer;

    private void Awake()
    {
        countDownTimer = new CountdownTimer(5f);
    }

    private void Start()
    {
        List<TrackSO> tracks = inventory.tracks;

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
        countDownTimer.OnTimerEnd += StartGame;
        countDownTimer.Start();
    }

    private void StartGame()
    {
        startGame?.Raise();
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
