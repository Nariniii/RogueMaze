using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Position
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class MazePanelElement : MonoBehaviour
{
    [SerializeField]
    private GameObject _backPanelObject;
    [SerializeField]
    private GameObject _goalPanelObject;
    [SerializeField]
    private GameObject _wallPanelObject;
    [SerializeField]
    private GameObject _doorPanelObject;
    [SerializeField]
    private GameObject _potionPanelObject;
    [SerializeField]
    private GameObject _goblinPanelObject;
    [SerializeField]
    private GameObject _soldierPanelObject;
    [SerializeField]
    private GameObject _magePanelObject;
    [SerializeField]
    private GameObject _thiefPanelObject;
    [SerializeField]
    private GameObject _keyPanelObject;
    [SerializeField]
    private RectTransform _rectTransform;

    private Enemy enemy;

    public Position Position { get; private set; }
    public enum PanelType
    {
        Background,
        StartPoint,
        GoalPoint,
        Wall,
        Door,
        Potion,
        Enemy,
        Player,
        Key
    }

    public PanelType PanelStatus;

    public void SetPanelElement(string splitJsonString)
    {
        PanelStatus = PanelType.Background;
        _backPanelObject.SetActive(true);
        switch(splitJsonString)
        {
            case "s":
                PanelStatus = PanelType.StartPoint;
                //_soldierPanelObject.SetActive(true);
                break;
            case "t":
                PanelStatus = PanelType.GoalPoint;
                _goalPanelObject.SetActive(true);
                break;
            case "p":
                PanelStatus = PanelType.Potion;
                _potionPanelObject.SetActive(true);
                break;
            case "e":
                PanelStatus = PanelType.Enemy;
                //_goblinPanelObject.SetActive(true);
                //enemy = new Enemy("easy");
                //enemy.SetEnemyInformation();
                break;
            case "k":
                PanelStatus = PanelType.Key;
                _keyPanelObject.SetActive(true);
                break;
            case "w":
                PanelStatus = PanelType.Wall;
                _wallPanelObject.SetActive(true);
                break;
        }
    }

    public void SetPosition(Position position)
    {
        Position = position;
    }

    public void GetKey()
    {
        _keyPanelObject.SetActive(false);
        PanelStatus = PanelType.Background;
    }

    public void GetPotion()
    {
        _potionPanelObject.SetActive(false);
        PanelStatus = PanelType.Background;
    }

    public void GetGoal()
    {
        _goalPanelObject.SetActive(false);
        PanelStatus = PanelType.Background;
    }

    public Vector3 GetLocalPosition()
    {
        return _rectTransform.localPosition;
    }

    public string GetEnemyLevelInformation(string jsonString)
    {
        return jsonString;
    }

    public void UpdatePanelStatus(PanelType type)
    {
        PanelStatus = type;
    }
}
