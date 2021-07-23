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
    public int ShouldCallCount => _differenceFieldCount;
    private int _differenceFieldCount => Math.Abs(FieldLeftMoney.Count - FieldRightMoney.Count);

    private Dictionary<MoneyType, Money> _moneyMap;

    private Money[,] _moneyStruct;

    public void Init(int playerInitializeCount, int fieldInitializeCount)
    {
        _moneyMap = new Dictionary<MoneyType, Money>();
        
        _moneyMap.Add(MoneyType.Left, new Money(playerInitializeCount, UpdateUI));
        _moneyMap.Add(MoneyType.Right, new Money(playerInitializeCount, UpdateUI));
        _moneyMap.Add(MoneyType.FieldLeft, new Money(fieldInitializeCount, UpdateUI));
        _moneyMap.Add(MoneyType.FieldRight, new Money(fieldInitializeCount, UpdateUI));

        _moneyStruct = new Money[2, 2] { { LeftMoney, FieldLeftMoney }, { RightMoney, FieldRightMoney } };

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

    public Money GetOpponentPlayerMoney(GameDefine.PositionType position)
    {
        return GetMoney(GameDefine.GetOpponentPosition(position));
    }

    public Money GetFieldMoney(GameDefine.PositionType position)
    {
        switch (position)
        {
            case GameDefine.PositionType.Right:
                return FieldRightMoney;
            case GameDefine.PositionType.Left:
                return FieldLeftMoney;
            default:
                Debug.LogError($"GetFieldMoney > 想定外:{position}");
                return null;
        }
    }

    public Money GetOpponentFieldMoney(GameDefine.PositionType position)
    {
        return GetFieldMoney(GameDefine.GetOpponentPosition(position));
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

    /// <summary>
    /// ベット可能か
    /// </summary>
    public bool IsPossibleBet(GameDefine.PositionType position)
    {
        return GetOpponentPlayerMoney(position).Count != 0;
    }

    /// <summary>
    /// ベットできる最大金額の取得
    /// </summary>
    public int GetMaxBetCount(GameDefine.PositionType position)
    {
        int result = GetMoney(position).Count;
        int opponentCount = GetOpponentPlayerMoney(position).Count;

        // note:相手がコールできる量(相手の所持数)まで
        if (result > opponentCount)
        {
            result = opponentCount;
        }

        return result;
    }

    /// <summary>
    /// レイズ可能か
    /// </summary>
    public bool IsPossibleRaise(GameDefine.PositionType position)
    {
        // 自分が出せるかチェック
        if (!GetMoney(position).IsPayable(_differenceFieldCount))
        {
            return false;
        }

        return GetOpponentPlayerMoney(position).Count != 0;
    }

    /// <summary>
    /// レイズする場合の最大金額の取得
    /// </summary>
    public int GetMaxRaiseCount(GameDefine.PositionType position)
    {
        if (!IsPossibleRaise(position))
        {
            Debug.LogError("GetMaxRaiseCount > レイズできません");
            return 0;
        }

        int opponentSum = GetOpponentPlayerMoney(position).Count + GetOpponentFieldMoney(position).Count;
        return opponentSum - GetFieldMoney(position).Count;
    }

    /// <summary>
    /// レイズする場合の最低金額の取得
    /// </summary>
    public int GetMinRaiseCount(GameDefine.PositionType position)
    {
        if (!IsPossibleRaise(position))
        {
            Debug.LogError("GetMaxRaiseCount > レイズできません");
            return 0;
        }

        return _differenceFieldCount + 1;
    }

    private void UpdateUI()
    {
        foreach(MoneyTypePair moneyText in _moneyUIList)
        {
            moneyText.MoneyText.text = GetMoney(moneyText.MoneyType).Count.ToString();
        }
    }
}
