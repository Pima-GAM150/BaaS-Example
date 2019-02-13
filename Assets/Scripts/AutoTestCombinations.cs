using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTestCombinations : MonoBehaviour
{
	public GridManager manager;

	public int iterations;

	void Start() {
		for( int index = 0; index < iterations; index++ ) {
			foreach( GridLocation loc in manager.allGridLocations ) {
				loc.color = manager.colors[ Random.Range( 0, manager.colors.Count ) ];
			}

			manager.CheckForPatterns();

			if( manager.won ) {
				return;
			}
		}
	}
}
