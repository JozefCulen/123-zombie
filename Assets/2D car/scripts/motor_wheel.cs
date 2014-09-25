using UnityEngine;
using System.Collections;

public class motor_wheel : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.name == "GrassThinSprite")
						car.groundContact = true;
	}
	void OnCollisionExit2D(Collision2D col){
		if (col.gameObject.name == "GrassThinSprite")
			car.groundContact = false;
	}

}
