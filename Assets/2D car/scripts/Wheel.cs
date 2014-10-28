using UnityEngine;
using System.Collections;
using GlobalVariables;

public class Wheel : MonoBehaviour {
	private WheelJoint2D joint; // objekt kde je pripojene koleso
	private bool powered; // je koleso pohanane motorem?
	private ParticleSystem smoke; // particle system pre dymenie
	private ParticleSystem dirt = null; // particle system pre smyk
	private bool sliding; // koleso smykuje
	private bool groundContact; // koleso ma kontakt so zemou
	private bool smoking; // z kolesa sa generuje dym
	private float hugeFallLastTime; // cas posledneho velkeho dopadu
	private float wheelRadius; // velkost kolesa
	private float skidConstant ; // konstanta podla kerej sa rozhoduje ci auto smykuje aleob ne
	private car carInstance;

	public constants.WheelTypeEnum wheelType;
	public int wheelLevel = 0;

	public void Wheel2(WheelJoint2D jointObject, int input_index, bool input_powered)
	{
		this.carInstance = this.gameObject.transform.parent.GetComponent<car> ();
		this.joint = jointObject;
		this.powered = input_powered;
		
		// skopirujem z objektu auta secky dymy a nastavim ich na pozicie koles (pozor na rozlicne polomery koles)
		this.smoke = (ParticleSystem) Instantiate(Resources.Load("particles/bahno_dym", typeof(ParticleSystem)));
		this.smoke.transform.parent = carInstance.transform;
		this.smoke.enableEmission = false;
		
		if ( this.powered ) {
			// okrem skopirovani dymu skopirujem aj strikani bahna (pozicia sa este uvidi)
			this.dirt = (ParticleSystem) Instantiate(Resources.Load("particles/bahno", typeof(ParticleSystem)));
			this.dirt.transform.parent = carInstance.transform;
			this.dirt.enableEmission = false;
		}
		
		this.sliding = false;
		this.groundContact = false;
		this.smoking = false;
		this.wheelRadius = this.joint.connectedBody.gameObject.GetComponent<CircleCollider2D> ().radius;
		this.skidConstant = 57.2957795f / this.wheelRadius;

		// pridanie skriptu pre detekciu kolizie na koleso
		AttachCollisionScript (jointObject, input_index);

	}

	private void AttachCollisionScript(WheelJoint2D jointObject, int input_index) {
		Rigidbody2D wheelSprite;
		
		// zoberiem si sprite kolesa
		wheelSprite = jointObject.connectedBody; 
		
		// priradim skript na detekciu kolizie
		WheelCollision collisionScript = wheelSprite.gameObject.AddComponent("WheelCollision") as WheelCollision;
		
		// do skriptu si zapisem ID kolesa ke keremu patri
		collisionScript.wheelId = input_index;

		// ulozim do premennej odkaz na objekt car
		collisionScript.carInstance = this.carInstance;
	}
	
	public void UpdateParticlesPositions() {
		// ak koleso nie je pohanane, tak smyky neriesim
		if ( this.powered ) {
			dirt.transform.position = new Vector3 (joint.connectedBody.transform.position.x, joint.connectedBody.transform.position.y - 0, joint.connectedBody.transform.position.z - 5);
		}
		smoke.transform.position = new Vector3(joint.connectedBody.transform.position.x, joint.connectedBody.transform.position.y - this.wheelRadius + 0.2f,joint.connectedBody.transform.position.z - 5);
	}

	public void SlidingDetection() {
		float skidValue; // absolutna hodnota smyku

		// pomer rychlosti otacania kolesa a rychlosti kolesa (t.j. aku vzdialenost preslo)
		skidValue = Mathf.Abs(joint.jointSpeed / joint.rigidbody2D.velocity.magnitude );

		if ( (skidValue / skidConstant > 1.03 || skidValue / skidConstant < 0.95) && this.groundContact && joint.rigidbody2D.velocity.x > 0.1f) {
			this.sliding = true;
		}
		else{
			this.sliding = false;
		}
	}
	
	public void SmokingDetection() {
		this.smoking = false; // defaultny stav
		
		// auto smykuje
		if ( this.sliding ) {
			this.smoking = true;
		}
		
		// velka rychlost kolesa
		if ( Mathf.Abs(this.joint.rigidbody2D.velocity.magnitude) > carInstance.smokingLargeSpeed && groundContact ) {
			this.smoking = true;
		}

		// pad z velkej vysky v poslednej dobe
		if ( Time.time - this.hugeFallLastTime < 0.5f ) {
			this.smoking = true;
		}
	}

	public void TurnOffMotor() {
		JointMotor2D newMotor = this.joint.motor;
		newMotor.maxMotorTorque = 0;
		this.joint.motor = newMotor;
	}
	
	public void updateSpeed() {
		JointMotor2D newMotor = this.joint.motor; // objekt motora, ktorym sa nahradi ten aktualny

		// nie je stlacene ziadne tlacitko
		if (this.carInstance.GetDirection() == 0) {
			if( this.joint.motor.motorSpeed * this.joint.rigidbody2D.velocity.x > 0 ) {
				newMotor.motorSpeed = newMotor.motorSpeed * -1;
			}

			// vypnem tah motora (ala neutral)
			newMotor.maxMotorTorque = 0;
			
			// vypnutie brzdy na prednom kolese
			this.joint.useMotor = false;
		}
		// uzivatel stlaca plyn a auto ide do opacneho smeru
		else if(this.carInstance.GetDirection() * this.joint.rigidbody2D.velocity.x > 0 ) {
			// zapnem tah motora
			newMotor.maxMotorTorque = carInstance.maxTorgueBreak;
			
			// nulova rychlost motora = brzda
			newMotor.motorSpeed = 0;
		}
		// uzivatel stlaca plyn a auto ide do rovnakeho smeru
		else {
			newMotor.motorSpeed = this.carInstance.GetDirection() * carInstance.maxMotorSpeed;
			
			if( Mathf.Abs(newMotor.motorSpeed) < Mathf.Abs(this.joint.motor.motorSpeed) ) {
				newMotor.motorSpeed = this.joint.motor.motorSpeed;
			}
			
			// zapnem tah motora
			newMotor.maxMotorTorque = carInstance.maxTorgueSpeed;
			
			// vypnutie brzdy na prednom kolese
			this.joint.useMotor = false;	
		}

		// aktualizovanie objektu motora
		this.joint.motor = newMotor;
	}
	
	public void updateBreak() {
		// v pripade nepohananeho kolesa sa motor chova ako brzda
		
		JointMotor2D newMotor = this.joint.motor;

		if( this.carInstance.IsCarBreaking() == true ) {
			// zapnem motor (brzdu)
			newMotor.maxMotorTorque = carInstance.maxTorgueBreak;
		}
		else {
			// vypnem motor (brzdu)
			newMotor.maxMotorTorque = 0;
		}

		this.joint.motor = newMotor;
	}

	public void UpdateParticlesEmissions() {
		// dymenie kolesa
		this.smoke.enableEmission = this.smoking;

		// ak sa jedna o pohanane koleso, moze dane koleso aj smykovat
		if( this.powered ) {
			this.dirt.enableEmission = this.sliding;
		}
	}

	public bool IsPowered() {
		return this.powered;
	}

	public float GetVelocityMagnitude() {
		return this.joint.rigidbody2D.velocity.magnitude;
	}

	public void UpdateGroundContact( bool input_value) {
		this.groundContact = input_value;
	}
	
	public void UpdateFallMagnitude( float input_value) {
		if (input_value > carInstance.hugeFallValue) {
			this.hugeFallLastTime = Time.time;
		}
	}
}
