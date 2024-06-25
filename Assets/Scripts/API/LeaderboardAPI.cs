using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class LeaderboardAPI : MonoBehaviour
{
    private const string serverURL = "https://api.mrjrgames.com/score";//"https://gms-api.dickyri.net/score/";
    public ScoreAPI scoreAPI;

    public HighscoreUI[] rankItems;

    public static LeaderboardAPI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void GetLeaderboardData()
    {
        StartCoroutine(GetData());
    }

    [System.Serializable]
    public class LeaderboardDatas
    {
        public string game;
    }

    IEnumerator GetData()
    {
        for (int i = 0; i < rankItems.Length; i++)
        {
            rankItems[i].Init((i + 1), "------", "------", "--");
        }

        Debug.Log("Getting Leaderboard Data");


        string url = (Initial.Instance.isDev ? Consts.DEV_URL : Consts.PROD_URL) + "/score";

        // Create UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(url + "?game=" + (Initial.Instance.isDev ? Consts.DEV_GAME_ID : Consts.PROD_GAME_ID));
        request.downloadHandler = new DownloadHandlerBuffer();
       
        // Send request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching leaderboard: " + request.error);
        }
        else
        {
            // Parse JSON response
            string jsonResponse = request.downloadHandler.text;
            LeaderboardResponse leaderboardResponse = JsonUtility.FromJson<LeaderboardResponse>(jsonResponse);
            Debug.Log("leaderboard : " + jsonResponse);
            // Display leaderboard data
            if (leaderboardResponse != null && leaderboardResponse.data != null)
            {
                Debug.Log("leaderboard count : " + leaderboardResponse.data.Count);
                for (int i = 0; i < leaderboardResponse.data.Count; i++)
                {
                    if (i < 5)
                    {
                        rankItems[i].Init((i + 1), leaderboardResponse.data[i].User.username, leaderboardResponse.data[i].Department.name, leaderboardResponse.data[i].score.ToString());

                        Debug.Log("Rank " + (i + 1) + ": " +
                                 "Username: " + leaderboardResponse.data[i].User.username + ", " +
                                 "Score: " + leaderboardResponse.data[i].score + ", " +
                                 "Department: " + leaderboardResponse.data[i].Department.name);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class LeaderboardData
    {
        public string id;
        public string user_id;
        public int score;
        public string department_id;
        public GameData Game;
        public UserData User;
        public DepartmentData Department;
    }

    [System.Serializable]
    public class GameData
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class UserData
    {
        public string id;
        public string username;
        public string email;
    }

    [System.Serializable]
    public class DepartmentData
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class LeaderboardResponse
    {
        public List<LeaderboardData> data;
    }
}
