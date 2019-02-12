using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreLabel : MonoBehaviour
{
	public const int incompleteScore = 9999;

	public TextMeshProUGUI label;
	public int score { get; set; }

	public void SetScore( int newScore ) {
		score = newScore;
		label.text = score.ToString();
	}

	public void SetIncomplete() {
		score = incompleteScore;
		label.text = "-";
	}
}
