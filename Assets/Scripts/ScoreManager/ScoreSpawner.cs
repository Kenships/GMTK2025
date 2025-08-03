using System;
using Obvious.Soap;
using PrimeTween;
using TMPro;
using UnityEngine;

public class ScoreSpawner : MonoBehaviour
{
    [SerializeField] private Transform disc;
    
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject prefab;

    [SerializeField] private Transform endPosition;

    [SerializeField] private IntVariable Score;
    [SerializeField] private IntVariable PrevScore;
    
    [SerializeField] private Color positive;
    [SerializeField] private Color negative;

    private void Start()
    {
        Score.OnValueChanged += SpawnScore;
    }

    private void SpawnScore(int obj)
    {
        int gainedScore = Score.Value - PrevScore.Value;
        
        GameObject scoreText = Instantiate(prefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        scoreText.GetComponent<TextMeshProUGUI>().text = gainedScore > 0 ? $"+{gainedScore}pt" : $"{gainedScore}pt";
        scoreText.GetComponent<TextMeshProUGUI>().color = gainedScore > 0 ? positive : negative;
        Sequence.Create(Tween.Scale(
            target: scoreText.transform,
            endValue: 1.2f,
            duration: 0.2f,
            ease: Ease.InOutExpo,
            cycles: 2,
            cycleMode: CycleMode.Rewind
        )).Group(Tween.Position(
            target: scoreText.transform,
            endValue: endPosition.position,
            duration: 0.4f,
            ease: Ease.OutExpo,
            cycles: 1
        )).Group(Tween.Scale(
                target: disc.transform,
                endValue: 1.2f,
                duration: 0.2f,
                ease: Ease.InOutExpo,
                cycles: 2,
                cycleMode: CycleMode.Rewind
            )
                ).ChainDelay(0.6f)
        .OnComplete(() => Destroy(scoreText));
        
    }
}
