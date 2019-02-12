using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
	public MovementPath[] nodes;
	public GridLocation[] moveableLocations;

	public float moveSpeed = 1f;

	public void MoveAllNext() {
		foreach( GridLocation loc in moveableLocations ) {
			GridManager.data.locks++;
			LeanTween.move( loc.gameObject, GetClosestNode( loc ).next.transform.position, moveSpeed ).setOnComplete( () => GridManager.data.locks-- );
		}
	}

	public MovementPath GetClosestNode( GridLocation loc ) {
		float leastDistance = Mathf.Infinity;
		MovementPath closestNode = null;
		foreach( MovementPath node in nodes ) {
			float dist = (node.transform.position - loc.transform.position).sqrMagnitude;

			if( dist < leastDistance ) {
				leastDistance = dist;
				closestNode = node;
			}
		}

		return closestNode;
	}
}
