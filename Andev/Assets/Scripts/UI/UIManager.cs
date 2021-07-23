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
        GameObject prefab = (GameObject)Resources.Load("SelectDialog");

        if(prefab == null)
        {
            Debug.LogError("ShowSelectDialog > ロード失敗");
            return;
        }

        GameObject go = Instantiate(prefab);
        go.GetComponent<SelectDialog>().Setup(buttonRegisterDataList);
        go.transform.SetParent(_canvasObj.transform, false);
    }
}
