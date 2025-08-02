using System;
using Obvious.Soap;
using UnityEngine;

public enum AudioType
{
    Master,
    Background,
    SFX
}

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    [SerializeField] private FloatVariable MasterVolume;
    [SerializeField] private FloatVariable SFXVolume;
    [SerializeField] private FloatVariable BackgroundVolume;
    [SerializeField] private BoolVariable Mute;
    [SerializeField] private AudioType audioType;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        MasterVolume.OnValueChanged += UpdateMasterVolume;
        SFXVolume.OnValueChanged += UpdateSFXVolume;
        BackgroundVolume.OnValueChanged += UpdateBackgroundVolume;
        Mute.OnValueChanged += UpdateMute;
        
        UpdateMasterVolume(MasterVolume.Value);
    }
    

    private void OnDestroy()
    {
        MasterVolume.OnValueChanged -= UpdateBackgroundVolume;
        SFXVolume.OnValueChanged -= UpdateSFXVolume;
        BackgroundVolume.OnValueChanged -= UpdateBackgroundVolume;
        Mute.OnValueChanged -= UpdateMute;
    }
    
    private void UpdateMute(bool isMute)
    {
        audioSource.mute = isMute;
    }

    private void UpdateMasterVolume(float volume)
    {
        if (audioType == AudioType.Master)
        {
            audioSource.volume = volume;
            return;
        }
        
        UpdateSFXVolume(volume);
        UpdateBackgroundVolume(volume);
    }

    private void UpdateSFXVolume(float volume)
    {
        if (audioType == AudioType.SFX)
        {
            audioSource.volume = SFXVolume.Value * MasterVolume.Value / 10000f;
        }
    }

    private void UpdateBackgroundVolume(float volume)
    {
        if (audioType == AudioType.Background)
        {
            audioSource.volume = BackgroundVolume.Value * MasterVolume.Value / 10000f;
        }
    }
}
