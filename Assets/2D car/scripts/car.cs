using UnityEngine;
using System.Collections;

public class car : MonoBehaviour {
	WheelJoint2D kolesoZ ;
	WheelJoint2D[] kolesa;
	WheelJoint2D kolesoP ;
	Camera cam;
	GasTank tank;

	//maximalna rychlost
	int maxMotorSpeed = 4000;
	//Uroven akceleracie
	int acceleration = 1;

	float OLDX = 0;
	float OLDY = 0;
	float rozdil =0;
	float sucetDelta= 0;
	float sucetRozdil = 0;
	float neutral = 0.2f;


	//bahno z kolesa
	ParticleSystem bahno;
	ParticleSystem bahno_dym_zadne;
	ParticleSystem bahno_dym_predne;
	//ground contact
	public static bool groundContact = false;
	public static bool wheel_smoke = false;

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

		kolesa = GetComponents<WheelJoint2D>();
		kolesoZ =kolesa[0];
		kolesoP = kolesa[1];
		bahno =(ParticleSystem) GameObject.Find("bahno").particleSystem;
		bahno_dym_zadne =(ParticleSystem) GameObject.Find("bahno_dym").particleSystem;

		bahno_dym_predne =(ParticleSystem) Instantiate(bahno_dym_zadne);
		bahno.enableEmission = false;
		bahno_dym_zadne.enableEmission = false;
		bahno_dym_predne.enableEmission = false;

		this.tank = new GasTank ();
		this.tank.setMaxFill (10000);
	}

	// Update is called once per frame
	void Update () {

		JointMotor2D mot = kolesoZ.motor;
		//float priemer = 0.95f;
		float uhlova_rychlost = kolesoZ.jointSpeed;
		float rozdilX = this.transform.position.x - OLDX;
		float rozdilY = this.transform.position.y - OLDY;

		Rigidbody2D koleso_zadne = GameObject.Find("koleso_zadne").rigidbody2D;
		Rigidbody2D koleso_predne = GameObject.Find("koleso_predne").rigidbody2D;
		
		bahno.transform.position = new Vector3(koleso_zadne.transform.position.x, koleso_zadne.transform.position.y - 0.2f,koleso_zadne.transform.position.z);
		bahno_dym_predne.transform.position = new Vector3(koleso_predne.transform.position.x, koleso_predne.transform.position.y - 0.2f,koleso_predne.transform.position.z);




		rozdil = Mathf.Sqrt (kolesoZ.rigidbody2D.velocity.x  * kolesoZ.rigidbody2D.velocity.x  + kolesoZ.rigidbody2D.velocity.y  * kolesoZ.rigidbody2D.velocity.y );
		sucetDelta += Time.deltaTime;
		sucetRozdil += rozdil;
		float smyk = 0;
		if (Time.deltaTime > 0.0001f) {
			rozdil = sucetRozdil/sucetDelta;
						float vysledek = kolesoZ.jointSpeed / rozdil;
						sucetDelta = 0;
						sucetRozdil=0;


			//if(  Mathf.Abs(vysledek) > 180 && Mathf.Abs(vysledek) < 1000000 && groundContact){
			smyk = Mathf.Abs(kolesoZ.jointSpeed / Mathf.Sqrt (kolesoZ.rigidbody2D.velocity.x * kolesoZ.rigidbody2D.velocity.x + kolesoZ.rigidbody2D.velocity.y * kolesoZ.rigidbody2D.velocity.y));
			if ( ( smyk > 234 || smyk < 114 || wheel_smoke) && groundContact && Mathf.Abs(kolesoZ.rigidbody2D.velocity.x) > 0.1f ) {
				bahno.enableEmission=true;
				bahno_dym_zadne.enableEmission=true;
				bahno_dym_predne.enableEmission=true;
			}
			else{
				bahno.enableEmission=false;
				bahno_dym_zadne.enableEmission=false;
				bahno_dym_predne.enableEmission=false;
			}

			//gui.setValue(kolesoZ.jointSpeed.ToString() +"\n"+ bodkovica +"\n  "+ vysledek +" rozdil:"+rozdil.ToString()+" rozdilX:"+rozdilX.ToString()+"rozdilY:"+rozdilY.ToString()+"\n delta :"+Time.deltaTime.ToString());
		}
		
		OLDX = this.transform.position.x;
		OLDY = this.transform.position.y;

		float oldSpeed = mot.motorSpeed;
		float newSpeed; // nastaveni rychlosti motora
		float newTorque; // nastaveni zaberu motora

		float direction = Input.GetAxis ("Vertical");   //zistenie smeru chodu

		// nie je stlacene ziadne tlacitko
		if (direction == 0) {
			// vypnem tah motora (ala neutral)
			newSpeed = oldSpeed;
			newTorque = 0;

			// vypnutie brzdy na prednom kolese
			kolesoP.useMotor = false;
		}
		// uzivatel stlaca plyn a auto ide do opacneho smeru
		else if(direction * kolesoZ.rigidbody2D.velocity.x > 0 ) {
			newSpeed = direction * maxMotorSpeed;

			// zapnem tah motora
			newTorque = 50;

			// nulova rychlost motora = brzda
			newSpeed = 0;

			JointMotor2D predne = kolesoP.motor;
			predne.motorSpeed = 0;
			kolesoP.useMotor = true;
			kolesoP.motor = predne;
		}
		// uzivatel stlaca plyn a auto ide do rovnakeho smeru
		else {
			newSpeed = direction * maxMotorSpeed;

			if( Mathf.Abs(newSpeed) < Mathf.Abs(mot.motorSpeed) ) {
				newSpeed = mot.motorSpeed;
			}

			// zapnem tah motora
			newTorque = 50;

			// vypnutie brzdy na prednom kolese
			kolesoP.useMotor = false;	
		}

		// spotreba beniznu + kontrola prazdnosti nadrze
		if (!this.tank.use (Mathf.Abs(direction) + this.neutral)) {
			// vypnem tah motora (ala neutral)
			newSpeed = oldSpeed;

			newTorque = 0;
		}
		// nadrz nie je prazdna
		else {
			mot.motorSpeed = newSpeed;
			mot.maxMotorTorque = newTorque;
			kolesoZ.motor = mot;
		}



		gui.setValue (
			"Contact:" + groundContact.ToString() + "\n"
			+ "direction:" + direction.ToString() + "\n"
			+ "newSpeed:" + newSpeed.ToString() + "\n"
			+ "newTorque:" + newTorque.ToString() + "\n"
			+ "kolesoZ.rigidbody2D.velocity.x:" + kolesoZ.rigidbody2D.velocity.x.ToString() + "\n"
			+ "oldSpeed:" + oldSpeed.ToString() + "\n"
			+ "smyk:" + smyk.ToString() + "\n"
			+ "Speed:" + mot.motorSpeed.ToString() + "\n"
			+ "Tank:" + this.tank.getCurrentFill() + "/" + this.tank.getMaxFill() + "\n"
			+ "Health:" + this.getHealth()
		);


		if (Mathf.Abs(kolesoZ.rigidbody2D.velocity.x) > 5 && groundContact) {
			bahno_dym_zadne.enableEmission=true;
			bahno_dym_predne.enableEmission=true;
		}
		else{
			bahno_dym_zadne.enableEmission=false;
			bahno_dym_predne.enableEmission=false;
		}
	}
}
