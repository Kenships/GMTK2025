using System;
using System.Collections.Generic;
using Obvious.Soap;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsOpen : MonoBehaviour
{
    [SerializeField] private Transform leftDisk;
    [SerializeField] private Transform rightDisk;
    [SerializeField] private Transform leftDiskStart;
    [SerializeField] private Transform rightDiskStart;
    [SerializeField] private Transform leftDiskEnd;
    [SerializeField] private Transform rightDiskEnd;

    [SerializeField] private BoolVariable fullScreen;
    
    [SerializeField] private List<AudioSource> audioSources;

    [SerializeField]
    private InputAction openSettings;

    private AudioSource settingsMusic;
    
    private void Start()
    {
        openSettings.Enable();
        openSettings.performed += Open;
        fullScreen.OnValueChanged += UpdateFullscreen;
        Time.timeScale = 1f;
        foreach (var audioSource in audioSources)
        {
            audioSource.UnPause();
        }
        settingsMusic.Stop();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        openSettings.Disable();
        openSettings.performed -= Open;
        fullScreen.OnValueChanged -= UpdateFullscreen;
    }

    private void Open(InputAction.CallbackContext callbackContext)
    {
        
        gameObject.SetActive(true);
    }

    private void UpdateFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void Close()
    {
        
        Sequence.Create(useUnscaledTime:true).Group(Tween.Position(
            target: leftDisk,
            startValue: leftDiskEnd.position,
            endValue: leftDiskStart.position,
            duration: 0.3f,
            ease: Ease.InOutSine,
            cycles: 1,
            useUnscaledTime: true)).Group(
            Tween.Rotation(
                target: leftDisk,
                startValue: leftDiskEnd.rotation,
                endValue: leftDiskStart.rotation,
                duration: 0.3f,
                ease: Ease.InOutSine,
                cycles: 1,
                useUnscaledTime: true))
            .Group(Tween.Position(
            target: rightDisk,
            startValue: rightDiskEnd.position,
            endValue: rightDiskStart.position,
            duration: 0.3f,
            ease: Ease.InOutSine,
            cycles: 1,
            useUnscaledTime: true)).Group(
            Tween.Rotation(
                target: rightDisk,
                startValue: rightDiskEnd.rotation,
                endValue: rightDiskStart.rotation,
                duration: 0.3f,
                ease: Ease.InOutSine,
                cycles: 1,
                useUnscaledTime: true)).OnComplete(() =>
        {
            Time.timeScale = 1f;
            foreach (var audioSource in audioSources)
            {
                audioSource.UnPause();
            }
            settingsMusic.Stop();
            gameObject.SetActive(false);
        });
        
    }
    
    private void OnEnable()
    {
        if (!settingsMusic)
        {
            settingsMusic = GetComponent<AudioSource>();
        }
        
        Time.timeScale = 0f;
        foreach (var audioSource in audioSources)
        {
            audioSource.Pause();
        }
        settingsMusic.Play();
        Sequence.Create(useUnscaledTime:true).Group(Tween.Position(
            target: leftDisk,
            startValue: leftDiskStart.position,
            endValue: leftDiskEnd.position,
            duration: 0.3f,
            ease: Ease.InOutSine,
            cycles: 1,
            useUnscaledTime: true)).Group(
            Tween.Rotation(
                target: leftDisk,
                startValue: leftDiskStart.rotation,
                endValue: leftDiskEnd.rotation,
                duration: 0.3f,
                ease: Ease.InOutSine,
                cycles: 1,
                useUnscaledTime: true))
            .Group(Tween.Position(
            target: rightDisk,
            startValue: rightDiskStart.position,
            endValue: rightDiskEnd.position,
            duration: 0.3f,
            ease: Ease.InOutSine,
            cycles: 1,
            useUnscaledTime: true)).Group(
            Tween.Rotation(
                target: rightDisk,
                startValue: rightDiskStart.rotation,
                endValue: rightDiskEnd.rotation,
                duration: 0.3f,
                ease: Ease.InOutSine,
                cycles: 1,
                useUnscaledTime: true));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenItch()
    {
        Application.OpenURL("https://kenships.itch.io/queuechaos");
    }

    public void OpenGithub()
    {
        Application.OpenURL("https://github.com/Kenships/GMTK2025");
    }
}
