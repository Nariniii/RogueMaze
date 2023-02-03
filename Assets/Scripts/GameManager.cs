using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Menu _menu;
    [SerializeField]
    private GameController _gameController;
    [SerializeField]
    private GameObject _menuCanvasObject;
    [SerializeField]
    private GameObject _gameCanvasObject;
    private string _playerJobName;
    private bool _isGameScene = false;
    private int _stageNumber;

    private void Awake()
    {
        _menuCanvasObject.SetActive(true);
        _gameCanvasObject.SetActive(false);
    }

    private void Start()
    {
        Debug.Log("gameManager Start");
        GameManagerInit();
        _menu.changeSceneCallback.AddListener(ChangeGameScene);
        _menu.gameContinueCallback.AddListener(GameContinue);
        _gameController.gameGoalCallback.AddListener(GameGoal);
    }

    private void Update()
    {
        if (!_isGameScene) return;
        if (_gameController.IsPlayerDead())
        {
            _gameCanvasObject.SetActive(false);
            _menuCanvasObject.SetActive(true);
            _menu.DeadOrContinue();
        }
        if (_isGameScene && _gameController.IsGameSetup)
        {
            _gameController.Move();
            //if (_gameController.GetRandomMoveEnemyCount() != 0)
            //{
            //    _gameController.RandomMoveEnemy();
            //}
            //if (_gameController.GetEngageEnemyCount() != 0)
            //{
            //    _gameController.FollowingMoveEnemy();
            //}
        }
    }

    private void GameManagerInit()
    {
        _isGameScene = false;
        _gameController.IsGameSetup = false;
        _stageNumber = 0;
    }

    private void GameGoal()
    {
        _stageNumber = _gameController.StageNumber;
        _gameController.SetStageNumber(_stageNumber);
        StartCoroutine(_gameController.GameInit(_playerJobName));
    }

    private void GameContinue()
    {
        _stageNumber = _gameController.StageNumber;
        _gameController.SetStageNumber(_stageNumber);
        StartCoroutine(_gameController.GameInit(_playerJobName));
    }

    private void ChangeGameScene()
    {
        Debug.Log("Change scene");
        _playerJobName = _menu.PlayerTypeName;
        //SceneManager.LoadScene("Game");
        _menuCanvasObject.SetActive(false);
        _gameCanvasObject.SetActive(true);
        _isGameScene = true;
        GameStart();
    }

    private void GameStart()
    {
        _gameController.SetStageNumber(_stageNumber);
        StartCoroutine(_gameController.GameInit(_playerJobName));
    }

}
