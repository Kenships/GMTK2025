using System;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level
{
    public class TutorialModifier : MonoBehaviour
    {
        [SerializeField]
        private ScriptableEventNoParam tutorialModifier;

        private bool first = true;
        private void Update()
        {
            if (first && transform.childCount > 0)
            {
                tutorialModifier.Raise();
                first = false;
            }
        }
    }
}
