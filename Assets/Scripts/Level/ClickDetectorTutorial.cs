using System;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level
{
    public class ClickDetectorTutorial : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private ScriptableEventNoParam next;

        public void OnPointerDown(PointerEventData eventData)
        {
            next.Raise();
        }
    }
}
