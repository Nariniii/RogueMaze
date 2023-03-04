using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(EventTrigger))]
public class ImageButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private EventTrigger _trigger;
    [SerializeField]
    private CanvasGroup _canvasGroup;

    public System.Action onClickCallback;
    public EventTrigger.Entry _entryUp;
    public EventTrigger.Entry _entryDown;
    public EventTrigger.Entry _entryClick;
    public EventTrigger.Entry _entryEnter;
    public EventTrigger.Entry _entryExit;

    public delegate void ButtonFunc(BaseEventData eventData);

    private void Awake()
    {
        _entryClick.eventID = EventTriggerType.PointerClick;
        _entryUp.eventID = EventTriggerType.PointerUp;
        _entryDown.eventID = EventTriggerType.PointerDown;
        _entryEnter.eventID = EventTriggerType.PointerEnter;
        _entryExit.eventID = EventTriggerType.PointerExit;
    }

    public void ResetTrigger()
    {
        _trigger.triggers.Clear();
    }

    public void UpAddListener(ButtonFunc func)
    {
        _entryUp.callback.AddListener((eventData) => { func(eventData); });
        _trigger.triggers.Add(_entryUp);
    }

    public void DownAddListener(ButtonFunc func)
    {
        _entryDown.callback.AddListener((eventData) => { func(eventData); });
        _trigger.triggers.Add(_entryDown);
    }

    public void EnterAddListener(ButtonFunc func)
    {
        _entryEnter.callback.AddListener((eventData) => { func(eventData); });
        _trigger.triggers.Add(_entryEnter);
    }

    public void ExitAddListener(ButtonFunc func)
    {
        _entryExit.callback.AddListener((eventData) => { func(eventData); });
        _trigger.triggers.Add(_entryExit);
    }

    public void ClickAddListener(ButtonFunc func)
    {
        _entryClick.callback.AddListener((eventData) => { func(eventData); });
        _trigger.triggers.Add(_entryClick);
    }

    public void DOScale(Vector3 value, float duration)
    {
        this.gameObject.transform.DOScale(value, duration).SetEase(Ease.OutCubic);
    }

    public void DOFade(float value, float duration)
    {
        _canvasGroup.DOFade(value, duration).SetEase(Ease.OutCubic);
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
