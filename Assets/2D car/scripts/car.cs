using UnityEngine;
using System.Collections;

public class car : MonoBehaviour {
	WheelJoint2D kolesoZ ;
	Camera cam;
	GasTank tank;

	//maximalna rychlost
	int maxMotorSpeed = 4000;
	//Uroven akceleracie
	int acceleration = 100;

	float OLDX = 0;
	float OLDY = 0;
	float rozdil =0;
	float sucetDelta= 0;
	float sucetRozdil = 0;

	float neutral = 0.2f;


	//bahno z kolesa
	ParticleSystem bahno;
	//ground contact
	public static bool groundContact = false;
	protected float health;
	
	void OnCollisionEnter2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite") {
			this.makeDamage(col.relativeVelocity.magnitude);
		}
	}

	void makeDamage(float v)	{
		this.health -= v;
	}
	
	float getHealth()	{
		return this.health;
	}


	void Start () {
		this.health = 1000;

		kolesoZ = GetComponent<WheelJoint2D>();
		bahno =(ParticleSystem) GameObject.Find("bahno").particleSystem;

		bahno.enableEmission = false;

		this.tank = new GasTank ();
		this.tank.setMaxFill (1000);
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

		float direction = Input.GetAxis ("Vertical") * (-1);   //divne invertovane

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

		if (!this.tank.use (Mathf.Abs(direction) + this.neutral)) {
			newSpeed = 0;

			kolesoZ.useMotor = false;
		}
		else {
			mot.motorSpeed = newSpeed;
			kolesoZ.motor = mot;

			kolesoZ.useMotor = true;
		}

		/*
		if (Input.GetKey (KeyCode.R)) {
			this.transform.position = new Vector3(this.transform.position.x-0.1f , this.transform.position.y + 0.5f, this.transform.position.z);
		}
		*/

		gui.setValue (
			"Contact:" + groundContact.ToString() + "\n"
			+ "Speed:" + mot.motorSpeed.ToString() + "\n"
			+ "Tank:" + this.tank.getCurrentFill() + "/" + this.tank.getMaxFill() + "\n"
			+ "Health:" + this.getHealth()
		);
		
	}
}
