using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {

	public const int levelOffset = 2;

	public AsyncOperation loadOp;
	int _currentlyLoadingLevel;

	public void Load( int levelToLoad ) {

		if( _currentlyLoadingLevel == levelToLoad ) {
			loadOp.allowSceneActivation = true;
		}
		else {
			NotifyStats( levelToLoad );
			SceneManager.LoadScene( levelToLoad );
		}
	}

	public void LoadNext() {
		LoadAsync( SceneManager.GetActiveScene().buildIndex + 1 );
	}

	public void Reload() {
		LoadAsync( SceneManager.GetActiveScene().buildIndex );
	}

	public void LoadAsyncFromEvent( int levelToLoad ) {
		LoadAsync( levelToLoad );
	}

	public void LoadAsync( int levelToLoad, bool loadImmediately = true ) {
		NotifyStats( levelToLoad );

		loadOp = SceneManager.LoadSceneAsync( levelToLoad );
		loadOp.allowSceneActivation = loadImmediately;

		_currentlyLoadingLevel = levelToLoad;
	}

	public void LoadLastSavedLevel() {
		LoadAsync( Backend.manager.lastLevelPlayed );
	}

	void NotifyStats( int nextLevel ) {
		if( StatTracker.stats ) StatTracker.stats.SceneChanged( SceneManager.GetActiveScene().buildIndex, nextLevel );
	}
}
