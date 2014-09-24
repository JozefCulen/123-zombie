using UnityEngine;
using System.Collections;

public class car : MonoBehaviour {
	WheelJoint2D kolesoZ ;
	Camera cam;

	//maximalna rychlost
	int maxMotorSpeed = 2000;
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
		/*float priemer = 0.95f;
		float uhlova_rychlost = kolesoZ.jointSpeed;
		float rozdilX = this.transform.position.x - OLDX;
		float rozdilY = this.transform.position.y - OLDY;
		
		rozdil = Mathf.Sqrt (rozdilX * rozdilX + rozdilY * rozdilY);
		sucetDelta += Time.deltaTime;
		sucetRozdil += rozdil;
		if (Time.deltaTime > 0.005f) {
			rozdil = sucetRozdil/sucetDelta;
						float vysledek = kolesoZ.jointSpeed / rozdil;
						sucetDelta = 0;
						sucetRozdil=0;
			int bodky = (int) Mathf.Abs(vysledek)/20;
			string bodkovica ="";
			//for (int i = 0; i < bodky; i++)
			//	bodkovica += "+";

			/*GameObject dym = gameObject.GetComponent("Cube");
			if(Mathf.Abs(vysledek) > 250){

			}
			else
				dym.enableEmission=false;
			//gui.setValue(kolesoZ.jointSpeed.ToString() +"\n"+ bodkovica +"\n  "+ vysledek +" rozdil:"+rozdil.ToString()+" rozdilX:"+rozdilX.ToString()+"rozdilY:"+rozdilY.ToString()+"\n delta :"+Time.deltaTime.ToString());

		}
		
		OLDX = this.transform.position.x;
		OLDY = this.transform.position.y;
		*/
		//float vysledek = 
		gui.setValue (groundContact.ToString()+"\n"+mot.motorSpeed.ToString());
		if (Input.GetKey(KeyCode.LeftArrow)) {

			if (mot.motorSpeed <= maxMotorSpeed){
				kolesoZ.useMotor = true;
				if(mot.motorSpeed < 0)
					mot.motorSpeed = 0;
				if(mot.motorSpeed >= maxMotorSpeed )
					mot.motorSpeed = maxMotorSpeed;
				else{
					mot.motorSpeed += acceleration;
				}
				kolesoZ.motor = mot;
				if(/*kolesoZ.jointSpeed < maxMotorSpeed &&*/ groundContact){
					bahno.transform.rotation = Quaternion.Euler(200,0,0);
					bahno.enableEmission = true;
				}
				else
					bahno.enableEmission = false;

			}
		}
		else if (Input.GetKey(KeyCode.RightArrow)) {
			if (mot.motorSpeed >= -maxMotorSpeed){
				kolesoZ.useMotor = true;
				if(mot.motorSpeed > 0)
					mot.motorSpeed = 0;
				if(mot.motorSpeed <= -maxMotorSpeed )
					mot.motorSpeed = -maxMotorSpeed;
				else{
					mot.motorSpeed -= acceleration;
				}
				kolesoZ.motor = mot;
				if(/*kolesoZ.jointSpeed > -maxMotorSpeed &&*/ groundContact){
					bahno.enableEmission = true;
				}
				else
					bahno.enableEmission = false;
			}
		}
		else{
			bahno.transform.rotation = Quaternion.Euler(330,0,0);
			bahno.enableEmission = false;

			if(kolesoZ.useMotor == true){
				kolesoZ.useMotor = false;
			}
		}

		if (Input.GetKey (KeyCode.R)) {
			this.transform.position = new Vector3(this.transform.position.x-0.1f , this.transform.position.y + 0.5f, this.transform.position.z);
		}
	}
}
