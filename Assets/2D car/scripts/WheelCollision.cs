using UnityEngine;
using System.Collections;

public class WheelCollision : MonoBehaviour {

	public int wheelId;

	void OnCollisionEnter2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite") {
			// nastavim priznak kolizie na kolese
			car.getWheel(wheelId).UpdateGroundContact(true);

			// odoslem informaciu o velkosti kolizie
			car.getWheel(wheelId).UpdateFallMagnitude (col.relativeVelocity.magnitude);
		}
	}
	
	void OnCollisionExit2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite") {
			// nastavim priznak kolizie na kolese
			car.getWheel(wheelId).UpdateGroundContact (false);
		}
	}
	
	void OnCollisionStay2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite") {
			// nastavim priznak kolizie na kolese
			car.getWheel(wheelId).UpdateGroundContact (true);
		}
	}
}
