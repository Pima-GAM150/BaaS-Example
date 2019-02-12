using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour {

	readonly Color matchedColor = Color.yellow;

	public GridLocation[] allGridLocations { get; set; }

	public int numberInARowToLose = 3;

	public List<Color> colors = new List<Color>();
	int currentColIndex = 0;

	public Location loseSymbol;
	public Location winSymbol;
	public Location restartSymbol;
	public Location continueSymbol;

	public float neighborDistance;
	public float dotThreshold;
	public float tickRate;

	public int locks { get; set; }

	public static GridManager data;

	public UnityEvent clickedLocationEvent;

	void Awake() {
		data = this;

		allGridLocations = GetComponentsInChildren<GridLocation>();
	}

	void Start() {
		StartCoroutine( Tick() );
	}

	public void UserClickedOn( GridLocation clickedLocation ) {
		SwitchColors();
		clickedLocation.color = colors[currentColIndex];

		clickedLocationEvent.Invoke();
	}

	IEnumerator Tick() {
		while( !levelComplete ) {
			CheckForPatterns();

			yield return new WaitForSeconds( tickRate );
		}
	}

	void SwitchColors() {
		currentColIndex = (currentColIndex + 1) % colors.Count;
	}

	public void CheckForPatterns() {

		int numLocationsFilled = 0;

		foreach( GridLocation thisGridLocation in allGridLocations ) {
			Color thisGridColor = thisGridLocation.color;

			if( colors.Contains( thisGridColor ) ) { // has the box been filled?
				numLocationsFilled++;

				List<GridLocation> neighbors = GetNeighbors( thisGridLocation );

				foreach( GridLocation neighbor in neighbors ) {
					if( neighbor.color == thisGridColor ) { // if one of the neighbors has the same material as this one
						Vector3 dirToNeighbor = (neighbor.transform.position - thisGridLocation.transform.position).normalized;

						RowPattern initialPattern = new RowPattern { locations = new List<GridLocation> { thisGridLocation } };
						RowPattern pattern = HowManyInARow( thisGridLocation, dirToNeighbor, thisGridColor, initialPattern );

						if( pattern.locations.Count >= numberInARowToLose ) {
							foreach( GridLocation matchedLoc in pattern.locations ) matchedLoc.color = matchedColor;
							for( int index = 0; index < pattern.locations.Count - 1; index++ ) {
								Vector3 origin = pattern.locations[index].transform.position;
								Vector3 dest = pattern.locations[index + 1].transform.position;
							}

							loseSymbol.gameObject.SetActive( true );
							restartSymbol.gameObject.SetActive( true );
							loseSymbol.color = thisGridColor;
							LeanTween.cancelAll();
							return;
						}
					}
				}
			}
		}

		if( numLocationsFilled == allGridLocations.Length ) { // if all grid locations have been filled
			if( winSymbol.gameObject.activeInHierarchy == false ) { // if we haven't already won yet
				winSymbol.gameObject.SetActive( true );
				continueSymbol.gameObject.SetActive( true );
				LeanTween.cancelAll();
			}
		}
	}

	List<GridLocation> GetNeighbors( GridLocation location ) {
		List<GridLocation> neighbors = new List<GridLocation>();

		foreach( GridLocation gridLocation in allGridLocations ) {
			if( gridLocation == location ) continue;
			if( gridLocation.color == Color.white ) continue;
			if( gridLocation.color == matchedColor ) continue;

			float sqrDist = (gridLocation.transform.position - location.transform.position).sqrMagnitude;
			if( sqrDist > neighborDistance ) continue;

			neighbors.Add( gridLocation );
		}

		return neighbors;
	}

	RowPattern HowManyInARow( GridLocation gridLocation, Vector3 dirOfPattern, Color colorToCheck, RowPattern pattern ) {
		foreach( GridLocation neighbor in GetNeighbors( gridLocation ) ) {
			if( neighbor.color == colorToCheck ) { // does the neighbor have the same color as the last one?
				Vector3 dirToNeighbor = Vector3.Normalize( neighbor.transform.position - gridLocation.transform.position );
				// Vector3 dirOfPattern = Vector3.Normalize( neighbor.transform.position - pattern.locations[0].transform.position );

				float dot = Vector3.Dot( dirOfPattern, dirToNeighbor );
				if( dot > dotThreshold ) { // if the two directions point in approximately the same direction
					pattern.locations.Add( neighbor );

					return HowManyInARow( neighbor, dirToNeighbor, colorToCheck, pattern );
				}
			}
		}

		return pattern;
	}

	class RowPattern {
		public List<GridLocation> locations;
	}

	public bool levelComplete { get { return won || lost; } }
	public bool won { get { return winSymbol.gameObject.activeInHierarchy; } }
	public bool lost { get { return loseSymbol.gameObject.activeInHierarchy; } }
	public bool locked { get { return locks > 0; } }
}
