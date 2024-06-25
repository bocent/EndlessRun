using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Choice : MonoBehaviour
{
    public TMP_Text numberingText;
    public TMP_Text answerText;

    private Button button;
    private string numbering;
    private QuizController controller;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    public void Init(QuizController controller, string numbering, string answer)
    {
        this.controller = controller;
        this.numbering = numbering;
        numberingText.text = numbering;
        answerText.text = answer;
    }

    public void OnButtonClicked()
    {
        controller.CollectAnswer(numbering);
    }
}
