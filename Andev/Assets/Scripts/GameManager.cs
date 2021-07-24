using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // TODO 別クラスに移す
    [SerializeField]
    private CardViewer _LeftCard;

    // TODO 別クラスに移す
    [SerializeField]
    private CardViewer _RightCard;

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

        yield return new WaitForSeconds(1f);

        _myPlayer.Setup(1, "悪魔", GameDefine.PositionType.Left, PlayerSelectCallBack, _moneyManager);
        _enemyPlayer.Setup(2, "天使", GameDefine.PositionType.Right, PlayerSelectCallBack, _moneyManager);

        while (!_moneyManager.AnyEmptyMoneyPlayer())
        {
            yield return RoundLoop();
        }

        UIManager.Instance.ShowGameMessage("終了!!!!!!!", 3f);

        yield return new WaitForSeconds(3f);

        UIManager.Instance.ShowSelectDialog(new List<UIManager.ButtonRegisterData> {
            new UIManager.ButtonRegisterData
            {
                ButtonText = "再プレイ",
                ButtonCallback = Restart
            }
        });

    }

    /// <summary>
    /// 1ラウンドのループ
    /// </summary>
    private IEnumerator RoundLoop()
    {
        SetupRound();

        PayEntryCharge();

        UIManager.Instance.ShowGameMessage("カードを引く", 0.5f);
        yield return new WaitForSeconds(0.5f);

        DrawCard();

        yield return new WaitForSeconds(0.5f);

        // とりあえず悪魔を先攻
        const int FirstPlayerId = 1;
        GetPlayer(FirstPlayerId).ChangeState(Player.StateType.Select1);
        _currentWaitingPlayerId = FirstPlayerId;

        _afterPlayerAction = AfterPlayerActionType.PlayerSelect;

        // 互いの選択が続く
        _currentGameStatus = GameStatus.WaitingPlayerSelect;
        yield return new WaitUntil(() => _afterPlayerAction != AfterPlayerActionType.PlayerSelect);

        if(_afterPlayerAction == AfterPlayerActionType.BattleEffect)
        {
            UIManager.Instance.ShowGameMessage("勝負！！", 1f);
            yield return new WaitForSeconds(1f);
        }

        Judgement();

        yield return new WaitForSeconds(2f);
    }

    private void InitGame()
    {
        _moneyManager.Init(10, 0);
    }

    private void SetupRound()
    {
        _passCount = 0;
        _LeftCard.Clear();
        _RightCard.Clear();
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

    private void DrawCard()
    {
        List<Card> drawCard = CardUtility.DrawCard(2);
        _myPlayer.SetCard(drawCard[0]);
        _enemyPlayer.SetCard(drawCard[1]);

        _LeftCard.SetCard(_myPlayer.HaveCard);
        _RightCard.SetCard(_enemyPlayer.HaveCard);
    }

    private void Judgement()
    {
        string message;

        if (_afterPlayerAction == AfterPlayerActionType.FoldJudgement)
        {
            Player foldPlayer = GetPlayer(_currentWaitingPlayerId);
            _moneyManager.ExecuteWiningPay(GetOpponentPlayer(foldPlayer.PlayerId).Position);

            message = string.Format(Wording.LoadWord("message_fold_player"), foldPlayer.PlayerName);
        }
        else
        {
            Player winner = CardUtility.IsStrong(_myPlayer.HaveCard, _enemyPlayer.HaveCard)
                ? _myPlayer
    :           _enemyPlayer;

            _moneyManager.ExecuteWiningPay(winner.Position);

            message = string.Format(Wording.LoadWord("message_wining_player"), winner.PlayerName);
        }

        UIManager.Instance.ShowGameMessage(message, 1.5f);
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

        yield return new WaitForSeconds(0.5f);

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
