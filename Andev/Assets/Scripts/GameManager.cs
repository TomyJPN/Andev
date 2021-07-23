using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameStatus = GameStateUtility.GameStatus;
using AfterPlayerActionType = GameStateUtility.AfterPlayerActionType;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Player _myPlayer;

    [SerializeField]
    private Player _enemyPlayer;

    [SerializeField]
    private MoneyManager _moneyManager;

    private GameStatus _currentGameStatus;
    private AfterPlayerActionType _afterPlayerAction;
    private int _currentWaitingPlayerId;

    /// <summary>場に出ている掛け金</summary>
    public int FieldMoney => _firldMoney;
    private int _firldMoney;

    Coroutine _gameCoroutine;

    /// <summary>パスされた回数</summary>
    private int _passCount;


    void Start()
    {
        _gameCoroutine = StartCoroutine(GameCoroutine());
    }

    public void Restart()
    {
        StopCoroutine(_gameCoroutine);
        Start();
    }

    /// <summary>
    /// ゲームループ
    /// </summary>
    private IEnumerator GameCoroutine()
    {
        InitGame();
        _currentGameStatus = GameStatus.InProgress;

        yield return new WaitForSeconds(2f);

        _myPlayer.Setup(1, "悪魔", true, GameDefine.PositionType.Left, PlayerSelectCallBack);
        _enemyPlayer.Setup(2, "天使", false, GameDefine.PositionType.Right, PlayerSelectCallBack);

        PayEntryCharge();

        // とりあえず先攻
        _currentWaitingPlayerId = 1;

        _afterPlayerAction = AfterPlayerActionType.PlayerSelect;

        // 互いの選択が続く
        _currentGameStatus = GameStatus.WaitingPlayerSelect;
        yield return new WaitUntil(() => _afterPlayerAction != AfterPlayerActionType.PlayerSelect);
    
        Debug.Log($"{_afterPlayerAction}演出を行って判定します！", Debug.Green);
    }

    private void InitGame()
    {
        _passCount = 0;
        _moneyManager.Init(10, 0);
    }

    /// <summary>
    /// 参加料の支払い
    /// </summary>
    private void PayEntryCharge()
    {
        // 参加料
        const int EntryCharge = 1;

        if(!_moneyManager.IsPayableBothPlayer(EntryCharge))
        {
            Debug.LogError("支払い不能プレイヤーがいます");
            return;
        }

        _moneyManager.PayEntryCharge(EntryCharge);
    }


    /// <summary>
    /// プレイヤーの取得
    /// </summary>
    private Player GetPlayer(int playerId)
    {
        switch (playerId)
        {
            case 1:
                return _myPlayer;
            case 2:
                return _enemyPlayer;
            default:
                Debug.LogError($"GetPlayer > いません:{playerId}");
                return null;
        }
    }

    /// <summary>
    /// 指定プレイヤーの相手の取得
    /// </summary>
    private Player GetOpponentPlayer(int playerId)
    {
        switch (playerId)
        {
            case 1:
                return GetPlayer(2);
            case 2:
                return GetPlayer(1);
            default:
                Debug.LogError($"GetOpponentPlayer > いません:{playerId}");
                return null;
        }
    }

    /// <summary>
    /// プレイヤーの選択コールバック
    /// </summary>
    private void PlayerSelectCallBack(GameDefine.PlayerSelectType select, Player player, int selectCount)
    {
        if (_currentGameStatus != GameStatus.WaitingPlayerSelect ||
           _currentWaitingPlayerId != player.PlayerId)
        {
            Debug.LogError($"PlayerSelectCallBack > 入力待ち状態ではありません 選択プレイヤー:{player.PlayerName}, 待ちプレイヤー:{GetPlayer(_currentWaitingPlayerId).PlayerName}");
            return;
        }

        _currentGameStatus = GameStatus.InProgress;

        if(select == GameDefine.PlayerSelectType.Pass)
        {
            _passCount++;
            UnityEngine.Debug.Log($"{_passCount}に");
        }

        if(select == GameDefine.PlayerSelectType.Bet ||
            select == GameDefine.PlayerSelectType.Raise)
        {
            Debug.Log($"数値:{selectCount}を選択");
        }

        PayMoney(select, player, selectCount);

        StartCoroutine(ChangeTurn(select, player));
    }

    /// <summary>
    /// ターンの切り替え
    /// </summary>
    private IEnumerator ChangeTurn(GameDefine.PlayerSelectType select, Player player)
    {
        Player opponentPlayer = GetOpponentPlayer(player.PlayerId);
        Player.StateType nextState = GameStateUtility.GetOpponentPlayerNextState(player.CurrentStateEnum, select, _passCount);

        yield return new WaitForSeconds(1f);

        // 相手の設定
        opponentPlayer.ChangeState(nextState);

        _afterPlayerAction = GameStateUtility.GetAfterPlayerAction(player.CurrentStateEnum, select, _passCount);

        if (_afterPlayerAction == AfterPlayerActionType.PlayerSelect)
        {
            _currentWaitingPlayerId = opponentPlayer.PlayerId;
            _currentGameStatus = GameStatus.WaitingPlayerSelect;
        }
    }

    /// <summary>
    /// 支払い
    /// </summary>
    private void PayMoney(GameDefine.PlayerSelectType select, Player player, int selectCount)
    {
        int payCount = 0;

        switch (select)
        {
            case GameDefine.PlayerSelectType.Bet:
            case GameDefine.PlayerSelectType.Raise:
                payCount = selectCount;

                break;
            case GameDefine.PlayerSelectType.Call:
                payCount = _moneyManager.ShouldCallCount;
                break;
        }

        _moneyManager.PayPlayerMoneyToField(player.Position, payCount);
    }

}
