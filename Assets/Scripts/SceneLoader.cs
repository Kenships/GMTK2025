using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private ScriptableEventNoParam trigger;

    [SerializeField]
    private FloatVariable loadingProgress;
    
    [SerializeField]
    private string sceneName;
    
    void Start()
    {
        trigger.OnRaised += () => StartCoroutine(LoadLevelAsync());
    }

    IEnumerator LoadLevelAsync()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        
        while(!loadOperation.isDone)
        {
            loadingProgress.Value = loadOperation.progress;
            yield return null;
        }
    }
}
