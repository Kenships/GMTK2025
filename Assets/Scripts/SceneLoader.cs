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
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is not set in SceneLoader");
            return;
        }

        if (trigger == null)
        {
            Debug.LogError("Trigger event is not assigned in SceneLoader");
            return;
        }

        trigger.OnRaised += HandleSceneLoad;
    }

    private void HandleSceneLoad()
    {
        StartCoroutine(LoadLevelAsync());
    }

    IEnumerator LoadLevelAsync()
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' cannot be loaded. Make sure it exists and is added to build settings.");
            yield break;
        }

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        while(!loadOperation.isDone)
        {
            if (loadingProgress != null)
                loadingProgress.Value = loadOperation.progress;
            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (trigger != null)
            trigger.OnRaised -= HandleSceneLoad;
    }
}
