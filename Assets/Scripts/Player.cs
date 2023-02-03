using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public class OriginalStatus
    {
        public int Life { get; private set; }
        public int Mana { get; private set; }
        public int Power { get; private set; }
        public int Magic { get; private set; }
        public float Speed { get; private set; }
        public OriginalStatus(int life, int mana, int power, int magic, float speed)
        {
            Life = life;
            Mana = mana;
            Power = power;
            Magic = magic;
            Speed = speed;
        }
    }

    [SerializeField]
    private Image _soldierImage;
    [SerializeField]
    private Image _mageImage;
    [SerializeField]
    private Image _thiefImage;
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private RectTransform _weaponRect;

    private float _moveTime = 0.2f;
    private OriginalStatus status;

    public bool IsDead { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsMoving { get; private set; }
    public int Life { get; private set; }
    public int Mana { get; private set; }
    public int Power { get; private set; }
    public int Magic { get; private set; }
    public float Speed { get; private set; }

    public enum PlayerJob
    {
        Soldier,
        Mage,
        Thief
    }
    public PlayerJob playerStatus;

    public void Setup(string playerJobName)
    {
        IsMoving = false;
        IsAttacking = false;
        IsDead = false;
        switch(playerJobName)
        {
            case "Soldier":
                playerStatus = PlayerJob.Soldier;
                _soldierImage.gameObject.SetActive(true);
                Life = 6;
                Mana = 0;
                Power = 1;
                Magic = 0;
                Speed = 1.5f;
                status = new OriginalStatus(Life, Mana, Power, Magic, Speed);
                break;
            case "Mage":
                //playerStatus = PlayerType.Mage;
                playerStatus = PlayerJob.Soldier;
                _mageImage.gameObject.SetActive(true);
                Life = 4;
                Mana = 10;
                Power = 0;
                Magic = 3;
                Speed = 1;
                status = new OriginalStatus(Life, Mana, Power, Magic, Speed);
                break;
            case "Thief":
                //playerStatus = PlayerType.Thief;
                playerStatus = PlayerJob.Soldier;
                _thiefImage.gameObject.SetActive(true);
                Life = 3;
                Mana = 5;
                Power = 1;
                Magic = 1;
                Speed = 1;
                status = new OriginalStatus(Life, Mana, Power, Magic, Speed);
                break;
        }
    }

    public IEnumerator Move(Vector2 nextLocalPosition)
    {
        IsMoving = true;
        yield return _rectTransform.DOAnchorPos(nextLocalPosition, _moveTime).SetEase(Ease.Linear).WaitForCompletion();
        IsMoving = false;
    }

    public void Attack(Enemy enemy)
    {
        IsAttacking = true;
        int damage = Power <= Magic ? Magic : Power;
        StartCoroutine(AttackMotion());
        StartCoroutine(enemy.GetDamage(damage));
        if (enemy.IsDead) Destroy(enemy);
        IsAttacking = false;
    }

    private IEnumerator AttackMotion()
    {
        yield return _weaponRect.DOLocalRotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic);
        yield return new WaitForSecondsRealtime(Speed);
    }

    public void DeadEnemy(Enemy enemy)
    {

    }

    public IEnumerator ShakeMotion()
    {
        Debug.Log("shake player");
        yield return _rectTransform.DOShakeAnchorPos(0.5f, new Vector2(5f, 0f), 10).WaitForCompletion();
    }

    public void SetLocalPosition(Vector2 pos)
    {
        _rectTransform.localPosition = pos;
    }

    public RectTransform GetPlayerRectTransform()
    {
        return _rectTransform;
    }

    public void GetPotion()
    {
        Life = status.Life;
        Mana = status.Mana;
    }

    public void GetKey()
    {
        //TODO GetKeyMotion
    }

    public IEnumerator GetDamage(int damage)
    {
        Life = Life - damage;
        yield return _soldierImage.DOColor(new Color(1f, 0, 0), 0.5f).SetEase(Ease.Linear);
        yield return _soldierImage.DOColor(new Color(0, 0, 0), 0.5f).SetEase(Ease.Linear);
        if (Life < 0) IsDead = true;
    }
}
