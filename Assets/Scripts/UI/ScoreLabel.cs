using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreLabel : MonoBehaviour
{
	public TextMeshProUGUI label;
	public int score { get; set; }

	public void SetScore( int newScore ) {
		score = newScore;

		if( score == StatTracker.maxLevelCompleteTime ) {
			SetIncomplete();
		}
		else {
			label.text = score.ToString();
		}
	}

	public void SetIncomplete() {
		score = StatTracker.maxLevelCompleteTime;
		label.text = "-";
	}
}
