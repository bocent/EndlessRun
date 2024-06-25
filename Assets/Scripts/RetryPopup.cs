using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetryPopup : MonoBehaviour
{
    public GameObject container;
    public Text titleText;
    public Text contentText;
    public Button button;

    public static RetryPopup Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Show(string title, string content, Action onButtonClicked)
    {
        container.SetActive(true);
        titleText.text = title;
        contentText.text = content;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onButtonClicked.Invoke);
        button.onClick.AddListener(Hide);
    }

    private void Hide()
    {
        container.SetActive(false);
    }
}
