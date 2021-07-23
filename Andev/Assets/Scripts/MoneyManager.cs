using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public enum MoneyType
    {
        None,
        /// <summary>左プレイヤー所持金</summary>
        Left,
        /// <summary>右プレイヤー所持金</summary>
        Right,
        /// <summary>左プレイヤーの場に出した掛け金</summary>
        FieldLeft,
        /// <summary>右プレイヤーの場に出した掛け金</summary>
        FieldRight
    }

    [Serializable]
    public class MoneyTypePair
    {
        public MoneyType MoneyType;
        public Text MoneyText;
    }

    [SerializeField]
    private List<MoneyTypePair> _moneyUIList;

    public Money LeftMoney => GetMoney(MoneyType.Left);
    public Money RightMoney => GetMoney(MoneyType.Right);
    public Money FieldRightMoney => GetMoney(MoneyType.FieldRight);
    public Money FieldLeftMoney => GetMoney(MoneyType.FieldLeft);

    /// <summary>コールに必要な数</summary>
    public int ShouldCallCount => Math.Abs(FieldLeftMoney.Count - FieldRightMoney.Count);

    private Dictionary<MoneyType, Money> _moneyMap;

    public void Init(int playerInitializeCount, int fieldInitializeCount)
    {
        _moneyMap = new Dictionary<MoneyType, Money>();
        
        _moneyMap.Add(MoneyType.Left, new Money(playerInitializeCount, UpdateUI));
        _moneyMap.Add(MoneyType.Right, new Money(playerInitializeCount, UpdateUI));
        _moneyMap.Add(MoneyType.FieldLeft, new Money(fieldInitializeCount, UpdateUI));
        _moneyMap.Add(MoneyType.FieldRight, new Money(fieldInitializeCount, UpdateUI));

        UpdateUI();
    }

    public Money GetMoney(MoneyType moneyType)
    {
        if (!_moneyMap.ContainsKey(moneyType))
        {
            Debug.LogError("GetMoneyCount > ない");
            return null;
        }

        return _moneyMap[moneyType];
    }

    public Money GetMoney(GameDefine.PositionType position)
    {
        // 決め
        switch (position)
        {
            case GameDefine.PositionType.Right:
                return RightMoney;
            case GameDefine.PositionType.Left:
                return LeftMoney;
            default:
                Debug.LogError($"GetMoney > 想定外:{position}");
                return null;
        }
    }

    /// <summary>
    /// 参加料の支払い手続き
    /// </summary>
    public void PayEntryCharge(int entryCharge)
    {
        FieldLeftMoney.AddMoney(LeftMoney.PayMoney(entryCharge));
        FieldRightMoney.AddMoney(RightMoney.PayMoney(entryCharge));
    }

    /// <summary>
    /// プレイヤーの所持金を場に出す
    /// </summary>
    public void PayPlayerMoneyToField(GameDefine.PositionType position, int payCount)
    {
        switch (position)
        {
            case GameDefine.PositionType.Right:
                FieldRightMoney.AddMoney(RightMoney.PayMoney(payCount));
                break;

            case GameDefine.PositionType.Left:
                FieldLeftMoney.AddMoney(LeftMoney.PayMoney(payCount));
                break;

            default:
                Debug.LogError("PayPlayerMoneyToField > 想定外");
                break;
        }
    }

    /// <summary>
    /// 両プレイヤーが指定金額を払えるか
    /// </summary>
    public bool IsPayableBothPlayer(int count)
    {
        return LeftMoney.IsPayable(count) && RightMoney.IsPayable(count);
    }

    private void UpdateUI()
    {
        foreach(MoneyTypePair moneyText in _moneyUIList)
        {
            moneyText.MoneyText.text = GetMoney(moneyText.MoneyType).Count.ToString();
        }
    }
}
