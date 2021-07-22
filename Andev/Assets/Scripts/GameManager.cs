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

    private GameStatus _currentGameStatus;
    private AfterPlayerActionType _afterPlayerAction;
    private uint _currentWaitingPlayerId;

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
        const int InitializeMoney = 10;
        _passCount = 0;

        Debug.Log("ゲーム開始");
        _currentGameStatus = GameStatus.InProgress;

        _myPlayer.Setup(1, "悪魔", InitializeMoney, true, PlayerSelectCallBack);
        _enemyPlayer.Setup(2, "天使", InitializeMoney, false, PlayerSelectCallBack);

        yield return new WaitForSeconds(2f);

        // とりあえず先攻
        _currentWaitingPlayerId = 1;

        _afterPlayerAction = AfterPlayerActionType.PlayerSelect;

        // 互いの選択が続く
        _currentGameStatus = GameStatus.WaitingPlayerSelect;
        yield return new WaitUntil(() => _afterPlayerAction != AfterPlayerActionType.PlayerSelect);
    
        Debug.Log($"{_afterPlayerAction}演出を行って判定します！", Debug.Green);
    }

    /// <summary>
    /// プレイヤーの取得
    /// </summary>
    private Player GetPlayer(uint playerId)
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
    private Player GetOpponentPlayer(uint playerId)
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
    private void PlayerSelectCallBack(GameDefine.selectType select, Player player)
    {
        if (_currentGameStatus != GameStatus.WaitingPlayerSelect ||
           _currentWaitingPlayerId != player.PlayerId)
        {
            Debug.LogError($"PlayerSelectCallBack > 入力待ち状態ではありません 選択プレイヤー:{player.PlayerName}, 待ちプレイヤー:{GetPlayer(_currentWaitingPlayerId).PlayerName}");
            return;
        }

        if(select == GameDefine.selectType.Pass)
        {
            _passCount++;
        }

        StartCoroutine(ChangeTurn(select, player));
    }

    /// <summary>
    /// ターンの切り替え
    /// </summary>
    private IEnumerator ChangeTurn(GameDefine.selectType select, Player player)
    {
        Player opponentPlayer = GetOpponentPlayer(player.PlayerId);
        Player.StateType nextState = GameStateUtility.GetOpponentPlayerNextState(player.CurrentStateEnum, select);

        yield return new WaitForSeconds(1f);

        // 相手の設定
        opponentPlayer.ChangeState(nextState);

        _afterPlayerAction = GameStateUtility.GetAfterPlayerAction(player.CurrentStateEnum, select, _passCount);

        if (_afterPlayerAction == AfterPlayerActionType.PlayerSelect)
        {
            _currentWaitingPlayerId = opponentPlayer.PlayerId;
        }
    }
    
}
