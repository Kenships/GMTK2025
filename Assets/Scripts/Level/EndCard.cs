using System;
using System.Collections;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.UI;

namespace Level
{
    public class EndCard : MonoBehaviour
    {
        [SerializeField] private GameObject endScreen;
        
        [SerializeField] private GameObject winScoreText;
        [SerializeField] private Sprite winScoreSprite;
        [SerializeField] private GameObject winScoreButton;
        
        [SerializeField] private GameObject loseScoreText;
        [SerializeField] private Sprite loseScoreSprite;
        [SerializeField] private GameObject loseScoreButton;
        
        [SerializeField] private IntVariable score;
        [SerializeField] private IntVariable scoreDisplay;

        [SerializeField] private IntVariable overScore;
        [SerializeField] private IntVariable overScoreDisplay;
        [SerializeField] private GameObject overScoreText;

        [SerializeField] private IntVariable moneyGained;
        [SerializeField] private IntVariable moneyDisplay;

        [SerializeField] private GameObject moneyGainedText;
        
        [SerializeField] private IntVariable money;
       
        [SerializeField] private GameObject totalMoneyText;
        

        private void OnEnable()
        {
            Debug.Log("Starting end card");
            overScoreText.SetActive(false);
            moneyGainedText.SetActive(false);
            totalMoneyText.SetActive(false);
            winScoreText.SetActive(false);
            winScoreButton.SetActive(false);
            loseScoreText.SetActive(false);
            loseScoreButton.SetActive(false);

            if (overScore >= 0)
            {
                
            }
            
            endScreen.GetComponent<Image>().sprite = overScore >= 0 ? winScoreSprite : loseScoreSprite;
            
            StartCoroutine(LerpWin());
        }

        IEnumerator LerpLose()
        {
            yield return new WaitForSeconds(0.5f);
            
            float t = 0;
            
            while (true)
            {
                if (t <= 1)
                {
                    scoreDisplay.Value = (int) Mathf.Lerp(0, score.Value, t);
                }
                else
                {
                    scoreDisplay.Value = score.Value;
                    break;
                }
                
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            
            yield return new WaitForSeconds(0.6f);
            loseScoreButton.SetActive(false);
        }

        IEnumerator LerpWin()
        {
            yield return new WaitForSeconds(0.5f);
            
            float t = 0;
            
            while (true)
            {
                if (t <= 1)
                {
                    scoreDisplay.Value = (int) Mathf.Lerp(0, score.Value, t);
                }
                else
                {
                    scoreDisplay.Value = score.Value;
                    break;
                }
                
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            
            yield return new WaitForSeconds(0.6f);
            
            overScoreText.SetActive(true);
            
            t = 0;
            while (true)
            {
                if (t <= 1)
                {
                    overScoreDisplay.Value = (int) Mathf.Lerp(0, overScore.Value, t);
                    
                    scoreDisplay.Value = score.Value - overScoreDisplay.Value;
                }
                else
                {
                    overScoreDisplay.Value = overScore.Value;
                    
                    scoreDisplay.Value = score.Value - overScore.Value;
                    break;
                }
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            
            yield return new WaitForSeconds(0.4f);
            
            moneyGainedText.SetActive(true);
            
            t = 0;
            
            while (true)
            {
                if (t <= 1)
                {
                    moneyDisplay.Value = (int) Mathf.Lerp(0, moneyGained, t);
                }
                else
                {
                    moneyDisplay.Value = moneyGained.Value;
                    break;
                }
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            
            yield return new WaitForSeconds(0.2f);
            
            totalMoneyText.SetActive(true);
            winScoreButton.SetActive(true);
        }

        private void Update()
        {
            
        }
    }
}
