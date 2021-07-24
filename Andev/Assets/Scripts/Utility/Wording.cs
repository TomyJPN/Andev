using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wording
{
    private static readonly Dictionary<string, string> _wordMap = new Dictionary<string, string>
    {
        { "bet", "賭ける" },
        { "call", "勝負する" },
        { "pass", "パスする" },
        { "raise", "ふやす" },
        { "fold", "おりる" },
        { "message_wining_player", "{0}のかち" },
        { "message_fold_player", "{0}がおりた" },
        { "", "" },
    };

    /// <summary>
    /// 文言のロード
    /// </summary>
    public static string LoadWord(string wordingKey)
    {
        string key = wordingKey.ToLower();

        if (!_wordMap.ContainsKey(key))
        {
            Debug.LogError($"LoadWord > 文言:{key}がありません");
            return "";
        }

        return _wordMap[key];
    }
}
