using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLeaderboard : MonoBehaviour
{
	public LeaderboardEntry entryPrefab;
	public RectTransform entryLayout;

	void Start() {
		// request data from the backend
		Backend.manager.GetLeaderboardTimes( OnReceivedLeaderboardInfo );
	}

	void OnReceivedLeaderboardInfo( List<LeaderboardInfo> infos ) {
		// populate ui

		for( int index = 0; index < infos.Count; index++ ) {
			LeaderboardEntry newEntry = Instantiate<LeaderboardEntry>( entryPrefab );
			newEntry.transform.SetParent( entryLayout, false );
			newEntry.playerNameLabel.text = infos[index].name;
			newEntry.Initialize();
			newEntry.FillLabels( infos[index].timesByLevel );
		}
	}
}

public class LeaderboardInfo {
	public int rank;
	public string name;
	public int[] timesByLevel;
}