using UnityEngine;
using System.Collections;

public class Wheel 
{
	public WheelJoint2D joint;
	public bool powered;
	public ParticleSystem dirt;
	public ParticleSystem smoke;

	private bool sliding;
	private bool groundContact;
	private bool smoking;

	public Wheel(WheelJoint2D jointObject)
	{
		this.joint = jointObject;
		this.powered = false;
		this.dirt = null;
		this.sliding = false;
		this.groundContact = false;
		this.smoking = false;
	}

	public void updateParticlesPositions() {
		if (dirt != null) {
			dirt.transform.position = new Vector3 (joint.connectedBody.transform.position.x, joint.connectedBody.transform.position.y - 0.2f, joint.connectedBody.transform.position.z - 5);
		}
		smoke.transform.position = new Vector3(joint.connectedBody.transform.position.x, joint.connectedBody.transform.position.y - 0.2f,joint.connectedBody.transform.position.z - 5);
	}

	public bool isSliding() {

		return sliding;
	}

	public bool slidingDetection() {
		float skidConstant = 174;
		float skidValue; // absolutna hodnota smyku
		float traveledDistance; // ujeta vzdialenost auta

		// ujeta vzdialenost auta sa pocita na zaklade rychlosti po suranici X a Y
		traveledDistance = Mathf.Sqrt (joint.rigidbody2D.velocity.x * joint.rigidbody2D.velocity.x + joint.rigidbody2D.velocity.y * joint.rigidbody2D.velocity.y);

		// vypocet urovne smyku
		skidValue = Mathf.Abs(joint.jointSpeed / traveledDistance );
		if ( ( skidValue > skidConstant + 60 || skidValue < skidConstant - 60 ) && this.groundContact && Mathf.Abs(joint.rigidbody2D.velocity.x) > 0.1f && traveledDistance > 0.1f) {
			this.sliding = true;
		}
		else{
			this.sliding = false;
		}

		this.smoke.enableEmission = this.sliding;

		if (this.dirt != null) {
			this.dirt.enableEmission = this.sliding;
		}
		return this.sliding;
	}

	public void updateSpeed() {
		if( this.powered == false) {
			if( car.breaking == true ) {
				JointMotor2D newMotor = this.joint.motor;
				newMotor.motorSpeed = 0;

				this.joint.useMotor = true;
				this.joint.motor = newMotor;
			}
			else {
				this.joint.useMotor = false;
			}
		}
		else {
			int maxMotorSpeed = 4000;
			float oldSpeed; // aktualna rychlost motora = rychlost motora v predchodzom frame
			float newSpeed; // nastaveni rychlosti motora
			float newTorque; // nastaveni zaberu motora

			oldSpeed = this.joint.motor.motorSpeed;

			//zistenie smeru stlacania sipky
			float direction = Input.GetAxis ("Vertical");
			
			// nie je stlacene ziadne tlacitko
			if (direction == 0) {
				// vypnem tah motora (ala neutral)
				newSpeed = oldSpeed;
				newTorque = 0;
				
				// vypnutie brzdy na prednom kolese
				this.joint.useMotor = false;
			}
			// uzivatel stlaca plyn a auto ide do opacneho smeru
			else if(direction * this.joint.rigidbody2D.velocity.x > 0 ) {
				newSpeed = direction * maxMotorSpeed;
				
				// zapnem tah motora
				newTorque = 50;
				
				// nulova rychlost motora = brzda
				newSpeed = 0;
			}
			// uzivatel stlaca plyn a auto ide do rovnakeho smeru
			else {
				newSpeed = direction * maxMotorSpeed;
				
				if( Mathf.Abs(newSpeed) < Mathf.Abs(this.joint.motor.motorSpeed) ) {
					newSpeed = this.joint.motor.motorSpeed;
				}
				
				// zapnem tah motora
				newTorque = 50;
				
				// vypnutie brzdy na prednom kolese
				this.joint.useMotor = false;	
			}


			// spotreba beniznu + kontrola prazdnosti nadrze
			if (!car.tank.use (Mathf.Abs(direction) + car.neutralGasUsage)) {
				// vypnem tah motora (ala neutral)
				newSpeed = oldSpeed;
				
				newTorque = 0;
			}
			
			JointMotor2D newMotor = this.joint.motor;
			newMotor.motorSpeed = newSpeed;
			newMotor.maxMotorTorque = newTorque;

			this.joint.motor = newMotor;
		}
		if (Mathf.Abs(this.joint.rigidbody2D.velocity.x) > 5 && this.groundContact) {
			this.smoke.enableEmission = true;
		}
		else{
			this.smoke.enableEmission = false;
		}
	}

	public void updateGroundContact(bool newValue) {
		this.groundContact = newValue;
	}
	public void updateSmoking(bool newValue) {
		this.smoking = newValue;
	}

}

public class car : MonoBehaviour {
	public static Wheel[] wheels; // obsahuje pole vsetkych kolies auta
	public static bool breaking;

	WheelJoint2D kolesoZ ;
	WheelJoint2D[] kolesa;
	WheelJoint2D kolesoP ;
	Camera cam;
	public static GasTank tank;

	//maximalna rychlost
	int maxMotorSpeed = 4000;
	//Uroven akceleracie
	//int acceleration = 1;
	
	float rozdil =0;
	float sucetDelta= 0;
	float sucetRozdil = 0;
	public static float neutralGasUsage = 0.2f;


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

	private void wheelsParticles() {
		int i; // pomocna iteracna premenna

		for (i = 0; i < wheels.Length; i++) {
			// posuniem particle systemy spojene s kolesom na zaklade aktualnej pozicie kolesa
			wheels[i].updateParticlesPositions();
		}
	}


	private void sliding() {
		int i; // pomocna iteracna premenna
		
		for (i = 0; i < wheels.Length; i++) {
			// posuniem particle systemy spojene s kolesom na zaklade aktualnej pozicie kolesa
			wheels[i].slidingDetection();
		}
	}


	private void movement () {
		int i; // pomocna iteracna premenna
		
		for (i = 0; i < wheels.Length; i++) {
			// posuniem particle systemy spojene s kolesom na zaklade aktualnej pozicie kolesa
			wheels[i].updateSpeed();
		}
	}


	private void gas_usage() {

	}


	private void breakingDetection() {
		//zistenie smeru stlacania sipky
		float direction = Input.GetAxis ("Vertical");
		
		// uzivatel stlaca plyn a auto ide do opacneho smeru
		if (direction * this.rigidbody2D.velocity.x > 0) {
			car.breaking = true;
		}
		else {
			car.breaking = false;
		}
	}


	void Start () {
		int i; // pomocna iteracna premenna
		Wheel newWheel;
		ParticleSystem dirt; // obsahuje particle system pre smykovanie
		ParticleSystem smoke; // obsahuje particle system pre dymenie kolies (napr. velka rychlost)
		Rigidbody2D wheelSprite;

		car.breaking = false;

		// nacitanie particle systemu
		dirt = (ParticleSystem) GameObject.Find("bahno").particleSystem;
		dirt.enableEmission = false;
		smoke =(ParticleSystem) GameObject.Find("bahno_dym").particleSystem;
		smoke.enableEmission = false;

		// nacitam si zoznam vsetkych kolies
		WheelJoint2D[] wheelsJoint = GetComponents<WheelJoint2D>();
		car.wheels = new Wheel[wheelsJoint.Length];

		for( i = 0; i < wheelsJoint.Length; i++ ) {
			// nase upravene koleso inicializujeme zakladnym objektom kolesa
			newWheel = new Wheel(wheelsJoint[i]);

			// zoberiem si sprite kolesa
			wheelSprite = wheelsJoint[i].connectedBody;

			// priradim skript na detekciu kolizie
			motor_wheel collisionScript = wheelSprite.gameObject.AddComponent("motor_wheel") as motor_wheel;

			// do skriptu si zapisem ID kolesa ke keremu patri
			collisionScript.wheelId = i;

			// skopirujem z objektu auta secky dymy a nastavim ich na pozicie koles (pozor na rozlicne polomery koles)
			newWheel.smoke = (ParticleSystem) Instantiate(smoke);
			newWheel.smoke.enableEmission = false;

			// na zaklade prvotnej hodnoty ci je motor zapojeny sa zisti ci sa jedna o koleso pohanane motorem
			newWheel.powered = wheelsJoint[i].useMotor;

			if ( newWheel.powered ) {
				// okrem skopirovani dymu skopirujem aj strikani bahna (pozicia sa este uvidi)
				newWheel.dirt = (ParticleSystem) Instantiate(dirt);
				newWheel.dirt.enableEmission = false;
			}

			// ulozenie kolesa do pola
			car.wheels[i] = newWheel;
		}

		this.health = 1000;
		car.tank = new GasTank ();
		car.tank.setMaxFill (10000);
	}

	// Update is called once per frame
	void Update () {
		//		ked auto smykluje
		//			podla tabulky materialu zistim kery smyk mam pouzit (priorita materialu)
		//			zapnem particle efekt smyku
		//		ked auto ide moc rychle alebo smykuje
		//			podla inej tabulky materialu zistim kery dym mam pouzit (priorita materialu)
		//			zapnem particle efekt dymu
		//	

		// nastavenie smykovania + dymenia pre jednotlive kolesa

		wheelsParticles ();

		// detekcia smyku
		sliding ();

		breakingDetection ();

		movement ();


/*
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
*/

		/*
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
		*/
	}
}
