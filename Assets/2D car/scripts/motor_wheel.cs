using UnityEngine;
using System.Collections;

public class motor_wheel : MonoBehaviour {

	public int wheelId;

	void OnCollisionEnter2D(Collision2D col)	{
		//return;
		if (col.gameObject.name == "GrassThinSprite") {
			// nastavim priznak kolizie na kolese
			//car.wheels[wheelId].updateGroundContact(true);
			
			if(col.relativeVelocity.magnitude > 5){
				car.wheels [wheelId].updateSmoking (true);
			}
			else{
				car.wheels [wheelId].updateSmoking (false);
			}
			
		}
	}

	void OnCollisionExit2D(Collision2D col)	{
		//return;
		if (col.gameObject.name == "GrassThinSprite") {
			// nastavim priznak kolizie na kolese
			car.wheels [wheelId].updateGroundContact (false);
		}
		car.wheels [wheelId].updateSmoking (false);
	}
	
	void OnCollisionStay2D(Collision2D col)	{
		//return;
		if (col.gameObject.name == "GrassThinSprite") {
			// nastavim priznak kolizie na kolese
			car.wheels[wheelId].updateGroundContact (true);
		}
	}
}
