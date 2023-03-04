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

    public bool IsDeadOrContinueButtonActive { get; set; }

    public string PlayerTypeName { get; private set; }

    public UnityEvent changeSceneCallback = new UnityEvent();
    public UnityEvent gameContinueCallback = new UnityEvent();

    private void Start()
    {
        MenuInit();
    }

    public void MenuInit()
    {
        StartButtonInit();
    }

    private void StartButtonInit()
    {
        IsDeadOrContinueButtonActive = false;
        _startButton.gameObject.SetActive(true);
        _playerSelectButtonsObject.SetActive(false);
        _gameProgressButtonsObject.SetActive(false);

        _startButton.DownAddListener(StartButtonDown);
        _startButton.UpAddListener(StartButtonUp);
    }

    public void StartButtonDown(BaseEventData eventData)
    {
        ButtonEnterAnimation(_startButton);
    }

    public void StartButtonUp(BaseEventData eventData)
    {
        ButtonExitAnimation(_startButton);
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

        _soldierButton.ResetTrigger();
        _mageButton.ResetTrigger();
        _thiefButton.ResetTrigger();

        _soldierButton.EnterAddListener(PlayerSelectButtonEnter);
        _mageButton.EnterAddListener(PlayerSelectButtonEnter);
        _thiefButton.EnterAddListener(PlayerSelectButtonEnter);

        _soldierButton.ExitAddListener(PlayerSelectButtonExit);
        _mageButton.ExitAddListener(PlayerSelectButtonExit);
        _thiefButton.ExitAddListener(PlayerSelectButtonExit);

        _soldierButton.ClickAddListener(PlayerSelectButtonClick);
        _mageButton.ClickAddListener(PlayerSelectButtonClick);
        _thiefButton.ClickAddListener(PlayerSelectButtonClick);
    }

    public void PlayerSelectButtonEnter(BaseEventData eventData)
    {
        GameObject pointerObj = (eventData as PointerEventData).pointerEnter;
        if (pointerObj.tag == null)
        {
            Debug.LogWarning("pointerEnter info null.");
            return;
        }
        switch (pointerObj.tag)
        {
            case "Soldier":
                ButtonEnterAnimation(_soldierButton);
                break;
            case "Mage":
                ButtonEnterAnimation(_mageButton);
                break;

            case "Thief":
                ButtonEnterAnimation(_thiefButton);
                break;
        }
    }

    public void PlayerSelectButtonExit(BaseEventData eventData)
    {
        GameObject pointerObj = (eventData as PointerEventData).pointerEnter;
        if (pointerObj.tag == null)
        {
            Debug.LogWarning("pointerExit info null.");
            return;
        }
        switch (pointerObj.tag)
        {
            case "Soldier":
                ButtonExitAnimation(_soldierButton);
                break;
            case "Mage":
                ButtonExitAnimation(_mageButton);
                break;
            case "Thief":
                ButtonExitAnimation(_thiefButton);
                break;
        }
    }

    private void ButtonEnterAnimation(ImageButton button)
    {
        button.DOScale(new Vector3(.95f, .95f), .24f);
        button.DOFade(.8f, .24f);
    }

    private void ButtonExitAnimation(ImageButton button)
    {
        button.DOScale(new Vector3(1f, 1f), .24f);
        button.DOFade(1f, .24f);
    }

    public void PlayerSelectButtonClick(BaseEventData eventData)
    {
        GameObject pointerObj = (eventData as PointerEventData).pointerEnter;
        if (pointerObj.tag == null)
        {
            Debug.LogWarning("pointerClick info null.");
            return;
        }
        PlayerTypeName = pointerObj.name;
        Debug.Log(PlayerTypeName);
        changeSceneCallback.Invoke();
    }

    public void GameProgressButtonsInit()
    {
        IsDeadOrContinueButtonActive = true;
        _gameProgressButtonsObject.SetActive(true);
        _playerSelectButtonsObject.SetActive(false);
        _startButton.gameObject.SetActive(false);

        _gameOverButton.DownAddListener(GameOverButtonDown);
        _gameOverButton.UpAddListener(GameOverButtonUp);

        _continueButton.DownAddListener(ContinueButtonDown);
        _continueButton.UpAddListener(ContinueButtonUp);
    }

    public void GameOverButtonDown(BaseEventData eventData)
    {
        ButtonEnterAnimation(_gameOverButton);
    }

    public void GameOverButtonUp(BaseEventData eventData)
    {
        ButtonExitAnimation(_gameOverButton);
        StartButtonInit();
    }

    public void ContinueButtonDown(BaseEventData eventData)
    {
        ButtonEnterAnimation(_continueButton);
    }

    public void ContinueButtonUp(BaseEventData eventData)
    {
        ButtonExitAnimation(_continueButton);
        gameContinueCallback.Invoke();
    }

    public void DeadOrContinue()
    {
        if (IsDeadOrContinueButtonActive) return;
        GameProgressButtonsInit();
    }
}
