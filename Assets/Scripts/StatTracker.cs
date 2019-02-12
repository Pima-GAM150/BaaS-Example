using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

using System.Linq;

public class StatTracker : MonoBehaviour {

	const int maxLevelCompleteTime = 9999;

	public static StatTracker stats;

	int[] timesPerLevel;

	void Awake() {
		if( StatTracker.stats ) {
			DestroyImmediate( gameObject );
			return;
		}
		else {
			stats = this;
			DontDestroyOnLoad( gameObject );
		}

		print("Initializing times per level to an array with " + SceneManager.sceneCountInBuildSettings + " entries ");
		timesPerLevel = new int[SceneManager.sceneCountInBuildSettings - 2];
		for( int index = 0; index < timesPerLevel.Length; index++ ) timesPerLevel[index] = maxLevelCompleteTime;
	}

	void Start() {
		PlayerPrefs.SetInt( "colorEverFlipped", 0 ); // debug

		Backend.manager.ReportAnalytic( "timeToCompleteLevel" + SceneManager.GetActiveScene().buildIndex );

		if( PlayerPrefs.GetInt( "colorEverFlipped", 0 ) == 0 ) {
			Backend.manager.ReportAnalytic( "colorFlipped" );
			print( "Reported waiting for color to flip with analytic code " + "colorEverFlipped" );
		}
	}

	public void SceneChanged( int origSceneIndex, int nextSceneIndex ) {

		int origLevelIndex = origSceneIndex - LoadLevel.levelOffset;
		int nextLevelIndex = nextSceneIndex - LoadLevel.levelOffset;

		if( origLevelIndex < 0 ) return;

		print( "Scene change detected by stat tracker from scene index " + origLevelIndex + " to scene " + nextLevelIndex );

		if( origLevelIndex == nextLevelIndex ) {
			Backend.manager.ReportAnalytic( "restartedLevel" + origLevelIndex );
			print( "Reported level restart with analytic code " + "restartedLevel" + origLevelIndex );
			return;
		}

		string timesPrint = "";
		for( int index = 0; index < timesPerLevel.Length; index++ ) timesPrint += timesPerLevel[index] + ":";
		print( "Adding time for level " + origLevelIndex + " (whole collection is: " + timesPrint + ")" );
		timesPerLevel[origLevelIndex] = (int)Time.timeSinceLevelLoad;

		FinishedLevelInfo finishedLevelInfo = new FinishedLevelInfo();
		string json = JsonUtility.ToJson( finishedLevelInfo );

		Backend.manager.ReportAnalytic( "finishedLevel" + origLevelIndex, json );

		// set saved player data across sessions and devices
		Backend.manager.SetCloudData(
			new IntAttribute[] { new IntAttribute { key = "lastLevelPlayed", val = nextLevelIndex } }
		);

		// send leaderboard score data
		Backend.manager.UpdateLeaderboard( timesPerLevel );

		print( "Reported level end with analytic code " + "finishedLevel" + origLevelIndex + " and data " + json );
	}

	public void GridLocColorFlipped() {
		if( PlayerPrefs.GetInt( "colorEverFlipped", 0 ) == 0 ) {
			Backend.manager.ReportAnalytic( "colorFlipped" );
			print( "Reported color flipped with analytic code " + "colorEverFlipped" );
			PlayerPrefs.SetInt( "colorEverFlipped", 1 );
		}
	}

	public class FinishedLevelInfo {
		public string username;
		public int secondsToComplete;
		public int numClicks;

		public FinishedLevelInfo() {
			username = PlayerPrefs.GetString( "onlineName", "unknown" );
			secondsToComplete = (int)Time.timeSinceLevelLoad;
			numClicks = GridManager.data.allGridLocations.Sum( location => location.numClicks );
		}
	}
}

public class IntAttribute {
	public string key;
	public int val;
}
