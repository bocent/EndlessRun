using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;

[Serializable]
public class CreateScoreResponse
{
    public string message;
    public int code;
    public bool success;
    public CreateScoreDataResponse data;
}

[Serializable]
public class CreateScoreDataResponse
{
    public string id;
    public int score;
    public string user_id;
    public string game_id;
    public string department_id;
    public string duration;
    public int question_count;
}

public class ScoreAPI : MonoBehaviour
{
    public int score;
    public int quizCount;
    public string user_id;
    //public string game_id = "ffcaeae6-bd0f-4014-b7a0-397a43ac1c70";
    public string department_id;
    public string postUrl = "https://api.mrjrgames.com/score/create";//"https://gms-api.dickyri.net/score/create";
    public string duration;
    public string token;

    public void SendScore(bool correct, Action<int> onComplete)
    {
        StartCoroutine(RequestSendScore(correct, onComplete));
    }

    IEnumerator RequestSendScore(bool correct, Action<int> onComplete)
    {
        // Create a class to hold login data
        CreateScoreData createScoreData = new CreateScoreData();
        createScoreData.answer = correct;
        createScoreData.user_id = user_id;
        createScoreData.game_id = Initial.Instance.isDev ? Consts.DEV_GAME_ID : Consts.PROD_GAME_ID;
        //createScoreData.department_id = department_id;
        createScoreData.duration = duration;

        // Convert object to JSON
        string json = JsonUtility.ToJson(createScoreData);
        Debug.Log("json : " + json);
        // Create request object

        string url = (Initial.Instance.isDev ? Consts.DEV_URL : Consts.PROD_URL) + "/score/create";
        UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json");
        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("token", token);

        // Send request
        Debug.Log("Send Request");
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Login successful!");
            Debug.Log(request.downloadHandler.text); // Response from the server
            CreateScoreResponse response = JsonUtility.FromJson<CreateScoreResponse>(request.downloadHandler.text);
            if (response != null)
            {
                score = response.data.score;
                quizCount = response.data.question_count;
                TrackManager.instance.SetScore(score);
                onComplete?.Invoke(score);

            }
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            RetryPopup.Instance.Show("Terjadi Masalah Koneksi", "Koneksi terputus. Cobalah untuk refresh/gunakan koneksi internet lain", () => SendScore(correct, onComplete));
        }
    }

    // Class to hold login data
    [Serializable]
    public class CreateScoreData
    {
        public bool answer;
        public string user_id;
        public string game_id;
        //public string department_id;
        public string duration;
    }
}
