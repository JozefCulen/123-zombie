using UnityEngine;
using System.Collections;

public class motor_wheel : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite") {
			car.groundContact = true;
			if(col.relativeVelocity.magnitude > 5){
				car.wheel_smoke = true;
			}
			else{
				car.wheel_smoke = false;
			}
			
		}
	}

	void OnCollisionExit2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite")
			car.groundContact = false;
		car.wheel_smoke = false;
	}
	
	void OnCollisionStay2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite")
			car.groundContact = true;
			
	}
}
