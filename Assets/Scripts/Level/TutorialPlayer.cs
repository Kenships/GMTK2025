using System;
using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Serialization;

namespace Level
{
    public class TutorialPlayer : MonoBehaviour
    {
        [SerializeField]
        private BoolVariable tutorialPlayed;

        [SerializeField] private ScriptableEventNoParam next;
        [SerializeField] private ScriptableEventNoParam tutorialModifier;
        [SerializeField] private ScriptableEventLevelDataSO startTutorial;
        
        [SerializeField] private GameObject backUpPlaylistDragText;
        [SerializeField] private GameObject activePlaylistText;
        [SerializeField] private GameObject discoPlaylistText;
        
        [SerializeField] private GameObject itemText;
        [SerializeField] private GameObject scoreText;
        
        [SerializeField] private GameObject modifierText;
        [SerializeField] private GameObject modifierText2;
        
        [SerializeField] private List<AudioSource> audioSources;
        
        private AudioSource audioSource;
        
        private bool queueNext;
        private bool modifier;
        
        private bool pause;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            backUpPlaylistDragText.SetActive(false);
            activePlaylistText.SetActive(false);
            discoPlaylistText.SetActive(false);
            itemText.SetActive(false);
            scoreText.SetActive(false);
            modifierText.SetActive(false);
            


            startTutorial.OnRaised += PrepareTutorial;
            next.OnRaised += QueueNext;
            tutorialModifier.OnRaised += OnTutorialModifier;
        }

        private void OnTutorialModifier()
        {
            modifier = true;
        }

        private void Update()
        {
            if (pause)
            {
                foreach (var item in audioSources)
                {
                    item.Pause();
                }
                audioSource.UnPause();
            }
            else
            {
                foreach (var item in audioSources)
                {
                    item.UnPause();
                }
                audioSource.Pause();
            }
        }

        private void PrepareTutorial(LevelDataSO obj)
        {
            if (tutorialPlayed.Value)
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(RunTutorial());
            }
        }

        private void OnDestroy()
        {
            next.OnRaised -= QueueNext;
        }

        private void QueueNext()
        {
            queueNext = true;
        }

        IEnumerator RunTutorial()
        {
            audioSource.Play();
            pause = true;
            
            Time.timeScale = 0;
            
            discoPlaylistText.SetActive(true);
            yield return new WaitUntil(() => queueNext);
            discoPlaylistText.SetActive(false);
            queueNext = false;
            
            activePlaylistText.SetActive(true);
            yield return new WaitUntil(() => queueNext);
            activePlaylistText.SetActive(false);
            queueNext = false;
            
            backUpPlaylistDragText.SetActive(true);
            yield return new WaitUntil(() => queueNext);
            backUpPlaylistDragText.SetActive(false);
            queueNext = false;
            
            itemText.SetActive(true);
            yield return new WaitUntil(() => queueNext);
            itemText.SetActive(false);
            queueNext = false;
            
            itemText.SetActive(false);
            scoreText.SetActive(true);
            yield return new WaitUntil(() => queueNext);
            scoreText.SetActive(false);
            
            Time.timeScale = 1;
            
            pause = false;
            
            queueNext = false;
            
            yield return new WaitUntil(() => modifier);
            Time.timeScale = 0;
            
            pause = true;
            
            modifierText.SetActive(true);
            queueNext = false;
            
            yield return new WaitUntil(() => queueNext);
            modifierText.SetActive(false);
            queueNext = false;
            
            modifierText2.SetActive(true);
            yield return new WaitUntil(() => queueNext);
            modifierText2.SetActive(false);
            queueNext = false;
            
            Time.timeScale = 1;
            
            pause = false;
            
            audioSource.Stop();
        }
    }
}
