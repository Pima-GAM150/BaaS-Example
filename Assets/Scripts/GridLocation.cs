using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridLocation : Location {

	public int numClicks { get; set; }

	public override void OnPointerDown( PointerEventData evt ) {
		if( GridManager.data.levelComplete ) return;
		if( GridManager.data.locked ) return;

		GridManager.data.UserClickedOn( this );

		if( numClicks == 1 ) StatTracker.stats.GridLocColorFlipped();
		numClicks++;
	}
}
