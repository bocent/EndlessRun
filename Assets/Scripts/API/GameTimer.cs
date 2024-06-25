using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text timerText;
    private float timer;
    public ScoreAPI scoreAPI;

    public static GameTimer Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public void StartTimer(string timeString)
    {
        Debug.Log("timestring : " + timeString);
        if (!string.IsNullOrEmpty(timeString))
        {
            string[] times = timeString.Split(':');
            if (times.Length > 2)
            {
                timer = int.Parse(times[0]) * 3600 + int.Parse(times[1]) * 60 + int.Parse(times[2]);
                Debug.Log("timer : " + timer);
                //this.timer = timer;
            }
        }
    }

    private void Update()
    {
        // Update the timer
        timer += Time.deltaTime;

        // Convert timer to hours, minutes, seconds
        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt((timer % 3600) / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        // Update the UI text
        scoreAPI.duration = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

}
