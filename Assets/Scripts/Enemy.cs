using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Image _goblinImage;
    [SerializeField]
    private RectTransform _rectTransform;

    private float _moveTime = 0.2f;

    public int Life { get; private set; }
    public int Power { get; private set; }
    public float Speed { get; private set; }
    public int Visibility { get; private set; }
    public Position Position { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsFindPlayer { get; set; }
    public enum EnemyType
    {
        Goblin,
        Skelton,
        Ghost,
        Doragon
    }
    public EnemyType EnemyStatus { get; private set; }

    public void Setup(string levelInfo)
    {
        IsMoving = false;
        IsAttacking = false;
        IsDead = false;
        IsFindPlayer = false;
        switch (levelInfo)
        {
            case "easy":
                EnemyStatus = EnemyType.Goblin;
                _goblinImage.gameObject.SetActive(true);
                Life = 1;
                Power = 1;
                Speed = 2;
                Visibility = 2;
                break;
            case "normal":
                int n = Random.Range(0, 2);
                if (n == 0)
                {
                    EnemyStatus = EnemyType.Ghost;
                }
                else
                {
                    EnemyStatus = EnemyType.Skelton;
                }
                break;
            case "hard":
                EnemyStatus = EnemyType.Doragon;
                break;
        }
    }

    public void SetPosition(Position position)
    {
        Position = position;
    }

    public void SetLocalPosition(Vector2 pos)
    {
        _rectTransform.anchoredPosition = pos;
    }

    public IEnumerator Move(Vector2 nextLocalPosition)
    {
        IsMoving = true;
        yield return _rectTransform.DOAnchorPos(nextLocalPosition, _moveTime).SetEase(Ease.Linear).WaitForCompletion();
        IsMoving = false;
    }

    public void Attack(Player player)
    {
        IsAttacking = true;
        StartCoroutine(player.GetDamage(Power));
        StartCoroutine(ShakeMotion());
        IsAttacking = false;
    }

    public IEnumerator GetDamage(int damage)
    {
        Life -= damage;
        yield return _goblinImage.DOColor(new Color(1f, 0, 0), 0.5f).SetEase(Ease.Linear).WaitForCompletion();
        yield return _goblinImage.DOColor(new Color(0, 0, 0), 0.5f).SetEase(Ease.Linear).WaitForCompletion();
        if (Life <= 0) IsDead = true;
    }

    public IEnumerator ShakeMotion()
    {
        yield return _rectTransform.DOShakeAnchorPos(0.5f, new Vector2(5f, 0f), 10).WaitForCompletion();
        yield return new WaitForSecondsRealtime(Speed);
    }
}
