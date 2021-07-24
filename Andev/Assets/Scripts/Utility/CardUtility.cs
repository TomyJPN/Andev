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
        List<Card> cardPool = Enumerable.Range(1, 13)
            .Select(num => new Card(num))
            .ToList();

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
    /// 強いかの判定
    /// </summary>
    /// <param name="target">強いかの判定対象</param>
    /// <param name="comparedTarget">比較される</param>
    public static bool IsStrong(Card target, Card comparedTarget)
    {
        return target.Number >= comparedTarget.Number;
    }
}
