using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetSavedProgress : MonoBehaviour
{
	public void Reset() {
		Backend.manager.lastLevelPlayed = LoadLevel.levelOffset;
		Backend.manager.SetCloudData(
			new IntAttribute[] { new IntAttribute { key = "lastLevelPlayed", val = LoadLevel.levelOffset } }
		);
	}
}
