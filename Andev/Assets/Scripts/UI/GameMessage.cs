using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMessage : MonoBehaviour
{
    [SerializeField]
    private Text _messageLabel;

    /// <summary>
    /// セットアップ
    /// </summary>
    /// <param name="message">表示メッセージ</param>
    /// <param name="showTime">表示時間</param>
    public void Setup(string message, float showTime = 2f)
    {
        _messageLabel.text = message;
        StartCoroutine(ViewingCoroutine(showTime));
    }

    /// <summary>
    /// 表示コルーチン
    /// </summary>
    private IEnumerator ViewingCoroutine(float showTime)
    {
        yield return new WaitForSeconds(showTime);

        Close();
    }

    private void Close()
    {
        Destroy(this.gameObject);
    }
}
