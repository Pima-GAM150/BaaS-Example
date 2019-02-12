using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotCamera : MonoBehaviour {

	public Transform cameraToRotate;
	public Transform pivotPoint;

	public float speed = 1f;

	Quaternion origRot;
	Vector3 clickedInput;

	void Update() {
		if( Input.GetMouseButtonDown(0) ) {
			clickedInput = Input.mousePosition;
			origRot = pivotPoint.rotation;
		}

		if( Input.GetMouseButton(0) ) {
			Vector3 delta = Input.mousePosition - clickedInput;
			Vector3 delaSwappedAxes = new Vector3( -delta.y, delta.x, delta.z );

			pivotPoint.rotation = origRot * Quaternion.Euler( delaSwappedAxes * speed );
		}
	}
}
