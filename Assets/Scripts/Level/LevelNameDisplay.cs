using System;
using TMPro;
using UnityEngine;

namespace Level
{
    public class LevelNameDisplay : MonoBehaviour
    {
        [SerializeField] GameStateSO gameState;

        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = gameState.GetCurrentLevelData().levelName;
        }
    }
}
