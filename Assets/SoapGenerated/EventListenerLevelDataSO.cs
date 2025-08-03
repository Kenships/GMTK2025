using Level;
using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;

[AddComponentMenu("Soap/EventListeners/EventListener"+nameof(LevelDataSO))]
public class EventListenerLevelDataSO : EventListenerGeneric<LevelDataSO>
{
    [SerializeField] private EventResponse[] _eventResponses = null;
    protected override EventResponse<LevelDataSO>[] EventResponses => _eventResponses;

    [System.Serializable]
    public class EventResponse : EventResponse<LevelDataSO>
    {
        [SerializeField] private ScriptableEventLevelDataSO _scriptableEvent = null;
        public override ScriptableEvent<LevelDataSO> ScriptableEvent => _scriptableEvent;

        [SerializeField] private LevelDataSOUnityEvent _response = null;
        public override UnityEvent<LevelDataSO> Response => _response;
    }

    [System.Serializable]
    public class LevelDataSOUnityEvent : UnityEvent<LevelDataSO>
    {
        
    }
}
