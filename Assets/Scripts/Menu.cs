using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private ImageButton _startButton;
    [SerializeField]
    private CanvasGroup _startButtonCanvasGroup;
    [SerializeField]
    private GameObject _playerSelectButtonsObject;
    [SerializeField]
    private ImageButton _soldierButton;
    [SerializeField]
    private CanvasGroup _soldierButtonCanvasGroup;
    [SerializeField]
    private ImageButton _mageButton;
    [SerializeField]
    private CanvasGroup _mageButtonCanvasGroup;
    [SerializeField]
    private ImageButton _thiefButton;
    [SerializeField]
    private CanvasGroup _thiefButtonCanvasGroup;
    [SerializeField]
    private GameObject _gameProgressButtonsObject;
    [SerializeField]
    private ImageButton _gameOverButton;
    [SerializeField]
    private CanvasGroup _gameOverButtonCanvasGroup;
    [SerializeField]
    private ImageButton _continueButton;
    [SerializeField]
    private CanvasGroup _continueButtonCanvasGroup;



    public string PlayerTypeName { get; private set; }

    public UnityEvent changeSceneCallback = new UnityEvent();
    public UnityEvent gameContinueCallback = new UnityEvent();

    private void Start()
    {
        StartButtonInit();
    }

    private void StartButtonInit()
    {
        _startButton.gameObject.SetActive(true);
        _playerSelectButtonsObject.SetActive(false);
        _gameProgressButtonsObject.SetActive(false);

        _startButton._entryDown.callback.AddListener((eventData) => { StartButtonDown(); });
        _startButton._entryUp.callback.AddListener((eventData) => { StartButtonUp(); });
        _startButton.SetTrigger("Down");
        _startButton.SetTrigger("Up");
    }

    public void StartButtonDown()
    {
        _startButton.gameObject.transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic);
        _startButtonCanvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic);
    }

    public void StartButtonUp()
    {
        _startButton.gameObject.transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic);
        _startButtonCanvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic);
        StartButton();
    }

    public void StartButton()
    {
        _startButton.gameObject.SetActive(false);
        PlayerSelectButtonInit();
    }

    public void PlayerSelectButtonInit()
    {
        _playerSelectButtonsObject.SetActive(true);

        _soldierButton._entryEnter.callback.AddListener((eventData) => { PlayerSelectButtonEnter(eventData); });
        _mageButton._entryEnter.callback.AddListener((eventData) => { PlayerSelectButtonEnter(eventData); });
        _thiefButton._entryEnter.callback.AddListener((eventData) => { PlayerSelectButtonEnter(eventData); });

        _soldierButton.SetTrigger("Enter");
        _mageButton.SetTrigger("Enter");
        _thiefButton.SetTrigger("Enter");

        _soldierButton._entryExit.callback.AddListener((eventData) => { PlayerSelectButtonExit(eventData); });
        _mageButton._entryExit.callback.AddListener((eventData) => { PlayerSelectButtonExit(eventData); });
        _thiefButton._entryExit.callback.AddListener((eventData) => { PlayerSelectButtonExit(eventData); });

        _soldierButton.SetTrigger("Exit");
        _mageButton.SetTrigger("Exit");
        _thiefButton.SetTrigger("Exit");

        _soldierButton._entryClick.callback.AddListener((eventData) => { PlayerSelectButtonClick(eventData); });
        _mageButton._entryClick.callback.AddListener((eventData) => { PlayerSelectButtonClick(eventData); });
        _thiefButton._entryClick.callback.AddListener((eventData) => { PlayerSelectButtonClick(eventData); });

        _soldierButton.SetTrigger("Click");
        _mageButton.SetTrigger("Click");
        _thiefButton.SetTrigger("Click");
    }

    public void PlayerSelectButtonEnter(BaseEventData eventData)
    {
        GameObject pointerObj = (eventData as PointerEventData).pointerEnter;
        if (pointerObj.tag == null)
        {
            Debug.LogWarning("pointerEnter info null");
            return;
        }
        switch (pointerObj.tag)
        {
            case "Soldier":
                _soldierButton.gameObject.transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic);
                _soldierButtonCanvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic);
                break;

            case "Mage":
                _mageButton.gameObject.transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic);
                _mageButtonCanvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic);
                break;

            case "Thief":
                _thiefButton.gameObject.transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic);
                _thiefButtonCanvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic);
                break;

            default:
                Debug.LogWarning("No tag character");
                break;
        }
    }

    public void PlayerSelectButtonExit(BaseEventData eventData)
    {
        GameObject pointerObj = (eventData as PointerEventData).pointerEnter;
        if (pointerObj.tag == null)
        {
            Debug.LogWarning("pointerExit info null");
            return;
        }
        switch (pointerObj.tag)
        {
            case "Soldier":
                _soldierButton.gameObject.transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic);
                _soldierButtonCanvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic);
                break;

            case "Mage":
                _mageButton.gameObject.transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic);
                _mageButtonCanvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic);
                break;

            case "Thief":
                _thiefButton.gameObject.transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic);
                _thiefButtonCanvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic);
                break;

            default:
                Debug.LogWarning("No tag character");
                break;
        }
    }

    public void PlayerSelectButtonClick(BaseEventData eventData)
    {
        GameObject pointerObj = (eventData as PointerEventData).pointerEnter;
        if (pointerObj.tag == null)
        {
            Debug.LogWarning("No tag obj");
            return;
        }
        PlayerTypeName = pointerObj.name;
        Debug.Log(PlayerTypeName);
        changeSceneCallback.Invoke();
    }

    public void GameProgressButtonsInit()
    {
        _gameProgressButtonsObject.SetActive(true);
        _playerSelectButtonsObject.SetActive(false);
        _startButton.gameObject.SetActive(false);

        _gameOverButton._entryDown.callback.AddListener((eventData) => { GameOverButtonDown(); });
        _gameOverButton._entryUp.callback.AddListener((eventData) => { GameOverButtonUp(); });
        _gameOverButton.SetTrigger("Down");
        _gameOverButton.SetTrigger("Up");

        _continueButton._entryDown.callback.AddListener((eventData) => { ContinueButtonDown(); });
        _continueButton._entryUp.callback.AddListener((eventData) => { ContinueButtonUp(); });
        _continueButton.SetTrigger("Down");
        _continueButton.SetTrigger("Up");
    }

    public void GameOverButtonDown()
    {
        _gameOverButton.gameObject.transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic);
        _gameOverButtonCanvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic);
    }

    public void GameOverButtonUp()
    {
        _gameOverButton.gameObject.transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic);
        _gameOverButtonCanvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic);
        StartButtonInit();
    }

    public void ContinueButtonDown()
    {
        _continueButton.gameObject.transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic);
        _continueButtonCanvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic);
    }

    public void ContinueButtonUp()
    {
        _continueButton.gameObject.transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic);
        _continueButtonCanvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic);
        gameContinueCallback.Invoke();
    }

    public void DeadOrContinue()
    {
        GameProgressButtonsInit();
    }
}
