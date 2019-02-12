using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LeaderboardLayout : MonoBehaviour
{
	const string labelTextPrefix = "Level ";

	public RectTransform labelParent;
	public TextMeshProUGUI levelLayoutPrefab;

	void Start() {
		// generate level labels
		for( int index = 2; index < SceneManager.sceneCountInBuildSettings; index++ ) {
			TextMeshProUGUI newLabel = Instantiate<TextMeshProUGUI>( levelLayoutPrefab );
			newLabel.transform.SetParent( labelParent, false );

			newLabel.text = labelTextPrefix + (index - 1);
		}
	}
}
