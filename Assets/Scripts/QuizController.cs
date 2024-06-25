using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class QuizInfo
{
    public string quizId;
    [TextArea] public string question;
    public List<string> choiceList;
    public string answer;
}

public class QuizController : MonoBehaviour
{
    public GameObject quizContainer;
    public List<QuizInfo> quizInfoList;

    public TMP_Text questionText;
    public TMP_Text questionCount;
    public TMP_Text confirmationText;
    public List<Choice> choiceList;

    public string holdAnswer;
    public AudioClip wrongSfx;
    public AudioClip correctSfx;
    public AudioSource sfxSource;

    public GameObject correctSignContainer;
    public GameObject incorrectSignContainer;

    public GameObject gameOverObj;
    public TMP_Text finalScoreText;

    public ScoreAPI scoreAPI;

    private List<int> quizIndexList = new List<int>();
    private List<char> indexingList = new List<char> { 'A', 'B', 'C', 'D' };
    private QuizInfo currentQuiz;


    public int quizCount;
    
    public int score = 0;

    private bool isQuizShown = false;
    private bool isGameOver = false;

    public static QuizController Instance { get; private set; }


    private void Start()
    {
        Instance = this;
        quizCount = 0;
        score = 0;
        isQuizShown = false;
        isGameOver = false;
    }

    public void ShowQuiz()
    {
        if (scoreAPI.quizCount < 10 || score < 80)
        {
            Debug.LogWarning("show quiz");
            if (!isQuizShown)
            {
                Time.timeScale = 0;
                confirmationText.gameObject.SetActive(false);
                quizContainer.SetActive(true);
                currentQuiz = GetQuiz();
                questionText.text = currentQuiz.question;
                for (int i = 0; i < choiceList.Count; i++)
                {
                    choiceList[i].Init(this, indexingList[i].ToString(), currentQuiz.choiceList[i]);
                }
                questionCount.text = "Pertanyaan " + (scoreAPI.quizCount + 1);
                isQuizShown = true;
            }
        }  
    }

    public void CheckScore(int score)
    {
        Debug.Log("check score : " + score + " " + scoreAPI.quizCount);
        this.score = score;
        if (scoreAPI.quizCount >= 10 && score >= 80)
        {
            GameOver(score);
        }
    }

    public void ShowConfirmation()
    {
        confirmationText.gameObject.SetActive(true);
    }

    public void HideQuiz()
    {
        if (!isGameOver)
        {
            Time.timeScale = 1;
        }
        quizContainer.SetActive(false);
        isQuizShown = false;
        isQuizShown = false;
    }

    private QuizInfo GetQuiz()
    {
        //Request API
        int rand = Random.Range(0, quizInfoList.Count);
        if (quizIndexList.Contains(rand))
        {
            return GetQuiz();
        }
        quizIndexList.Add(rand);
        if (quizIndexList.Count >= quizInfoList.Count)
        {
            quizIndexList.Clear();
        }
        return quizInfoList[rand];
    }

    public void CollectAnswer(string holdAnswer)
    {
        this.holdAnswer = holdAnswer;
    }

    public void CheckAnswer()
    {
        int index = indexingList.FindIndex(x => x.ToString() == holdAnswer);
        if (currentQuiz.answer == index.ToString())
        {
            ShowCorrectSign();
        }
        else
        {
            ShowIncorrectSign();
        }

        
    }

    private void ShowCorrectSign()
    {
        //add Score
        
        correctSignContainer.SetActive(true);
        sfxSource.PlayOneShot(correctSfx);
        StartCoroutine(WaitForInactive(correctSignContainer, 0.2f, HideQuiz));
        scoreAPI.SendScore(true, (score) =>
        {
            if (scoreAPI.quizCount >= 10 && score >= 80)
            {
                GameOver(score);
            }
            Time.timeScale = 0.2f;
        });
    }




    private void ShowIncorrectSign()
    {
        incorrectSignContainer.SetActive(true);
        sfxSource.PlayOneShot(wrongSfx);
        Time.timeScale = 0.2f;
        StartCoroutine(WaitForInactive(incorrectSignContainer, 0.2f, HideQuiz));
        scoreAPI.SendScore(false, (score) => Time.timeScale = 0.2f);
        //decrease score
    }

    private IEnumerator WaitForInactive(GameObject go, float duration, Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        go.SetActive(false);
        onComplete?.Invoke();
        Time.timeScale = 1f;
    }


    private void GameOver(int score)
    {
        //GameManager.Instance.GameOver();
        gameOverObj.SetActive(true);
        finalScoreText.text = score.ToString();
        isGameOver = true;
        Time.timeScale = 0;
    }
}
