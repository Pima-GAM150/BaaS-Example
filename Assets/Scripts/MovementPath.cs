using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPath : MonoBehaviour
{
	public MovementPath next;

	void OnDrawGizmos() {
		if( next ) {
			Gizmos.color = Color.blue;
			Gizmos.DrawLine( transform.position, next.transform.position );
		}
	}
}
