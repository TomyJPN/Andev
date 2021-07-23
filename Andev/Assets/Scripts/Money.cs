using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    [SerializeField]
    private Text _countText;

    /// <summary>場の掛け金の数</summary>
    public int Count => _count;
    private int _count;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(int count)
    {
        _count = count;
        OnChangeCount();
    }

    /// <summary>
    /// 指定した量を支払えるか
    /// </summary>
    public bool IsPayable(int payCount)
    {
        return _count > payCount;
    }

    /// <summary>
    /// 掛け金を支払う
    /// </summary>
    public int PayMoney(int count)
    {
        if (!IsPayable(count))
        {
            Debug.LogWarning($"支払えません:{count}");

            int ret = _count;
            SubMoney(ret);
            return ret;
        }

        SubMoney(count);
        return count;
    }

    /// <summary>
    /// 掛け金を追加
    /// </summary>
    public void AddMoney(int addCount)
    {
        _count += addCount;
        OnChangeCount();
    }

    /// <summary>
    /// 掛け金の減算
    /// </summary>
    private void SubMoney(int subCount)
    {
        _count -= subCount;
        OnChangeCount();
    }

    /// <summary>
    /// 数が変わったとき
    /// </summary>
    private void OnChangeCount()
    {
        _countText.text = _count.ToString();
    }
}
