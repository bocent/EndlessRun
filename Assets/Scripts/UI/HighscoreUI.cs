using UnityEngine;
using UnityEngine.UI;

public class HighscoreUI : MonoBehaviour
{
	public Text numberText;
	public Text playerNameText;
	public Text departmentText;
	public Text scoreText;

	public void Init(int number, string name, string department, string score)
	{
        numberText.text = number.ToString();
		playerNameText.text = name;
		departmentText.text = department;
		scoreText.text = score;
	}
}
