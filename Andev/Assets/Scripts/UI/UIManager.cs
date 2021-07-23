using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    private GameObject _canvasObj;        

    /// <summary>
    /// ボタンの登録に使うデータ
    /// </summary>
    public class ButtonRegisterData
    {
        public string ButtonText;
        public Action ButtonCallback;
    }

    protected override void OnInitialize()
    {
        _canvasObj = GameObject.Find("Canvas");
        if(_canvasObj == null)
        {
            Debug.LogError("Canvasを取得できませんでした");
        }
    }

    /// <summary>
    /// 選択肢ダイアログの表示
    /// </summary>
    public void ShowSelectDialog(List<ButtonRegisterData> buttonRegisterDataList)
    {
        GameObject prefab = LoadPrefab("SelectDialog");

        if(prefab == null)
        {
            Debug.LogError("ShowSelectDialog > ロード失敗");
            return;
        }

        GameObject go = Instantiate(prefab);
        go.GetComponent<SelectDialog>().Setup(buttonRegisterDataList);
        SetParentCanvas(go);
    }

    /// <summary>
    /// 数値選択ダイアログの表示
    /// </summary>
    /// <param name="min">選択できる最小値</param>
    /// <param name="max">選択できる最大値</param>
    /// <param name="onDecide">決定した時のコールバック（int:選択数値）</param>
    public void ShowCountSelectDialog(int min, int max, Action<int> onDecide)
    {
        GameObject prefab = LoadPrefab("CountSelectDialog");

        if (prefab == null)
        {
            Debug.LogError("ShowCountSelectDialog > ロード失敗");
            return;
        }

        GameObject go = Instantiate(prefab);
        go.GetComponent<CountSelectDialog>().Setup(min ,min, max, onDecide);
        SetParentCanvas(go);
    }

    private GameObject LoadPrefab(string prefabName)
    {
        return (GameObject)Resources.Load(prefabName);
    }

    private void SetParentCanvas(GameObject gameObject)
    {
        gameObject.transform.SetParent(_canvasObj.transform, false);
    }
}
