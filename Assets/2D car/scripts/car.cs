using UnityEngine;
using System.Collections;

public class car : MonoBehaviour {
	WheelJoint2D kolesoZ ;
	Camera cam;

	//maximalna rychlost
	int maxMotorSpeed = 4000;
	//Uroven akceleracie
	int acceleration = 100;

	float OLDX = 0;
	float OLDY = 0;
	float rozdil =0;
	float sucetDelta= 0;
	float sucetRozdil = 0;


	//bahno z kolesa
	ParticleSystem bahno;
	//ground contact
	public static bool groundContact = false;

	void Start () {
		kolesoZ = GetComponent<WheelJoint2D>();
		bahno =(ParticleSystem) GameObject.Find("bahno").particleSystem;

		bahno.enableEmission = false;

	}

	// Update is called once per frame
	void Update () {

		JointMotor2D mot = kolesoZ.motor;
		//float priemer = 0.95f;
		float uhlova_rychlost = kolesoZ.jointSpeed;
		float rozdilX = this.transform.position.x - OLDX;
		float rozdilY = this.transform.position.y - OLDY;
		
		rozdil = Mathf.Sqrt (rozdilX * rozdilX + rozdilY * rozdilY);
		sucetDelta += Time.deltaTime;
		sucetRozdil += rozdil;
		if (Time.deltaTime > 0.001f) {
			rozdil = sucetRozdil/sucetDelta;
						float vysledek = kolesoZ.jointSpeed / rozdil;
						sucetDelta = 0;
						sucetRozdil=0;
			int bodky = (int) Mathf.Abs(vysledek)/20;
			//string bodkovica ="";
			//for (int i = 0; i < bodky; i++)
			//	bodkovica += "+";


			if(Mathf.Abs(vysledek) > 190 && Mathf.Abs(vysledek) < 1000000 && groundContact){
				bahno.enableEmission=true;
			}
			else{
				bahno.enableEmission=false;
				}
			//gui.setValue(kolesoZ.jointSpeed.ToString() +"\n"+ bodkovica +"\n  "+ vysledek +" rozdil:"+rozdil.ToString()+" rozdilX:"+rozdilX.ToString()+"rozdilY:"+rozdilY.ToString()+"\n delta :"+Time.deltaTime.ToString());

		}
		
		OLDX = this.transform.position.x;
		OLDY = this.transform.position.y;

		//float vysledek =

		float direction = Input.GetAxis ("Horizontal") * (-1);   //divne invertovane

		float newSpeed = mot.motorSpeed;
		if (newSpeed * direction < 0)	{ //(newSpeed < 0 && direction > 0) || (newSpeed > 0 && direction < 0)) 
			newSpeed = 0;
		}
		newSpeed = newSpeed + direction * acceleration;

		if (newSpeed > maxMotorSpeed) {
			newSpeed = maxMotorSpeed;
		} 
		else if (newSpeed < -maxMotorSpeed) {
			newSpeed = -maxMotorSpeed;
		}
		
		mot.motorSpeed = newSpeed;
		kolesoZ.motor = mot;

		if (direction > 0) {
			kolesoZ.useMotor = true;
			//bahno.transform.rotation = Quaternion.Euler(330,0,0);
		}
		else if (direction < 0) {
			kolesoZ.useMotor = true;
			//bahno.transform.rotation = Quaternion.Euler(200,0,0);
		}
		else {
			kolesoZ.useMotor = false;
		}

		/*
		if (Input.GetKey (KeyCode.R)) {
			this.transform.position = new Vector3(this.transform.position.x-0.1f , this.transform.position.y + 0.5f, this.transform.position.z);
		}
		*/

		gui.setValue (groundContact.ToString()+"\n"+mot.motorSpeed.ToString());
		
	}
}
