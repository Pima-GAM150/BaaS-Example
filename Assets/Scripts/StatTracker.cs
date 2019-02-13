using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

using System.Linq;

public class StatTracker : MonoBehaviour {

	public const int maxLevelCompleteTime = 9999; // incomplete levels count really hard against players in rankings

	public static StatTracker stats;

	int[] timesPerLevel; // cached info about how long it took the user to complete each level

	void Awake() {
		if( StatTracker.stats ) {
			DestroyImmediate( gameObject );
			return;
		}
		else {
			stats = this;
			DontDestroyOnLoad( gameObject );
		}

		// the array we're going to use to hold level times is the total of all levels in the game minus the non-game levels
		// these non-game levels are tracked by a constant in the LoadLevel class ("levelOffset")
		timesPerLevel = new int[SceneManager.sceneCountInBuildSettings - LoadLevel.levelOffset];

		// fill the list with the worst possible times for now
		for( int index = 0; index < timesPerLevel.Length; index++ ) timesPerLevel[index] = maxLevelCompleteTime;
	}

	public void SceneChanged( int origSceneIndex, int nextSceneIndex ) {

		// when a scene gets changed, we want to know which actual game level it is on a scale of 0-x where x is the total number of levels
		int origLevelIndex = origSceneIndex - LoadLevel.levelOffset;
		int nextLevelIndex = nextSceneIndex - LoadLevel.levelOffset;

		if( origLevelIndex < 0 ) return; // don't report analytics if you're not in a game level

		if( origLevelIndex == nextLevelIndex ) {

			// we're reloading the same level we were in, so log a 'restart' event
			// note how the Backend methods are not specific to any particular backend
			// you could write a different backend with a different service with the same API
			Backend.manager.ReportAnalytic( "restartedLevel" + origLevelIndex );
			return;
		}

		timesPerLevel[origLevelIndex] = (int)Time.timeSinceLevelLoad; // cache how long it took us to complete this level

		// pack up some json to provide extra info for designers who are interested in this analytics event
		FinishedLevelInfo finishedLevelInfo = new FinishedLevelInfo();
		string json = JsonUtility.ToJson( finishedLevelInfo );

		Backend.manager.ReportAnalytic( "finishedLevel" + origLevelIndex, json );

		// set saved player data across sessions and devices
		if( nextLevelIndex >= 0 ) {
			Backend.manager.lastLevelPlayed = nextSceneIndex;
			Backend.manager.SetCloudData(
				new IntAttribute[] { new IntAttribute { key = "lastLevelPlayed", val = nextLevelIndex } }
			);
		}

		// send leaderboard score data
		Backend.manager.UpdateLeaderboard( timesPerLevel );
	}

	public void GridLocColorFlipped() {

		// keep track of when a user figured out how to flip colors on a tile and report it to the backend
		if( PlayerPrefs.GetInt( "colorEverFlipped", 0 ) == 0 ) {
			Backend.manager.ReportAnalytic( "colorFlipped" );
			print( "Reported color flipped with analytic code " + "colorEverFlipped" );
			PlayerPrefs.SetInt( "colorEverFlipped", 1 );
		}
	}

	// useful data payload for designers who want to know more than that a user finished a level
	// note the constructor, which generates the necessary data so it doesn't have to be set by the constructing method
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

// convenient serializable class for packing together int / string combos
public class IntAttribute {
	public string key;
	public int val;
}
