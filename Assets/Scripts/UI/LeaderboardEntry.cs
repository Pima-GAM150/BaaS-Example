using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LeaderboardEntry : MonoBehaviour
{
	public RectTransform scoreParent;

	public TextMeshProUGUI playerNameLabel;
	public ScoreLabel scoreLabelPrefab;

	[HideInInspector] public List<ScoreLabel> scoreLabels = new List<ScoreLabel>();

	public void Initialize() {
		// generate level labels
		for( int index = 2; index < SceneManager.sceneCountInBuildSettings; index++ ) {
			ScoreLabel newLabel = Instantiate<ScoreLabel>( scoreLabelPrefab );
			newLabel.transform.SetParent( scoreParent, false );

			scoreLabels.Add( newLabel );
		}
	}

	public void FillLabels( int[] scores ) {
		for( int index = 0; index < scoreLabels.Count; index++ ) {
			if( index < scores.Length ) {
				scoreLabels[index].SetScore( scores[index] );
			}
			else {
				scoreLabels[index].SetIncomplete();
			}
		}
	}
}
