using System;
using Obvious.Soap;
using PrimeTween;
using UnityEngine;

public class SettingsOpen : MonoBehaviour
{
    [SerializeField] private Transform leftDisk;
    [SerializeField] private Transform rightDisk;
    [SerializeField] private Transform leftDiskStart;
    [SerializeField] private Transform rightDiskStart;
    [SerializeField] private Transform leftDiskEnd;
    [SerializeField] private Transform rightDiskEnd;

    [SerializeField] private BoolVariable fullScreen;

    [SerializeField]
    private GameObject ButtonCanvas;

    private void Start()
    {
        fullScreen.OnValueChanged += (value) => Screen.fullScreen = value;
        gameObject.SetActive(false);
    }

    public void Close()
    {
        Sequence.Create(Tween.Position(
            target: leftDisk,
            startValue: leftDiskEnd.position,
            endValue: leftDiskStart.position,
            duration: 0.3f,
            ease: Ease.InOutSine,
            cycles: 1)).Group(
            Tween.Rotation(
                target: leftDisk,
                startValue: leftDiskEnd.rotation,
                endValue: leftDiskStart.rotation,
                duration: 0.3f,
                ease: Ease.InOutSine,
                cycles: 1));
        Sequence.Create(Tween.Position(
            target: rightDisk,
            startValue: rightDiskEnd.position,
            endValue: rightDiskStart.position,
            duration: 0.3f,
            ease: Ease.InOutSine,
            cycles: 1)).Group(
            Tween.Rotation(
                target: rightDisk,
                startValue: rightDiskEnd.rotation,
                endValue: rightDiskStart.rotation,
                duration: 0.3f,
                ease: Ease.InOutSine,
                cycles: 1)).OnComplete(() =>
        {
            gameObject.SetActive(false);
            ButtonCanvas.SetActive(true);
        });
    }
    
    private void OnEnable()
    {
        Sequence.Create(Tween.Position(
            target: leftDisk,
            startValue: leftDiskStart.position,
            endValue: leftDiskEnd.position,
            duration: 0.3f,
            ease: Ease.InOutSine,
            cycles: 1)).Group(
            Tween.Rotation(
                target: leftDisk,
                startValue: leftDiskStart.rotation,
                endValue: leftDiskEnd.rotation,
                duration: 0.3f,
                ease: Ease.InOutSine,
                cycles: 1));
        Sequence.Create(Tween.Position(
            target: rightDisk,
            startValue: rightDiskStart.position,
            endValue: rightDiskEnd.position,
            duration: 0.3f,
            ease: Ease.InOutSine,
            cycles: 1)).Group(
            Tween.Rotation(
                target: rightDisk,
                startValue: rightDiskStart.rotation,
                endValue: rightDiskEnd.rotation,
                duration: 0.3f,
                ease: Ease.InOutSine,
                cycles: 1));
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
