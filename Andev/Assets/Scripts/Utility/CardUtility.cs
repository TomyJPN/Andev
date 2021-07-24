using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CardUtility
{
    /// <summary>
    /// 指定枚数カードをランダムにドロー
    /// </summary>
    public static List<Card> DrawCard(int drawCount)
    {
        List<Card> cardPool = CreateCardPool();

        List<Card> result = new List<Card>();

        for(int i = 0; i < drawCount; i++)
        {
            int index = Random.Range(0, cardPool.Count());
            result.Add(cardPool[index]);
            cardPool.RemoveAt(index);
        }

        return result;
    }

    /// <summary>
    /// トランプカードプールの作成
    /// </summary>
    private static List<Card> CreateCardPool()
    {
        List<Card> cardPool = new List<Card>();

        foreach(Card.SuiteType suite in System.Enum.GetValues(typeof(Card.SuiteType)))
        {
            for(int i = 1; i <= 13; i++)
            {
                cardPool.Add(new Card(suite, i));
            }
        }

        return cardPool;
    }

    /// <summary>
    /// 強いかの判定
    /// </summary>
    /// <param name="target">強いかの判定対象</param>
    /// <param name="comparedTarget">比較される</param>
    public static bool IsStrong(Card target, Card comparedTarget)
    {
        if(target.Strength == comparedTarget.Strength)
        {
            // 数字が同じなら記号で判定
            return target.Suite < comparedTarget.Suite;
        }

        return target.Strength >= comparedTarget.Strength;
    }

    /// <summary>
    /// カードのファイル名取得
    /// </summary>
    public static string GetCardImagePathName(Card card)
    {
        string path = "card/";

        switch (card.Suite)
        {
            case Card.SuiteType.Club:
                path += "c";
                break;
            case Card.SuiteType.Diamond:
                path += "d";
                break;
            case Card.SuiteType.Spade:
                path += "s";
                break;
            case Card.SuiteType.Heart:
                path += "h";
                break;
            default:
                Debug.LogError("GetCardFileName > 想定外");
                return "z02";
        }

        path += string.Format("{0:D2}", card.Rank);

        return path;
    }
}
