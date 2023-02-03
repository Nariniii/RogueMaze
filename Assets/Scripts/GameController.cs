using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GridLayoutGroup _stageGridLayoutGroup;
    [SerializeField]
    private GridLayoutGroup _lifebarGridLayoutGroup;
    [SerializeField]
    private RectTransform _stageGridRect;
    [SerializeField]
    private Image _lifeElement;
    [SerializeField]
    private MazePanelElement _panelElement;
    [SerializeField]
    private Enemy _enemyElement;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private RectTransform _gameCanvasRect;
    [SerializeField]
    private RectTransform _lifebarRect;
    [SerializeField]
    private CanvasGroup _blackoutCanvasGroup;

    private int _rows;
    private int _columns;
    private int _dirX;
    private int _dirY;
    private float _scale;
    private Vector3 _gridPositionDiff;
    private List<Enemy> _enemyList = new List<Enemy>();
    private List<Enemy> _enemyRandomMoveList;
    private List<Enemy> _enemyEngageList;
    private List<string> _mazeStringList;
    private Dictionary<Position, MazePanelElement> _panelPositionDict;
    private Position _playerPosition;
    private Position _startPosition;
    private Position _keyPosition;
    private Position _doorPosition;
    private Position _goalPosition;
    private List<Position> _potionPositionList;
    private List<Position> _enemyPositionList;
    private List<Image> _lifeImageList;
    private string _enemyLevelInformation;
    private bool _haveKey;
    private bool _isPositionUpdate;
    private bool _isGameContinue;

    public UnityEvent gameGoalCallback = new UnityEvent();

    public int StageNumber { get; private set; }
    public bool IsGameSetup { get; set; }
    public bool IsGoal { get; private set; }

    private void Start()
    {
        Debug.Log("GameController start");
    }

    private void Update()
    {
        if (!IsGameSetup) return;
    }

    private void GameReset()
    {
        foreach (Transform t in _stageGridLayoutGroup.transform) Destroy(t.gameObject);
        foreach (Transform t in _lifebarGridLayoutGroup.transform) Destroy(t.gameObject);
        if (_enemyList.Count != 0)
            foreach (var e in _enemyList) Destroy(e.gameObject);
        _blackoutCanvasGroup.alpha = 0f;
        _scale = 1f;
        _enemyList = new List<Enemy>();
        _enemyRandomMoveList = new List<Enemy>();
        _enemyEngageList = new List<Enemy>();
        _mazeStringList = new List<string>();
        _potionPositionList = new List<Position>();
        _enemyPositionList = new List<Position>();
        _lifeImageList = new List<Image>();
        _panelPositionDict = new Dictionary<Position, MazePanelElement>();
        _haveKey = false;
        _isPositionUpdate = false;
        IsGoal = false;
        _isGameContinue = false;
        IsGameSetup = false;
    }

    public IEnumerator GameInit(string playerJobName)
    {
        GameReset();
        yield return new WaitForEndOfFrame();
        SetStageDataFromJson(StageNumber);
        yield return new WaitForEndOfFrame();
        MakeMaze();
        yield return new WaitForEndOfFrame();
        StartCoroutine( EnemySetup(_enemyLevelInformation));
        StartCoroutine( PlayerSetup(playerJobName));
        yield return new WaitForEndOfFrame();
        MakeLifebar();
        IsGameSetup = true;
    }

    private void ChangeStageLayoutSize(Dictionary<string, object> stageDataDict)
    {
        _gridPositionDiff = new Vector3(0, 0, 0);
        var stageSize = System.Convert.ToInt16(stageDataDict["maze_size"]);
        switch (stageSize)
        {
            case 200:
                _scale = 0.75f;
                //_stageGridRect.localScale = new Vector2(_scale, _scale);
                _stageGridLayoutGroup.cellSize = new Vector2(70, 70);
                _gridPositionDiff = new Vector3(-650, 300, 0);
                //_stageGridRect.localPosition = _gridPositionDiff;
                break;
        }
    }

    private Dictionary<string, object> GetJsonDataDict(string filename)
    {
        string jsonDataPath = Application.dataPath + "/Resources/JSON/" + filename;
        var reader = new StreamReader(jsonDataPath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        return MiniJSON.Json.Deserialize(datastr) as Dictionary<string, object>;
    }

    private void SetStageDataFromJson(int stageNumber)
    {
        Dictionary<string, object> jsonDataDict = GetJsonDataDict("maze.json");
        var stageDataJsonList = jsonDataDict["stage_data"] as List<object>;
        var stageDataDict = stageDataJsonList[stageNumber] as Dictionary<string, object>;
        ChangeStageLayoutSize(stageDataDict);
        _enemyLevelInformation = stageDataDict["level"].ToString();
        var mazeDataList = stageDataDict["maze"] as List<object>;
        _rows = mazeDataList.Count;
        for (int i = 0; i < _rows; i++)
        {
            // mazeDataListが１行単位でリストに入っているのか
            var mazeStringData = mazeDataList[i].ToString().Split(',');
            if (i == 0) _columns = mazeStringData.Length;
            for (int j = 0; j < _columns; j++) _mazeStringList.Add(mazeStringData[j]);
        }
    }

    private void MakeMaze()
    {
        _stageGridLayoutGroup.constraintCount = _columns;
        int panelCount = 0;
        Position pos;
        MazePanelElement panel;
        for (int i=0; i<_rows; i++)
        {
            for (int j=0; j<_columns; j++)
            {
                _panelPositionDict.Add(pos = new Position(j, i),
                    panel = Instantiate(_panelElement, _stageGridLayoutGroup.transform));
                panel.SetPanelElement(_mazeStringList[panelCount]);
                panel.SetPosition(pos);
                SetPanelPosition(panelCount, pos);
                panelCount++;
            }
        }
    }

    private void SetPanelPosition(int panelCount, Position position)
    {
        if (_mazeStringList[panelCount] == "s") _startPosition = position;
        else if (_mazeStringList[panelCount] == "t") _goalPosition = position;
        else if (_mazeStringList[panelCount] == "k") _keyPosition = position;
        else if (_mazeStringList[panelCount] == "d") _doorPosition = position;
        else if (_mazeStringList[panelCount] == "p") _potionPositionList.Add(position);
        else if (_mazeStringList[panelCount] == "e") _enemyPositionList.Add(position);
    }

    public IEnumerator EnemySetup(string enemyLevelInfo)
    {
        Enemy enemy;
        foreach (var p in _enemyPositionList)
        {
            _enemyList.Add(enemy = Instantiate(_enemyElement, _gameCanvasRect));
            _enemyRandomMoveList.Add(enemy);
            //enemy.SetLocalScale(new Vector2(_scale, _scale));
            yield return new WaitForEndOfFrame();
            enemy.SetLocalPosition(_panelPositionDict[p].GetLocalPosition());
            enemy.SetPosition(p);
            enemy.Setup(enemyLevelInfo);
        }
    }

    public IEnumerator PlayerSetup(string playerJobName)
    {
        _player.Setup(playerJobName);
        //_player.SetLocalScale(new Vector2(_scale, _scale));
        yield return new WaitForEndOfFrame();
        _player.SetLocalPosition(_panelPositionDict[_startPosition].GetLocalPosition());
        _playerPosition = _startPosition;
    }

    public void Move()
    {
        if (!IsGameSetup) return;
        if (_player.IsDead) return;
        if (_player.IsAttacking) return;
        if (_player.IsMoving) return;

        int x = _playerPosition.X, y = _playerPosition.Y;
        Position nextPosition = new Position(x, y);

        if (Input.GetKeyDown(KeyCode.W)) nextPosition = new Position(x, y - 1);
        else if (Input.GetKeyDown(KeyCode.A)) nextPosition = new Position(x - 1, y);
        else if (Input.GetKeyDown(KeyCode.D)) nextPosition = new Position(x + 1, y);
        else if (Input.GetKeyDown(KeyCode.S)) nextPosition = new Position(x, y + 1);

        if (nextPosition.Equals(_playerPosition)) return;
        
        if (CheckCanMove(nextPosition))
            StartCoroutine(MovePlayer(nextPosition));
        if (CheckWallPanel(nextPosition))
            StartCoroutine(_player.ShakeMotion());
        if (CheckEnemyPanel(nextPosition))
        {
            foreach(var e in _enemyList)
            {
                if (e.Position.Equals(nextPosition))
                    StartCoroutine(Battle(_player, e));
            }
        }
    }

    private IEnumerator MovePlayer(Position nextPosition)
    {
        _isPositionUpdate = true;
        CheckGoalPoint(nextPosition);
        CheckKey(nextPosition);
        CheckPotion(nextPosition);

        if (_isPositionUpdate)
        {
            _panelPositionDict[_playerPosition].UpdatePanelStatus(MazePanelElement.PanelType.Background);
            _playerPosition = nextPosition;
            _panelPositionDict[_playerPosition].UpdatePanelStatus(MazePanelElement.PanelType.Player);
            yield return _player.Move(_panelPositionDict[nextPosition].GetLocalPosition());
            yield return new WaitForEndOfFrame();

            if (_enemyRandomMoveList.Count != 0)
            {
                RandomMoveEnemy();
                yield return new WaitForEndOfFrame();
            }
            if (_enemyEngageList.Count != 0)
            {
                FollowingMoveEnemy();
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void RandomMoveEnemy()
    {
        var tempList = new List<Enemy>(_enemyRandomMoveList);
        foreach (var enemy in tempList)
        {
            if (IsNearCreature(enemy.Position, _playerPosition, enemy.Visibility))
            {
                enemy.IsFindPlayer = true;
                _enemyRandomMoveList.Remove(enemy);
                _enemyEngageList.Add(enemy);
                Debug.Log("near add");
            }
            else // enemy random move
            {
                var panelList = GetMovablePanelList(enemy.Position);
                //Debug.Log("can move panel: "+panelList.Count);
                if (panelList.Count == 0) continue;
                int n = Random.Range(0, panelList.Count);
                _panelPositionDict[enemy.Position].UpdatePanelStatus(MazePanelElement.PanelType.Background);
                panelList[n].UpdatePanelStatus(MazePanelElement.PanelType.Enemy);
                enemy.SetPosition(panelList[n].Position);
                StartCoroutine(enemy.Move(panelList[n].GetLocalPosition()));
            }
        }
    }

    public void FollowingMoveEnemy()
    {
        var tempList = new List<Enemy>(_enemyEngageList);
        foreach (var e in tempList)
        {
            // battle
            if (IsNearCreature(e.Position, _playerPosition, 1))
            {
                StartCoroutine(Battle(_player, e));
            }
            else // follow move
            {
                Debug.Log("no attack");
                var nextPosition = new Position(e.Position.X + _dirX, e.Position.Y + _dirY);
                MazePanelElement panel = _panelPositionDict[nextPosition];
                panel.UpdatePanelStatus(MazePanelElement.PanelType.Enemy);
                _panelPositionDict[e.Position].UpdatePanelStatus(MazePanelElement.PanelType.Background);
                e.SetPosition(panel.Position);
                if (!IsNearCreature(e.Position, _playerPosition, e.Visibility))
                {
                    Debug.Log("no near");
                    _enemyEngageList.Remove(e);
                    _enemyRandomMoveList.Add(e);
                }
                StartCoroutine(e.Move(panel.GetLocalPosition()));
            }
        }
    }

    public IEnumerator Battle(Player p, Enemy e)
    {
        Debug.Log("attack!");
        p.Attack(e);
        e.Attack(p);
        yield return new WaitForEndOfFrame();
        PlayerDamageAnimation();
        if (e.IsDead)
            yield return DeadEnemy(e);

    }

    private bool IsDeadEnemy()
    {
        foreach (var e in _enemyList)
        {
            if (e.IsDead)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator DeadEnemy(Enemy e)
    {
        Debug.Log("DeadEnemy!");
        if (_enemyList.Contains(e)) _enemyList.Remove(e);
        if (_enemyRandomMoveList.Contains(e)) _enemyRandomMoveList.Remove(e);
        if (_enemyEngageList.Contains(e)) _enemyEngageList.Remove(e);
        _panelPositionDict[e.Position].UpdatePanelStatus(MazePanelElement.PanelType.Background);
        yield return new WaitForEndOfFrame();
        Destroy(e.gameObject);
    }

    public int GetRandomMoveEnemyCount()
    {
        return _enemyList.Count;
    }

    public int GetEngageEnemyCount()
    {
        return _enemyEngageList.Count;
    }

    private bool CheckCanMove(Position nextPosition)
    {
        return _panelPositionDict.ContainsKey(nextPosition) && !CheckWallPanel(nextPosition)
            && !CheckPlayerPanel(nextPosition) && !CheckEnemyPanel(nextPosition);
    }

    private void CheckGoalPoint(Position nextPosition)
    {
        bool isGoalPanel = _panelPositionDict[nextPosition].PanelStatus == MazePanelElement.PanelType.GoalPoint;
        if (isGoalPanel && _haveKey)
            Goal();
        if (isGoalPanel && !_haveKey)
        {
            _isPositionUpdate = false;
            StartCoroutine(_player.ShakeMotion());
        }
    }

    private void CheckPotion(Position nextPosition)
    {
        if (CheckPotionPanel(nextPosition)) 
        {
            _player.GetPotion();
            _panelPositionDict[nextPosition].GetPotion();
            for (int i = 0; i < _player.Life; i++)
                _lifeImageList[i].color = Color.white;
            _isPositionUpdate = true;
        }
    }

    private void CheckKey(Position nextPosition)
    {
        if (CheckKeyPanel(nextPosition))
        {
            _player.GetKey();
            _panelPositionDict[nextPosition].GetKey();
            _haveKey = true;
            _isPositionUpdate = true;
        }
    }

    private void Goal()
    {
        Debug.Log("Goal");
        IsGoal = true;
        StageNumber += 1;
        //StartCoroutine(BlackOutStage());
        IsGoal = false;
        gameGoalCallback.Invoke();
    }

    private IEnumerator BlackOutStage()
    {
        yield return _blackoutCanvasGroup.DOFade(1f, 3f).SetEase(Ease.Linear).WaitForCompletion();
    }

    public bool IsPlayerDead()
    {
        return _player.IsDead;
    }
    private bool CheckPlayerPanel(Position nextPosition)
    {
        return _panelPositionDict[nextPosition].PanelStatus == MazePanelElement.PanelType.Player;
    }

    private bool CheckEnemyPanel(Position nextPosition)
    {
        return _panelPositionDict[nextPosition].PanelStatus == MazePanelElement.PanelType.Enemy;
    }

    private bool CheckWallPanel(Position nextPosition)
    {
        return _panelPositionDict[nextPosition].PanelStatus == MazePanelElement.PanelType.Wall;
    }

    private bool CheckKeyPanel(Position nextPosition)
    {
        return _panelPositionDict[nextPosition].PanelStatus == MazePanelElement.PanelType.Key;
    }

    private bool CheckPotionPanel(Position nextPosition)
    {
        return _panelPositionDict[nextPosition].PanelStatus == MazePanelElement.PanelType.Potion;
    }

    private bool CheckBrankPanel(Position nextPosition)
    {
        return _panelPositionDict[nextPosition].PanelStatus == MazePanelElement.PanelType.Background;
    }

    private bool IsNearCreature(Position myPos, Position enemyPos, int visibility)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                if (CheckDirectionPanel(myPos, enemyPos, i, j, visibility))
                {
                    _dirX = i;
                    _dirY = j;
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckDirectionPanel(Position myPos, Position enemyPos ,int dirX, int dirY, int length)
    {
        // dirX = 1 or 0 or -1, dirY = 1 or 0 or -1
        int px = enemyPos.X, py = enemyPos.Y;
        int x = myPos.X, y = myPos.Y;
        var isNear = false;
        for (int i = 1; i <= length; i++)
        {
            var p = new Position(x + i * dirX, y + i * dirY);
            if (CheckWallPanel(p)) return false;
            isNear = (px == x + i * dirX) && (py == y + i * dirY);
            if (isNear) return isNear;
        }
        return isNear;
    }

    private List<MazePanelElement> GetMovablePanelList(Position currentPosition ,int range=1)
    {
        int x = currentPosition.X, y = currentPosition.Y;
        var panelList = new List<MazePanelElement>();
        Position position;
        for (int i = 1; i <= range; i++)
        {
            //left
            position = new Position(x - i, y);
            if (CheckBrankPanel(position))
                panelList.Add(_panelPositionDict[position]);
            // left top
            //position = new Position(x - i, y - i);
            //if (CheckBrankPanel(position))
            //    panelList.Add(_panelPositionDict[position]);
            // top
            position = new Position(x, y - i);
            if (CheckBrankPanel(position))
                panelList.Add(_panelPositionDict[position]);
            // top right
            //position = new Position(x + i, y - i);
            //if (CheckBrankPanel(position))
            //    panelList.Add(_panelPositionDict[position]);
            // right
            position = new Position(x + i, y);
            if (CheckBrankPanel(position))
                panelList.Add(_panelPositionDict[position]);
            // right bottom
            //position = new Position(x + i, y + i);
            //if (CheckBrankPanel(position))
            //    panelList.Add(_panelPositionDict[position]);
            // bottom
            position = new Position(x, y + i);
            if (CheckBrankPanel(position))
                panelList.Add(_panelPositionDict[position]);
            // bottom left
            //position = new Position(x - i, y + i);
            //if (CheckBrankPanel(position))
            //    panelList.Add(_panelPositionDict[position]);
        }
        return panelList;
    }

    private void MakeLifebar()
    {
        _lifebarGridLayoutGroup.constraintCount = _player.Life;
        _lifebarGridLayoutGroup.cellSize = new Vector2(50, 50);
        for (int i = 0; i < _player.Life; i++)
        {
            _lifeImageList.Add(Instantiate(_lifeElement, _lifebarGridLayoutGroup.transform));

        }
        Debug.Log("lifeCount: " + _lifeImageList.Count);
    }

    private void PlayerDamageAnimation()
    {
        int n = _lifeImageList.Count - _player.Life;
        for (int i = 1; i <= n; i++)
        {
            _lifeImageList[_lifeImageList.Count - i].color = Color.black;
        }
    }

    public void SetStageNumber(int n)
    {
        StageNumber = n;
    }
}
