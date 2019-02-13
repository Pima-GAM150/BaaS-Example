using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

using TMPro;

public class LoginToBackend : MonoBehaviour {

	const int firstLevel = 1;

	public Animator animator;
	public Builder spinnerPrefab;
	public RectTransform spinnerParent;

	const string errorUnrecognized = "UNRECOGNISED";
	const string errorNoNetwork = "timeout";
	public const string lastLevelCode = "lastLevel";

	public TMP_InputField usernameLabel;
	public TMP_InputField pwdLabel;
	public TMP_InputField pwdLabel2;
	public TMP_InputField displayLabel;

	public TextMeshProUGUI responseLabel;

	public Color successColor = Color.green;
	public Color failureColor = Color.red;

	public LoadLevel levelLoader;

	Builder spinner;

	// generate a GameSparks registration request and send it, awaiting a response
	public void Register( string displayName, string username, string password ) {
		if( username == "" || password == "" || displayLabel.text == "" ) return; // don't bother if the user hasn't filled in necessary info

		// make sure the two passwords match
		if( CheckPasswordsMatch() ) {

			// show a visual effect to indicate we're waiting for the backend
			ShowSpinner();

			// note how the event request and the SetEventKey methods return a LogEventRequest, so you can 'chain' functions off each other
			new RegistrationRequest().SetDisplayName( displayName ).SetUserName( username ).SetPassword( password ).Send( RegisterResponse );
		}
	}
	public void Register() { Register( displayLabel.text, usernameLabel.text, pwdLabel.text ); } // simple override to be called by UI events

	public void RegisterResponse( RegistrationResponse response ) {
		if( !response.HasErrors ) {

			Backend.manager.displayName = response.DisplayName; // cache the user's display name for easy reference

			LogToConsole( "Player " + response.DisplayName + " created!", successColor );
			Backend.manager.lastLevelPlayed = 2;

			// we just made a new player with no saved data, so just get started fresh
			if( levelLoader ) levelLoader.Load( firstLevel );
		}
		else {
			LogToConsole( "Error Authenticating Player: " + response.Errors.JSON, failureColor );
		}

		spinner.animator.SetBool( "Toggle", false );
	}

	// these methods are very similar to the registration request
	public void Login( string username, string password ) {
		if( username == "" || password == "" ) return;

		ShowSpinner();
		
		new AuthenticationRequest().SetUserName( username ).SetPassword( password ).Send( LoginResponse );
	}
	public void Login() { Login( usernameLabel.text, pwdLabel.text ); }

	public void LoginResponse( AuthenticationResponse response ) {
		if( !response.HasErrors ) {

			Backend.manager.displayName = response.DisplayName;

			LogToConsole( "Welcome, " + response.DisplayName + "!", successColor );

			FindLastLevel();
		}
		else {			

			// check what's wrong if the login response has errors
			if( response.Errors.JSON.Contains( errorUnrecognized ) ) {
				LogToConsole( "Unrecognized username or password.  Register?", failureColor );
				animator.SetBool( "Register", true );
			}
			else if( response.Errors.JSON.Contains( errorNoNetwork ) ) {
				LogToConsole( "No network access.  Online features disabled.", failureColor );
			}
			else {
				// report unrecognized errors directly to the user
				LogToConsole( "Couldn't log in.  Reason: " + response.Errors.JSON, failureColor );
			}
		}

		spinner.animator.SetBool( "Toggle", false );
	}

	void FindLastLevel() {
		// generate request for last level on server and load it
		// note how we have to define the anonymous method before we use it, so it looks like the logic is backwards here
		Action<int[]> onFetchedListFromServer = list => {
			int levelToLoad = firstLevel;
			if( list != null ) {
				levelToLoad = list[0] + LoadLevel.levelOffset;
			}

			Backend.manager.lastLevelPlayed = levelToLoad; // cache it for reference
			levelLoader.Load( firstLevel ); // load whichever level is first
		};

		// initiate the request for the latest level this player has played
		Backend.manager.GetCloudInts( new string[] { "lastLevelPlayed" }, onFetchedListFromServer );
	}

	// a way to report backend responses to the player in a friendly way
	void LogToConsole( string msg, Color col ) {
		responseLabel.color = col;
		responseLabel.text = msg;

		animator.SetTrigger( "Response Bounce" );
	}

	void ShowSpinner() {
		if( spinner ) Destroy( spinner.gameObject );

		spinner = Instantiate<Builder>( spinnerPrefab );
		spinner.transform.SetParent( spinnerParent, false );
		spinner.transform.localPosition = Vector3.zero;
	}

	public void CancelRegistration() {
		animator.SetBool( "Register", false );
		responseLabel.text = "";
	}

	bool CheckPasswordsMatch() {
		if( pwdLabel.text != pwdLabel2.text ) {
			LogToConsole( "Passwords do not match...", failureColor );

			return false;
		}

		return true;
	}
}
