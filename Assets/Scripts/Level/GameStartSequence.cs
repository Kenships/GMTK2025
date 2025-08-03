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
    [SerializeField] private ScriptableEventNoParam startCountDownEvent;
    
    [SerializeField] GameStateSO gameState;
    
    [SerializeField] IntVariable countDownTime;

    [SerializeField] private ScriptableEventLevelDataSO startGame;
    
    [SerializeField] List<PlaylistController> playlists;
    
    [SerializeField] private InventorySO inventory;
    
    [SerializeField] private Transform abilitySlots;

    private CountdownTimer countDownTimer;
    private AudioSource audioSource;

    private void Awake()
    {
        countDownTimer = new CountdownTimer(1.8f);
        audioSource = GetComponent<AudioSource>();
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
        
        countDownTimer.OnTimerEnd += () =>
        {
            startGame?.Raise(gameState.GetCurrentLevelData());
            audioSource.Stop();
        };

        startCountDownEvent.OnRaised += StartTimer;
        
    }

    private void OnDestroy()
    {
        startCountDownEvent.OnRaised -= StartTimer;
    }

    private void StartTimer()
    {
        countDownTimer.Start();
        audioSource.Play();
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
}
