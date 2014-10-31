using UnityEngine;
using System.Collections;

public class WheelCollision : MonoBehaviour {
	public car carInstance;
	public int wheelId;

	void OnCollisionEnter2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite") {

			// nastavim priznak kolizie na kolese
			this.carInstance.getWheel(wheelId).UpdateGroundContact(true);

			// odoslem informaciu o velkosti kolizie
			this.carInstance.getWheel(wheelId).UpdateFallMagnitude (col.relativeVelocity.magnitude);
		}
	}
	
	void OnCollisionExit2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite") {
			// nastavim priznak kolizie na kolese
			this.carInstance.getWheel(wheelId).UpdateGroundContact (false);
		}
	}
	
	void OnCollisionStay2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite") {
			// nastavim priznak kolizie na kolese
			this.carInstance.getWheel(wheelId).UpdateGroundContact (true);
		}
	}

}
