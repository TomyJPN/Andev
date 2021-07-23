using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDialog : MonoBehaviour
{
    [Serializable]
    public class ButtonTextPair
    {
        public Text Text;
        public Button Button;
    }

    [SerializeField]
    private List<ButtonTextPair> buttonTextList;

    public void Setup(List<UIManager.ButtonRegisterData> buttonRegisterDataList)
    {
        HideButtons();

        int i = 0;
        foreach (var data in buttonRegisterDataList)
        {
            if (i > buttonTextList.Count)
            {
                Debug.LogError($"SelectDialog > ボタン登録に想定外の数量のデータです:{i}");
                break;
            }

            buttonTextList[i].Text.text = data.ButtonText;
            buttonTextList[i].Button.onClick.AddListener(() =>
            {
                data.ButtonCallback?.Invoke();
                OnClickButton();
            }
            );

            buttonTextList[i].Button.gameObject.SetActive(true);

            i++;
        }
    }

    private void HideButtons()
    {
        foreach(var button in buttonTextList)
        {
            button.Button.gameObject.SetActive(false);
        }
    }

    private void OnClickButton()
    {
        Destroy(this.gameObject);
    }
}
