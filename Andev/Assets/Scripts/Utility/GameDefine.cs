using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDefine
{
    /// <summary>
    /// 選択肢タイプ
    /// </summary>
    public enum PlayerSelectType
    {
        None,
        /// <summary>賭ける</summary>
        Bet,
        /// <summary>相手と同値賭ける</summary>
        Call,
        /// <summary>パス</summary>
        Pass,
        /// <summary>相手を超えて賭ける</summary>
        Raise,
        /// <summary>降りる</summary>
        Fold
    }

    /// <summary>
    /// 位置種別
    /// </summary>
    public enum PositionType
    {
        None,
        Left,
        Right
    }

    public static PositionType GetOpponentPosition(PositionType position)
    {
        return position == PositionType.Left ? PositionType.Right : PositionType.Left;
    }
}
