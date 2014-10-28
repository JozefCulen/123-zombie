using UnityEngine;
using System.Collections;


public class car : MonoBehaviour {
	private Wheel[] wheels; // obsahuje pole vsetkych kolies auta
	private bool breaking; // status ci auto brzdi
	private float velocity; // aktualna rychlost auta (pocitana na zaklade pytagora)
	private int firstPoweredWheel; // prve pohanane kolo (v poradi) na zaklade ktoreho sa budu pocitat niektore globalne hodnoty pre cele auto
	private GasTank tank; // palivova nadrz vozidla
	private float direction; // smer stlacana sipek

	public int maxMotorSpeed = 4000; // maximalna rychlost vozidla
	public float neutralGasUsage = 0.2f; // minimalna spotreba paliva
	public float health = 1000; // vydrz vozdila (pro damage) po spusteni
	public float defaultGasTankValue = 5000; // objem nadrze pri spusteni hry
	public float gasTankCapacity = 10000; // maximalny objem nadrze


	void Start () {
		// inicializacia autovych premenych
		this.breaking = false;
		this.velocity = 0.0f;
		this.firstPoweredWheel = -1;
		this.tank = new GasTank ( gasTankCapacity, defaultGasTankValue );
		this.direction = 0f;

		InitializeAllWheels ();
	}

	
	// Update is called once per frame
	void Update () {
		// aktualizovanie ovladania
		updateCarControl ();

		// aktualizovanie rychlosti auta
		updateCarVelocity();
		
		// detekcia ci auto brzdi
		breakingDetection ();

		// aktualizacia stavu nadrze
		updateGasTank ();

		// aktualizacia kolies
		updateWheels ();

		
		gui.setValue (
			"breaking:" + this.breaking.ToString() + "\n"
			+ "velocity:" + this.velocity.ToString() + "\n"
			+ "x motor speed:" + this.getWheel(this.firstPoweredWheel).joint.motor.motorSpeed.ToString() + "\n"
			+ "x velocity:" + this.getWheel(this.firstPoweredWheel).joint.rigidbody2D.velocity.x.ToString() + "\n"
			+ "direction:" + this.direction.ToString() + "\n"
			+ "w sliding:" + this.getWheel(this.firstPoweredWheel).sliding.ToString() + "\n"
			+ "w groundContact:" + this.getWheel(this.firstPoweredWheel).groundContact.ToString() + "\n"
			+ "w smoking:" + this.getWheel(this.firstPoweredWheel).smoking.ToString() + "\n"
			+ "Tank:" + this.tank.getCurrentFill() + "/" + this.tank.getMaxFill() + "\n"
			+ "Health:" + this.getHealth()
			);
	}

	public Wheel getWheel(int input_index) {
		return this.wheels[input_index];
	}
	
	private void InitializeAllWheels() {
		int i; // pomocna iteracna premenna
		ParticleSystem dirt; // obsahuje particle system pre smykovanie
		ParticleSystem smoke; // obsahuje particle system pre dymenie kolies (napr. velka rychlost)
		
		// nacitanie particle systemu
		dirt = (ParticleSystem) GameObject.Find("bahno").particleSystem;
		dirt.enableEmission = false;
		smoke =(ParticleSystem) GameObject.Find("bahno_dym").particleSystem;
		smoke.enableEmission = false;
		
		// nacitam si zoznam vsetkych kolies
		WheelJoint2D[] wheelsJoint = GetComponents<WheelJoint2D>();

		// inicializujem si pole do ktoreho ulozim vsetky kolesa
		this.wheels = new Wheel[wheelsJoint.Length];
		
		for( i = 0; i < wheelsJoint.Length; i++ ) {
			// objekt kolesa transofrmujem do rozsireneho objektu kolesa a ulozim do pola
			//this.wheels[i] = new Wheel(wheelsJoint[i], i, (ParticleSystem) Instantiate(smoke), (ParticleSystem) Instantiate(dirt));

			// jedna sa o prve pohanane koleso v ramci auta
			if( this.firstPoweredWheel == -1 && this.wheels[i].IsPowered() ) {
				this.firstPoweredWheel = i;
			}
		}
	}

	private void updateCarControl() {
		this.direction = Input.GetAxis ("Vertical");
	}

	private void updateCarVelocity() {
		// vypocitani rychlosti auta na zaklade pytagorovej vety
		this.velocity = this.wheels[firstPoweredWheel].GetVelocityMagnitude();
	}
	
	private void breakingDetection() {
		//zistenie smeru stlacania sipky
		float direction = Input.GetAxis ("Vertical");
		
		// uzivatel stlaca plyn a auto ide do opacneho smeru
		if (direction * this.rigidbody2D.velocity.x > 0) {
			this.breaking = true;
		}
		else {
			this.breaking = false;
		}
	}

	private void updateWheels() {
		int i; // pomocna iteracna premenna
		
		for (i = 0; i < wheels.Length; i++) {
			// posuniem particle systemy spojene s kolesom na zaklade aktualnej pozicie kolesa
			wheels[i].UpdateParticlesPositions();

			// zistim ci dane koleso smykuje
			wheels[i].SlidingDetection();

			// zistim ci dane koleso dymi
			wheels[i].SmokingDetection();

			// jedna sa o pohanane koleso, aktualizujem rychlost
			if( wheels[i].IsPowered() == true ) {
				wheels[i].updateSpeed();

				if ( this.IsGasTankEmpty ()) {
					wheels[i].TurnOffMotor();
				}
			}
			// jedna sa o nepohanane koleso, aktualizujem stav brzd
			else {
				wheels[i].updateBreak();
			}

			// zapnutie prislusnych particle systemov
			wheels[i].UpdateParticlesEmissions();
		}
	}

	private void updateGasTank() {
		this.tank.Use ( neutralGasUsage + Mathf.Abs(this.direction));
	}

	public bool IsGasTankEmpty() {
		return this.tank.IsEmpty ();
	}

	public bool IsCarBreaking() {
		return this.breaking;
	}

	public float GetDirection() {
		return this.direction;
	}

	// TODO: damage

	void OnCollisionEnter2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite") {
			this.makeDamage(col.relativeVelocity.magnitude);
		}
	}
	
	public void makeDamage(float v)	{
		this.health -= v;
	}
	
	public float getHealth()	{
		return this.health;
	}

}

//		ked auto smykluje
//			podla tabulky materialu zistim kery smyk mam pouzit (priorita materialu)
//			zapnem particle efekt smyku
//		ked auto ide moc rychle alebo smykuje
//			podla inej tabulky materialu zistim kery dym mam pouzit (priorita materialu)
//			zapnem particle efekt dymu
//	
