using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountSelectDialog : MonoBehaviour
{
    [SerializeField]
    private Button _okButton;

    [SerializeField]
    private Button _upButton;

    [SerializeField]
    private Button _downButton;

    [SerializeField]
    private Text _countText;

    private int _currentCount;

    /// <summary>選択できる最小値</summary>
    private int _min;

    /// <summary>選択できる最大値</summary>
    private int _max;

    public void Setup(int initCount, int min, int max, Action<int> onDecide)
    {
        _currentCount = initCount;
        _min = min;
        _max = max;

        _okButton.onClick.AddListener(() =>
        {
            onDecide(_currentCount);
            Close();
        });
        _upButton.onClick.AddListener(OnClickUP);
        _downButton.onClick.AddListener(OnClickDown);

        OnChangeCount();
    }

    private void OnClickUP()
    {
        _currentCount++;
        OnChangeCount();
    }

    private void OnClickDown()
    {
        _currentCount--;
        OnChangeCount();
    }

    private void OnChangeCount()
    {
        _upButton.interactable = _currentCount < _max;
        _downButton.interactable = _currentCount > _min;

        _countText.text = _currentCount.ToString();
    }

    private void Close()
    {
        Destroy(this.gameObject);
    }
}
