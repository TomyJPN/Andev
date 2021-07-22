using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO ifとswitchが乱立してるので、せめてDictionaryで定義を登録するようにしたい
public static class GameStateUtility
{
    /// <summary>
    /// ゲーム全体の状態（Manager視点）
    /// </summary>
    public enum GameStatus
    {
        None,
        /// <summary>進行中</summary>
        InProgress,
        /// <summary>プレイヤー選択待ち</summary>
        WaitingPlayerSelect,
    }

    /// <summary>
    /// プレイヤー行動後の状態種別
    /// </summary>
    public enum AfterPlayerActionType
    {
        None,
        /// <summary>引き続きプレイヤーの選択</summary>
        PlayerSelect,
        /// <summary>「勝負！」演出</summary>
        BattleEffect,
        /// <summary>勝敗ジャッジ</summary>
        Judgement
    }

    /// <summary>
    /// 次の相手の行動を取得する
    /// </summary>
    public static Player.StateType GetOpponentPlayerNextState(Player.StateType currentState, GameDefine.PlayerSelectType select)
    {

        if (currentState == Player.StateType.Select1)
        {
            switch (select)
            {
                case GameDefine.PlayerSelectType.Bet:
                    return Player.StateType.Select2;

                case GameDefine.PlayerSelectType.Pass:
                    return Player.StateType.Select1;
            }
        }

        if (currentState == Player.StateType.Select2)
        {
            switch (select)
            {
                case GameDefine.PlayerSelectType.Call:
                case GameDefine.PlayerSelectType.Fold:
                    // TODO:勝負待ちステータス作る
                    return Player.StateType.Waiting;

                case GameDefine.PlayerSelectType.Raise:
                    return Player.StateType.Select3;
            }
        }

        if (currentState == Player.StateType.Select3)
        {
            return Player.StateType.Waiting;
        }

        return Player.StateType.Waiting;
    }

    /// <summary>
    /// プレイヤー行動後の次の状態を取得する
    /// </summary>
    public static AfterPlayerActionType GetAfterPlayerAction(Player.StateType currentState, GameDefine.PlayerSelectType select, int passCount)
    {
        switch (select)
        {
            case GameDefine.PlayerSelectType.Bet:
                return AfterPlayerActionType.PlayerSelect;

            case GameDefine.PlayerSelectType.Call:
                return AfterPlayerActionType.BattleEffect;

            case GameDefine.PlayerSelectType.Fold:
                return AfterPlayerActionType.Judgement;

            case GameDefine.PlayerSelectType.Raise:
                return AfterPlayerActionType.PlayerSelect;

            case GameDefine.PlayerSelectType.Pass:
                return passCount == 2
                    ? AfterPlayerActionType.BattleEffect
                    : AfterPlayerActionType.PlayerSelect;

            default:
                return AfterPlayerActionType.None;
        }

    }

}
