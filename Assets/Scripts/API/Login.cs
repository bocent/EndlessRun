using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;
using System.Security.Cryptography;
using Unity.VisualScripting;
using static Login;

public class Login : MonoBehaviour
{
    public string email;
    public string username;
    public string department;
    // URL for the login request
    //public string postUrl = "https://api.mrjrgames.com/users/login"; // Replace with your actual URL
    public string data;
    int score;
    string duration;

    public ScoreAPI scoreAPI;
    public QuizController quizController;
    public Text nameText;
    public Text scoreText;

    private string[] parameters;
    private Dictionary<string, string> parameterDict = new Dictionary<string, string>();

    public IEnumerator Start()
    {
        GetParameter();
        yield return null;
        StartCoroutine(LoginRequest());
    }

    private void GetParameter()
    {
        int pm = Application.absoluteURL.IndexOf("?");

        if (pm != -1)
        {
            parameters = Application.absoluteURL.Split('&');
            foreach (string parameter in parameters)
            {
                // Split parameter key and value
                string[] keyValue = parameter.Split('=');
                string key = keyValue[0];
                if (keyValue[0].Contains("?"))
                {
                    key = keyValue[0].Substring(keyValue[0].IndexOf("?") + 1);
                }
                string value = keyValue[1];

                Debug.LogFormat("key : {0}, value : {1}", key, value);

                // Do something with the parameter
                parameterDict.Add(key, value);
            }

            if(parameterDict.ContainsKey("username")) username = parameterDict["username"];
            if(parameterDict.ContainsKey("email")) email = parameterDict["email"];

        }
    }

    IEnumerator LoginRequest()
    {
        // Create a class to hold login data
        LoginData loginData = new LoginData();
        loginData.email = email;
        loginData.username = username;
        loginData.department = department;

        //loginData.email = "lb.ananda.nugraha@jasaraharja.co.id";
        //loginData.username = "Ananda Rolanda Nugraha Buburanda";


        // Convert object to JSON
        string json = JsonUtility.ToJson(loginData);


        // Create request object
        string url = (Initial.Instance.isDev ? Consts.DEV_URL : Consts.PROD_URL) + "/users/login";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send request
        Debug.Log("Send Request");
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Login successful!");
            Debug.Log(request.downloadHandler.text); // Response from the server

            data = request.downloadHandler.text;
            Debug.Log("data : " + data);
            DataLogin dataLogin = JsonUtility.FromJson<DataLogin>(data);
            if (dataLogin.score_duration != null)
            {
                ScoreDuration scoreDuration = dataLogin.score_duration.Where(x => x.game_id == (Initial.Instance.isDev ? Consts.DEV_GAME_ID : Consts.PROD_GAME_ID)).FirstOrDefault();
                if (scoreDuration != null)
                {
                    quizController.score = score;
                    score = scoreDuration.score;
                    duration = scoreDuration.duration;
                    quizController.quizCount = scoreDuration.question_count;
                    scoreAPI.score = score;
                    scoreAPI.quizCount = scoreDuration.question_count;
                    quizController.CheckScore(score);

                    scoreText.text = score.ToString();
                }
            }
            scoreAPI.token = dataLogin.token;
            scoreAPI.user_id = dataLogin.user_id;
            scoreAPI.department_id = dataLogin.department_id;
            scoreAPI.duration = duration;

            GameTimer.Instance.StartTimer(duration);

            nameText.text = dataLogin.username;

            

            Debug.Log("SCORE " + score);

            //uIManager.scoreText.text = score.ToString();
            LeaderboardAPI.Instance.GetLeaderboardData();
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            RetryPopup.Instance.Show("Terjadi Masalah Koneksi", "Koneksi terputus. Cobalah untuk refresh/gunakan koneksi internet lain", () => StartCoroutine(LoginRequest()));
        }
    }

    // Class to hold login data
    [System.Serializable]
    public class LoginData
    {
        public string email;
        public string username;
        public string department;
    }

    [System.Serializable]
    public class DataLogin
    {
        public string token;
        public string username;
        public string email;
        public string department;
        public string user_id;
        public string department_id;
        public ScoreDuration[] score_duration;
    }

    [System.Serializable]
    public class ScoreDuration
    {
        public string game_name;
        public string game_id;
        public int score;
        public string duration;
        public int question_count;
    }

}
