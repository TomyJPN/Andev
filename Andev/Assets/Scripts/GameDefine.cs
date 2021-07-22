using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDefine
{
    /// <summary>
    /// 選択肢タイプ
    /// </summary>
    public enum selectType
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

}
