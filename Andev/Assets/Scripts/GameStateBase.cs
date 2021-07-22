using UnityEngine;

/// <summary>
/// State抽象クラス
/// </summary>
public abstract class PlayerStateBase
{
    /// <summary>
    /// ステート開始時
    /// </summary>
    public virtual void OnEnter(Player owner, PlayerStateBase prevState)
    {
        Debug.Log($"【state変更】{owner.PlayerName}が{owner.CurrentStateName}を開始");
    }

    /// <summary>
    /// 毎フレーム
    /// </summary>
    public virtual void OnUpdate(Player owner)
    {
    }

    /// <summary>
    /// ステート終了時
    /// </summary>
    public virtual void OnExit(Player owner, PlayerStateBase nextState)
    {
    }
}