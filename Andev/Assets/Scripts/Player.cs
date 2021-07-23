using UnityEngine;
using System;

using PlayerSelectType = GameDefine.PlayerSelectType;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public enum StateType
    {
        None,
        Waiting,
        Select1,
        Select2,
        Select3
    }

    private static readonly StateWaiting stateWaiting = new StateWaiting();
    private static readonly StateSelect1 stateSelect1 = new StateSelect1();
    private static readonly StateSelect2 stateSelect2 = new StateSelect2();
    private static readonly StateSelect3 stateSelect3 = new StateSelect3();
    
    /// <summary>所持金</summary>
    [SerializeField]
    private Money _haveMoney;

    /// <summary>プレイヤーID</summary>
    public int PlayerId => _playerId;
    private int _playerId;

    /// <summary>キャラ名</summary>
    public string PlayerName => _name;
    private string _name;

    /// <summary>選択肢のコールバック</summary>
    private Action<PlayerSelectType, Player> _selectCallBack;

    /// <summary>現在のState</summary>
    public PlayerStateBase CurrentState => _currentState;
    private PlayerStateBase _currentState = stateWaiting;

    public StateType CurrentStateEnum => _currentStateEnum;
    private StateType _currentStateEnum;

    /// <summary>現在のState名</summary>
    public string CurrentStateName => _currentStateEnum.ToString();

    /// <summary>
    /// セットアップ
    /// </summary>
    public void Setup(
        int id,
        string name,
        int initMoneyCount,
        bool isFirst,
        Action<PlayerSelectType, Player> SelectCallBack)
    {
        _playerId = id;
        _name = name;
        _haveMoney.Init(initMoneyCount);
        _selectCallBack = SelectCallBack;

        if (isFirst)
        {
            ChangeState(StateType.Select1);
        }
        else
        {
            ChangeState(StateType.Waiting);
        }
    }

    void Update()
    {
        _currentState.OnUpdate(this);
    }

    /// <summary>
    /// enumからstateクラスを取得
    /// </summary>
    private PlayerStateBase GetState(StateType stateEnum)
    {
        switch (stateEnum)
        {
            case StateType.Waiting:
                return stateWaiting;

            case StateType.Select1:
                return stateSelect1;

            case StateType.Select2:
                return stateSelect2;

            case StateType.Select3:
                return stateSelect3;

            default:
                return null;
        }
    }


    public void ChangeState(StateType nextStateEnum)
    {
        PlayerStateBase nextState = GetState(nextStateEnum);
        _currentState.OnExit(this, nextState);
        _currentState = nextState;
        _currentStateEnum = nextStateEnum;
        _currentState.OnEnter(this, _currentState);
    }

    /// <summary>
    /// 選択肢ダイアログの表示
    /// </summary>
    private void ShowSelectDialog(List<PlayerSelectType> selectList)
    {
        List<UIManager.ButtonRegisterData> buttonData = new List<UIManager.ButtonRegisterData>();

        foreach(var select in selectList)
        {
            buttonData.Add(new UIManager.ButtonRegisterData
            {
                ButtonText = Wording.LoadWord(select.ToString()),
                ButtonCallback = () => Select(select)
            });
        }

        UIManager.Instance.ShowSelectDialog(buttonData);
    }

    private void Select(PlayerSelectType select)
    {
        _selectCallBack.Invoke(select, this);
        ChangeState(StateType.Waiting);
    }

    /// <summary>
    /// 指定した量を支払えるか
    /// </summary>
    public bool IsPayable(int payCount)
    {
        return _haveMoney.IsPayable(payCount);
    }

    /// <summary>
    /// 掛け金を支払う
    /// </summary>
    public int PayMoney(int count)
    {
        return _haveMoney.PayMoney(count);
    }

    public class StateWaiting : PlayerStateBase
    {
        // 何もしない
    }

    /// <summary>
    /// 第1選択（ベット/パス）
    /// </summary>
    public class StateSelect1 : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            base.OnEnter(owner, prevState);
            Debug.Log(owner.PlayerName + "選択: ベットB or パスP", Debug.Green);

            List<PlayerSelectType> selectList = new List<PlayerSelectType>
            {
                PlayerSelectType.Bet,
                PlayerSelectType.Pass
            };
            owner.ShowSelectDialog(selectList);
        }
    }

    /// <summary>
    /// 第2選択（コール/フォールド/レイズ）
    /// </summary>
    public class StateSelect2 : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            base.OnEnter(owner, prevState);
            Debug.Log(owner.PlayerName + "選択: コールC or フォールドF or レイズR", Debug.Green);

            List<PlayerSelectType> selectList = new List<PlayerSelectType>
            {
                PlayerSelectType.Call,
                PlayerSelectType.Fold,
                PlayerSelectType.Raise
            };
            owner.ShowSelectDialog(selectList);
        }
    }

    /// <summary>
    /// 第3選択（コール/フォールド）
    /// </summary>
    public class StateSelect3 : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            base.OnEnter(owner, prevState);
            Debug.Log(owner.PlayerName + "選択: コールC or フォールドF", Debug.Green);

            List<PlayerSelectType> selectList = new List<PlayerSelectType>
            {
                PlayerSelectType.Call,
                PlayerSelectType.Fold,
            };
            owner.ShowSelectDialog(selectList);
        }
    }
}
