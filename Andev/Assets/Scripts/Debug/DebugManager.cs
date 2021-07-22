using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    [SerializeField]
    private Button debugButton;

    [SerializeField]
    private GameObject window;

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Button clearButton;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private Text logText;

    public void Update()
    {
        logText.text = Debug.LogText;
    }

    public void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
        clearButton.onClick.AddListener(ClearLog);
        debugButton.onClick.AddListener(SwitchEnableWindow);
    }

    private void SwitchEnableWindow()
    {
        bool enable = window.activeSelf;
        window.SetActive(!enable);
    }

    private void ClearLog()
    {
        Debug.ClearLog();
    }

    private void RestartGame()
    {
        gameManager.Restart();
    }
}
