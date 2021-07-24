using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardViewer : MonoBehaviour
{
    /// <summary>カードの画像</summary>
    [SerializeField]
    private Image _cardSprite;

    public void Start()
    {
        SetEnable(false);
    }

    /// <summary>
    /// カードのセット
    /// </summary>
    /// <param name="card"></param>
    public void SetCard(Card card)
    {
        SetSprite(CardUtility.GetCardImagePathName(card));
    }

    /// <summary>
    /// 画像のクリア
    /// </summary>
    public void Clear()
    {
        _cardSprite.sprite = null;
        SetEnable(false);
    }

    /// <summary>
    /// スプライトのセット
    /// </summary>
    private void SetSprite(string path)
    {
        Debug.Log(path);

        Sprite image = Resources.Load<Sprite>(path);
        if (image == null)
        {
            Debug.LogError("CardViewer > ロード失敗");
            return;
        }

        _cardSprite.sprite = image;
        SetEnable(true);
    }

    /// <summary>
    /// 画像の有効設定
    /// </summary>
    private void SetEnable(bool enabled)
    {
        _cardSprite.enabled = enabled;
    }
}
