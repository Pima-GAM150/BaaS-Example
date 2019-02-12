using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Location : MonoBehaviour, IPointerDownHandler {

	const float colorFadeSpeed = 0.2f;

	public MeshRenderer meshRenderer;
	public Color color = Color.white;

	Color currentColor;

	void Start() {
		currentColor = color;
		SetColor( currentColor );
	}

	void Update() {
		if( currentColor != color ) {
			currentColor = Color.Lerp( currentColor, color, colorFadeSpeed );
			SetColor( currentColor );
		}
	}

	public void SetColor( Color newColor ) {
		MaterialPropertyBlock matProps = new MaterialPropertyBlock();
		matProps.SetColor( "_Color", currentColor );
		meshRenderer.SetPropertyBlock( matProps );
	}

	public virtual void OnPointerDown( PointerEventData evt ) {}
}
