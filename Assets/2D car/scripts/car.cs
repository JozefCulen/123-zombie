using UnityEngine;
using System.Collections;

public class car : MonoBehaviour {
	WheelJoint2D kolesoZ ;
	Camera cam;
	 int maxMotorSpeed = 2000;
	int acceleration = 500;

	// Use this for initialization
	void Start () {
		kolesoZ = GetComponent<WheelJoint2D>();

		Camera.main.guiText.text = "hovado";
	}
	
	// Update is called once per frame
	void Update () {

		JointMotor2D mot = kolesoZ.motor;
		if (Input.anyKeyDown) {
			//Debug.Log("stalcam");
			Debug.Log(mot.motorSpeed);
		}
		if (Input.GetKey(KeyCode.LeftArrow)) {
			if (mot.motorSpeed <= maxMotorSpeed){
				kolesoZ.useMotor = true;
				if(mot.motorSpeed >= maxMotorSpeed )
					mot.motorSpeed = maxMotorSpeed;
				else
					mot.motorSpeed += acceleration;
				kolesoZ.motor = mot;
			}
		}
		else if (Input.GetKey(KeyCode.RightArrow)) {
			if (mot.motorSpeed >= -maxMotorSpeed){
				kolesoZ.useMotor = true;
				if(mot.motorSpeed <= -maxMotorSpeed )
					mot.motorSpeed = -maxMotorSpeed;
				else
					mot.motorSpeed -= acceleration;
				kolesoZ.motor = mot;
			}
		}
		else{
			if(kolesoZ.useMotor == true){
				kolesoZ.useMotor = false;
			//Debug.Log ("motor disabled");
			}
		}
	}
}
