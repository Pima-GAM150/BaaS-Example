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

	public void Register( string displayName, string username, string password ) {
		if( username == "" || password == "" || displayLabel.text == "" ) return;

		if( CheckPasswordsMatch() ) {
			print("Sending registration response... ");
			new RegistrationRequest().SetDisplayName( displayName ).SetUserName( username ).SetPassword( password ).Send( RegisterResponse );
		}
	}
	public void Register() { Register( displayLabel.text, usernameLabel.text, pwdLabel.text ); }

	public void RegisterResponse( RegistrationResponse response ) {
		if( !response.HasErrors ) {

			Backend.manager.displayName = response.DisplayName;

			LogToConsole( "Player " + response.DisplayName + " created!", successColor );
			if( levelLoader ) levelLoader.Load( firstLevel );
		}
		else {
			LogToConsole( "Error Authenticating Player: " + response.Errors.JSON, failureColor );
		}
	}

	public void Login( string username, string password ) {
		if( username == "" || password == "" ) return;

		spinner = Instantiate<Builder>( spinnerPrefab );
		spinner.transform.SetParent( spinnerParent );
		spinner.transform.position = Vector3.zero;
		
		// spinner.animator.SetBool( "Toggle", true );

		new AuthenticationRequest().SetUserName( username ).SetPassword( password ).Send( LoginResponse );
	}
	public void Login() { Login( usernameLabel.text, pwdLabel.text ); }

	public void LoginResponse( AuthenticationResponse response ) {
		if( !response.HasErrors ) {

			Backend.manager.displayName = response.DisplayName;

			LogToConsole( "Welcome, " + response.DisplayName + "!", successColor );

			if( levelLoader ) FindLastLevel();
		}
		else {			

			print("Login response had errors: " + response.Errors.JSON );
			if( response.Errors.JSON.Contains( errorUnrecognized ) ) {
				LogToConsole( "Unrecognized username or password.  Register?", failureColor );
				animator.SetBool( "Register", true );
			}
			else if( response.Errors.JSON.Contains( errorNoNetwork ) ) {
				LogToConsole( "No network access.  Online features disabled.", failureColor );
			}
			else {
				LogToConsole( "Couldn't log in.  Reason: " + response.Errors.JSON, failureColor );
			}
		}

		spinner.animator.SetBool( "Toggle", false );
	}

	void FindLastLevel() {
		// generate request for last level on server and load it
		Action<int[]> onFetchedListFromServer = list => {
			int levelToLoad = firstLevel;
			if( list != null ) {
				levelToLoad = list[0] + LoadLevel.levelOffset;
			}

			levelLoader.Load( levelToLoad );
		};

		Backend.manager.GetCloudInts( new string[] { "lastLevelPlayed" }, onFetchedListFromServer );
	}

	void LogToConsole( string msg, Color col ) {
		responseLabel.color = col;
		responseLabel.text = msg;

		animator.SetTrigger( "Response Bounce" );
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
