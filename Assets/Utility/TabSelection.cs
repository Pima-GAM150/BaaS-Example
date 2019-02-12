using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TabSelection : MonoBehaviour {

	public Selectable startingSelection;
	public Selectable endingSelection;

	EventSystem _system;

	public UnityEvent onFinished = new UnityEvent();

	void Start() {
		_system = EventSystem.current;

		startingSelection.Select();
	}

	public void Update() {
		if( Input.GetKeyDown( KeyCode.Tab ) ) {
			Selectable selected = _system.currentSelectedGameObject.GetComponent<Selectable>();

			if( selected ) {
				Selectable next = selected.FindSelectableOnDown();

				if( next ) {
					next.Select();
				}
			}
		}

		if( Input.GetKeyDown( KeyCode.Return ) ) {
			if( _system.currentSelectedGameObject == endingSelection.gameObject ) {
				onFinished.Invoke();
			}
		}
	}
}
