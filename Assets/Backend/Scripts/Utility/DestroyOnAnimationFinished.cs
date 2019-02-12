using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAnimationFinished : MonoBehaviour {

	public void Finished() {
		Destroy( gameObject );
	}
}
