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
	
	public float smokingLargeSpeed = 15;
	public int maxTorgueSpeed = 50;
	public int maxTorgueBreak = 175;
	public float hugeFallValue = 5f;
	public int wheelsLevel = 0;

	private float resetPosition;

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
		/*
		if (Input.GetKeyDown ("space")) {
			resetPosition = this.transform.position.y + 4.0f;
			this.transform.position = new Vector3(this.transform.position.x, resetPosition, this.transform.position.z);
		} else if (Input.GetKey ("space")) {
			this.transform.position = new Vector3(this.transform.position.x, resetPosition, this.transform.position.z);
			this.transform.Rotate( new Vector3(0, 0, 200) , 2f);
		}
		*/
		if (CustomInput.ResetCarJump ()) {
			resetPosition = this.transform.position.y + 4.0f;
			this.transform.position = new Vector3(this.transform.position.x, resetPosition, this.transform.position.z);
		} else if (CustomInput.ResetCarRotate ()) {
			this.transform.position = new Vector3(this.transform.position.x, resetPosition, this.transform.position.z);
			this.transform.Rotate( new Vector3(0, 0, 200) , 2f);
		}

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
			+ "velocity:" + this.velocity.ToString() + "\n"/*
			+ "x motor speed:" + this.getWheel(this.firstPoweredWheel).joint.motor.motorSpeed.ToString() + "\n"
			+ "x velocity:" + this.getWheel(this.firstPoweredWheel).joint.rigidbody2D.velocity.x.ToString() + "\n"
			+ "direction:" + this.direction.ToString() + "\n"
			+ "w sliding:" + this.getWheel(this.firstPoweredWheel).sliding.ToString() + "\n"
			+ "w groundContact:" + this.getWheel(this.firstPoweredWheel).groundContact.ToString() + "\n"
			+ "w smoking:" + this.getWheel(this.firstPoweredWheel).smoking.ToString() + "\n"*/
			+ "Tank:" + this.tank.getCurrentFill() + "/" + this.tank.getMaxFill() + "\n"
			+ "Health:" + this.getHealth()
			);
	}

	public Wheel getWheel(int input_index) {
		return this.wheels[input_index];
	}
	
	private void InitializeAllWheels() {
		int i; // pomocna iteracna premenna
		GameObject wheelPrefab; // kopia kolesa
		string wheelPrefabName; // nazov objektu kolesa
		WheelJoint2D wheelJoint; // spoj medzi autem a kolesem
		/*
		// nacitanie particle systemu
		dirt = ; //GameObject.Find("bahno").particleSystem;
		dirt.enableEmission = false;
		smoke =; //GameObject.Find("bahno_dym").particleSystem;
		smoke.enableEmission = false;
		*/
		// nacitam si zoznam bodov kde sa pripoja kolesa
		GameObject[] wheelsAnchor = GameObject.FindGameObjectsWithTag("wheelAnchor");

		// inicializujem si pole do ktoreho ulozim vsetky kolesa
		this.wheels = new Wheel[wheelsAnchor.Length];
		
		for( i = 0; i < wheelsAnchor.Length; i++ ) {
			// vytvoreni nazvu objektu kolesa
			wheelPrefabName = wheelsAnchor[i].GetComponent<WheelAnchor>().wheelType.ToString() + "_" + wheelsLevel.ToString();

			// auto pridam spoj pre koleso
			wheelJoint = this.gameObject.AddComponent<WheelJoint2D>();

			// skopirujem si objekt kolesa
			wheelPrefab = (GameObject) GameObject.Instantiate(Resources.Load("prefabs/wheels/"+wheelPrefabName));

			// vytvoreny objekt bude spadat pod auto
			wheelPrefab.transform.parent = this.transform;

			// nastaveni pozicie kolesa
			wheelPrefab.transform.position = new Vector3(wheelsAnchor[i].transform.position.x, wheelsAnchor[i].transform.position.y, -0.1f);

			// pripojeni kolesa k bodu pripoju na aute
			wheelJoint.connectedBody = wheelPrefab.rigidbody2D;

			// nastaveni pozicie bodu pripoju
			wheelJoint.anchor = new Vector2(wheelsAnchor[i].transform.localPosition.x, wheelsAnchor[i].transform.localPosition.y);

			// pripoj bude pripojeny na stred auta
			wheelJoint.connectedAnchor = new Vector2(0, 0);

			// inicializacia kolesa
			wheelPrefab.GetComponent<Wheel>().Wheel2(wheelJoint, i, wheelsAnchor[i].GetComponent<WheelAnchor>().powered);

			// nastaveni parametru tlmicov
			JointSuspension2D newSuspension = wheelJoint.suspension;
			newSuspension.dampingRatio = wheelsAnchor[i].GetComponent<WheelAnchor>().suspensionDampingRatio;
			newSuspension.frequency = wheelsAnchor[i].GetComponent<WheelAnchor>().suspensionFrequency;
			wheelJoint.suspension = newSuspension;

			// odstranim si pomocny objekt na zaklade kereho sem vedel kam dat koleso
			Destroy(wheelsAnchor[i]);

			// objekt kolesa transofrmujem do rozsireneho objektu kolesa a ulozim do pola
			this.wheels[i] = wheelPrefab.GetComponent<Wheel>();

			// jedna sa o prve pohanane koleso v ramci auta
			if( this.firstPoweredWheel == -1 && this.wheels[i].IsPowered() ) {
				this.firstPoweredWheel = i;
			}

		}
	}

	private void updateCarControl() {
		//this.direction = Input.GetAxis ("Vertical");
		//this.direction = CustomInput.GetAxis ("Vertical");
		this.direction = CustomInput.GetDirection ();
	}

	private void updateCarVelocity() {
		// vypocitani rychlosti auta na zaklade pytagorovej vety
		this.velocity = this.wheels[firstPoweredWheel].GetVelocityMagnitude();
	}
	
	private void breakingDetection() {
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
	
	public void gasTankFill(int value) {
		this.tank.Fill ( value);
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
