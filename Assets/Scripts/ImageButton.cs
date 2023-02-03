using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]

public class ImageButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public System.Action onClickCallback;
    public EventTrigger _trigger;
    public List<EventTrigger.Entry> _entryList;
    public EventTrigger.Entry _entryUp;
    public EventTrigger.Entry _entryDown;
    public EventTrigger.Entry _entryClick;
    public EventTrigger.Entry _entryEnter;
    public EventTrigger.Entry _entryExit;

    private void Awake()
    {
        this.gameObject.AddComponent<EventTrigger>();
        _trigger = this.gameObject.GetComponent<EventTrigger>();
        _entryList = new List<EventTrigger.Entry>();
        _entryClick.eventID = EventTriggerType.PointerClick;
        _entryUp.eventID = EventTriggerType.PointerUp;
        _entryDown.eventID = EventTriggerType.PointerDown;
        _entryEnter.eventID = EventTriggerType.PointerEnter;
        _entryExit.eventID = EventTriggerType.PointerExit;
    }

    public void SetTrigger(string entryTypeName)
    {
        switch (entryTypeName)
        {
            case "Up":
                _trigger.triggers.Add(_entryUp);
                break;
            case "Down":
                _trigger.triggers.Add(_entryDown);
                break;
            case "Click":
                _trigger.triggers.Add(_entryClick);
                break;
            case "Enter":
                _trigger.triggers.Add(_entryEnter);
                break;
            case "Exit":
                _trigger.triggers.Add(_entryExit);
                break;
            default:
                Debug.LogWarning("No match entryType");
                break;
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallback?.Invoke();
    }

    public void OnPointerDown(PointerEventData evnetData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
