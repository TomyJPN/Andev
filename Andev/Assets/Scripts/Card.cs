using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    /// <summary>
    /// 記号種別
    /// ※定義順を強さ順として扱っている
    /// </summary>
    public enum SuiteType
    {
        /// <summary>スペード♠</summary>
        Spade,
        /// <summary>ハート♥</summary>
        Heart,
        /// <summary>ダイア♦</summary>
        Diamond,
        /// <summary>クラブ♣</summary>
        Club,
    }

    /// <summary>数値</summary>
    public int Rank => _rank;
    private int _rank;

    /// <summary>強さ(エースは14として扱っている)</summary>
    public int Strength => _rank != 1 
        ? _rank
        : 14;

    /// <summary>記号</summary>
    public SuiteType Suite => _suite;
    private SuiteType _suite;


    public Card(SuiteType suite, int rank)
    {
        _rank = rank;
        _suite = suite;
    }
}
