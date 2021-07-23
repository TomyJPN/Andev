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
    private Money _myFieldMoney;

    [SerializeField]
    private Money _enemyFieldMoney;

    private GameStatus _currentGameStatus;
    private AfterPlayerActionType _afterPlayerAction;
    private int _currentWaitingPlayerId;

    /// <summary>場に出ている掛け金</summary>
    public int FieldMoney => _firldMoney;
    private int _firldMoney;

    Coroutine _gameCoroutine;

    /// <summary>パスされた回数</summary>
    private int _passCount;

    private int _shouldCallCount => Math.Abs(_myFieldMoney.Count - _enemyFieldMoney.Count);


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
        const int InitializeMoney = 10;

        InitGame();

        Debug.Log("ゲーム開始");
        _currentGameStatus = GameStatus.InProgress;

        yield return new WaitForSeconds(2f);

        _myPlayer.Setup(1, "悪魔", InitializeMoney, true, PlayerSelectCallBack);
        _enemyPlayer.Setup(2, "天使", InitializeMoney, false, PlayerSelectCallBack);

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
        _myFieldMoney.Init(0);
        _enemyFieldMoney.Init(0);
    }

    /// <summary>
    /// 参加料の支払い
    /// </summary>
    private void PayEntryCharge()
    {
        // 参加料
        const int EntryCharge = 1;

        if(!_myPlayer.IsPayable(EntryCharge) || !_enemyPlayer.IsPayable(EntryCharge))
        {
            Debug.LogError("支払い不能プレイヤーがいます");
            return;
        }

        _myFieldMoney.AddMoney(_myPlayer.PayMoney(EntryCharge));
        _enemyFieldMoney.AddMoney(_enemyPlayer.PayMoney(EntryCharge));
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

    private Money GetPlayerFieldMoney(int playerId)
    {
        switch (playerId)
        {
            case 1:
                return _myFieldMoney;
            case 2:
                return _enemyFieldMoney;
            default:
                Debug.LogError($"GetOpponentPlayer > いません:{playerId}");
                return null;
        }
    }

    /// <summary>
    /// プレイヤーの選択コールバック
    /// </summary>
    private void PlayerSelectCallBack(GameDefine.PlayerSelectType select, Player player)
    {
        if (_currentGameStatus != GameStatus.WaitingPlayerSelect ||
           _currentWaitingPlayerId != player.PlayerId)
        {
            Debug.LogError($"PlayerSelectCallBack > 入力待ち状態ではありません 選択プレイヤー:{player.PlayerName}, 待ちプレイヤー:{GetPlayer(_currentWaitingPlayerId).PlayerName}");
            return;
        }

        if(select == GameDefine.PlayerSelectType.Pass)
        {
            _passCount++;
            UnityEngine.Debug.Log($"{_passCount}に");
        }

        StartCoroutine(ChangeTurn(select, player));
    }

    /// <summary>
    /// ターンの切り替え
    /// </summary>
    private IEnumerator ChangeTurn(GameDefine.PlayerSelectType select, Player player)
    {
        Player opponentPlayer = GetOpponentPlayer(player.PlayerId);
        Player.StateType nextState = GameStateUtility.GetOpponentPlayerNextState(player.CurrentStateEnum, select, _passCount);

        PayMoney(select, player);

        yield return new WaitForSeconds(1f);

        // 相手の設定
        opponentPlayer.ChangeState(nextState);

        _afterPlayerAction = GameStateUtility.GetAfterPlayerAction(player.CurrentStateEnum, select, _passCount);

        if (_afterPlayerAction == AfterPlayerActionType.PlayerSelect)
        {
            _currentWaitingPlayerId = opponentPlayer.PlayerId;
        }
    }

    /// <summary>
    /// 支払い
    /// </summary>
    private void PayMoney(GameDefine.PlayerSelectType select, Player player)
    {
        switch (select)
        {
            case GameDefine.PlayerSelectType.Bet:
                const int BetMoney = 2; // 仮
                Debug.LogWarning("仮でベットを2としています");
                GetPlayerFieldMoney(player.PlayerId).AddMoney(player.PayMoney(BetMoney));
                break;
            case GameDefine.PlayerSelectType.Call:
                GetPlayerFieldMoney(player.PlayerId).AddMoney(player.PayMoney(_shouldCallCount));
                break;
            case GameDefine.PlayerSelectType.Raise:
                const int RaiseMoney = 1; // 仮
                Debug.LogWarning("仮でレイズを+1としています");
                GetPlayerFieldMoney(player.PlayerId).AddMoney(player.PayMoney(_shouldCallCount + RaiseMoney));
                break;
        }
    }
    
}
