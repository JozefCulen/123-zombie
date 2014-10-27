using UnityEngine;
using System.Collections;


public class car : MonoBehaviour {
	private static Wheel[] wheels; // obsahuje pole vsetkych kolies auta
	private static bool breaking; // status ci auto brzdi
	private static float velocity; // aktualna rychlost auta (pocitana na zaklade pytagora)
	private static int firstPoweredWheel; // prve pohanane kolo (v poradi) na zaklade ktoreho sa budu pocitat niektore globalne hodnoty pre cele auto
	private static GasTank tank; // palivova nadrz vozidla
	private static float direction; // smer stlacana sipek

	public static int maxMotorSpeed = 4000; // maximalna rychlost vozidla
	public static float neutralGasUsage = 0.2f; // minimalna spotreba paliva
	public static float health = 1000; // vydrz vozdila (pro damage) po spusteni
	public static float defaultGasTankValue = 5000; // objem nadrze pri spusteni hry
	public static float gasTankCapacity = 10000; // maximalny objem nadrze
	
	void Start () {
		// inicializacia autovych premenych
		car.breaking = false;
		car.velocity = 0.0f;
		car.firstPoweredWheel = -1;
		car.tank = new GasTank ( gasTankCapacity, defaultGasTankValue );
		car.direction = 0f;

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
			"breaking:" + car.breaking.ToString() + "\n"
			+ "velocity:" + car.velocity.ToString() + "\n"
			+ "x motor speed:" + car.getWheel(car.firstPoweredWheel).joint.motor.motorSpeed.ToString() + "\n"
			+ "x velocity:" + car.getWheel(car.firstPoweredWheel).joint.rigidbody2D.velocity.x.ToString() + "\n"
			+ "direction:" + car.direction.ToString() + "\n"
			+ "w sliding:" + car.getWheel(car.firstPoweredWheel).sliding.ToString() + "\n"
			+ "w groundContact:" + car.getWheel(car.firstPoweredWheel).groundContact.ToString() + "\n"
			+ "w smoking:" + car.getWheel(car.firstPoweredWheel).smoking.ToString() + "\n"
			+ "w temp:" + car.getWheel(car.firstPoweredWheel).temp.ToString() + "\n"
			+ "Tank:" + car.tank.getCurrentFill() + "/" + car.tank.getMaxFill() + "\n"
			+ "Health:" + car.getHealth()
			);
	}

	public static Wheel getWheel(int input_index) {
		return car.wheels[input_index];
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
		car.wheels = new Wheel[wheelsJoint.Length];
		
		for( i = 0; i < wheelsJoint.Length; i++ ) {
			// objekt kolesa transofrmujem do rozsireneho objektu kolesa a ulozim do pola
			car.wheels[i] = new Wheel(wheelsJoint[i], i, (ParticleSystem) Instantiate(smoke), (ParticleSystem) Instantiate(dirt));

			// jedna sa o prve pohanane koleso v ramci auta
			if( car.firstPoweredWheel == -1 && car.wheels[i].IsPowered() ) {
				car.firstPoweredWheel = i;
			}
		}
	}

	private void updateCarControl() {
		car.direction = Input.GetAxis ("Vertical");
	}

	private void updateCarVelocity() {
		// vypocitani rychlosti auta na zaklade pytagorovej vety
		car.velocity = car.wheels[firstPoweredWheel].GetVelocityMagnitude();
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

				if ( car.IsGasTankEmpty ()) {
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
		car.tank.Use ( neutralGasUsage + Mathf.Abs(car.direction));
	}

	public static bool IsGasTankEmpty() {
		return car.tank.IsEmpty ();
	}

	public static bool IsCarBreaking() {
		return car.breaking;
	}

	public static float GetDirection() {
		return car.direction;
	}

	// TODO: damage

	void OnCollisionEnter2D(Collision2D col)	{
		if (col.gameObject.name == "GrassThinSprite") {
			this.makeDamage(col.relativeVelocity.magnitude);
		}
	}
	
	public static  void makeDamage(float v)	{
		car.health -= v;
	}
	
	public static float getHealth()	{
		return car.health;
	}

}

//		ked auto smykluje
//			podla tabulky materialu zistim kery smyk mam pouzit (priorita materialu)
//			zapnem particle efekt smyku
//		ked auto ide moc rychle alebo smykuje
//			podla inej tabulky materialu zistim kery dym mam pouzit (priorita materialu)
//			zapnem particle efekt dymu
//	
