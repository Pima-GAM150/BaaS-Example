using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;

public class Backend : MonoBehaviour {

	const string saveEventKey = "SAVED_PLAYER_DATA";
	const string loadEventKey = "LOAD_PLAYER_DATA";
	const string leaderboardEventKey = "SUBMIT_SCORE";
	const string leaderboardShortcode = "highScores";

	public static Backend manager;

	public string displayName { get; set; }

	void Awake() {
		if( Backend.manager ) {
			DestroyImmediate( gameObject );
			return;
		}
		else {
			manager = this;
			DontDestroyOnLoad( gameObject );
		}
	}

	public void ReportAnalytic( string key, string json = "", Action<AnalyticsResponse> callback = null ) {
		if( callback == null ) callback = EmptyAnalyticsCallback;

		AnalyticsRequest request = new AnalyticsRequest()
			.SetKey( key );

		if( json != "" ) {
			request.SetData( new GSRequestData( json ) );
		}

		request.Send( EmptyAnalyticsCallback );
	}

	public void SetCloudData( params IntAttribute[] values ) {
		LogEventRequest request = new LogEventRequest().SetEventKey( saveEventKey );

		foreach( IntAttribute attribute in values ) {
			request.SetEventAttribute( attribute.key, attribute.val );
		}

		request.Send( EmptyEventCallback );
	}

	public void GetCloudInts( string[] keys, Action<int[]> callback ) {
		int[] values = new int[keys.Length];

		new LogEventRequest().SetEventKey( loadEventKey ).Send( response => {
			if( !response.HasErrors ) {
				GSData data = response.ScriptData.GetGSData("playerData");

				for( int index = 0; index < keys.Length; index++ ) {
					values[index] = (int)data.GetInt( keys[index] );
				}

				callback( values );
			}
			else {
				Debug.LogError( "Error from event backend: " + response.Errors.JSON.ToString() );

				callback( null );
			}
		} );
	}

	public void UpdateLeaderboard( int[] intTimes ) {
		TimesPerLevel times = new TimesPerLevel { times = intTimes };
		string json = JsonUtility.ToJson( times );

		LogEventRequest request = new LogEventRequest().SetEventKey( leaderboardEventKey );

		request.SetEventAttribute( "averageTime", intTimes.Average().ToString() );
		request.SetEventAttribute( "times", json );

		request.Send( EmptyEventCallback );
	}

	public void GetLeaderboardTimes( Action<List<LeaderboardInfo>> callback ) {

		List<LeaderboardInfo> leaderboardInfos = new List<LeaderboardInfo>();

		new LeaderboardDataRequest().SetLeaderboardShortCode( leaderboardShortcode ).SetEntryCount( 10 ).Send( response => {

			if( !response.HasErrors ) {
				Debug.Log( "Found Leaderboard Data..." );

				foreach( LeaderboardDataResponse._LeaderboardData entry in response.Data ) {

					string timesJson = entry.JSONData["times"].ToString();
					int[] times = JsonUtility.FromJson<TimesPerLevel>( timesJson ).times;
					int playerRank = (int) entry.Rank;
					string playerName = entry.UserName;

					LeaderboardInfo leaderboardInfo = new LeaderboardInfo {
						timesByLevel = times,
						rank = playerRank,
						name = playerName
					};

					leaderboardInfos.Add( leaderboardInfo );

					Debug.Log( "Rank:" + playerRank + " Name:" + playerName + " \n Score:" + timesJson );
				}

				callback( leaderboardInfos );
			}
			else {
				Debug.Log( "Error Retrieving Leaderboard Data... " + response.Errors.JSON.ToString());
			}
		});
	}

	void EmptyAnalyticsCallback( AnalyticsResponse response ) {
		if( response.HasErrors ) {
			Debug.LogError( "Error from analytics: " + response.Errors.JSON.ToString() );
		}
	}

	void EmptyEventCallback( LogEventResponse response ) {
		if( response.HasErrors ) {
			Debug.LogError( "Error from event backend: " + response.Errors.JSON.ToString() );
		}
		else {
			Debug.Log( "Successfully saved event to backend" );
		}
	}

	class TimesPerLevel {
		public int[] times;
	}
}