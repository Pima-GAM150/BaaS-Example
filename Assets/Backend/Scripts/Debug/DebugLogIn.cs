using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugLogIn : MonoBehaviour {

	public LoginToBackend backend;
	public float delay;
	public string username;
	public string password;

	IEnumerator Start() {
		yield return new WaitForSeconds( delay );
		backend.Login( username, password );
	}
}
