using UnityEngine;
using System.Collections;

public class Wheel {
	public WheelJoint2D joint; // objekt kde je pripojene koleso
	private bool powered; // je koleso pohanane motorem?
	private ParticleSystem smoke; // particle system pre dymenie
	private ParticleSystem dirt; // particle system pre smyk
	public bool sliding; // koleso smykuje
	public bool groundContact; // koleso ma kontakt so zemou
	public bool smoking; // z kolesa sa generuje dym
	private bool hugeFallLastEnable; // 
	private float hugeFallLastTime; //
	
	public static float wheelRadius = 0.2f; // velkost kolesa
	public static float skidConstant = 174;
	public static float smokingLargeSpeed = 15;
	public static int maxMotorSpeed = 4000;
	public static int maxTorgueSpeed = 50;
	public static int maxTorgueBreak = 175;
	public float temp;

	public Wheel(WheelJoint2D jointObject, int input_index, ParticleSystem input_smoke, ParticleSystem input_dirt)
	{
		this.joint = jointObject;
		
		// na zaklade prvotnej hodnoty ci je motor zapojeny sa zisti ci sa jedna o koleso pohanane motorem
		this.powered = jointObject.useMotor;
		
		// skopirujem z objektu auta secky dymy a nastavim ich na pozicie koles (pozor na rozlicne polomery koles)
		this.smoke = input_smoke;
		this.smoke.enableEmission = false;
		
		if ( this.powered ) {
			// okrem skopirovani dymu skopirujem aj strikani bahna (pozicia sa este uvidi)
			this.dirt = input_dirt;
			this.dirt.enableEmission = false;
		}
		
		this.sliding = false;
		this.groundContact = false;
		this.smoking = false;
		
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
	}
	
	public void UpdateParticlesPositions() {
		// ak koleso nie je pohanane, tak smyky neriesim
		if ( this.powered ) {
			dirt.transform.position = new Vector3 (joint.connectedBody.transform.position.x, joint.connectedBody.transform.position.y - wheelRadius, joint.connectedBody.transform.position.z - 5);
		}
		smoke.transform.position = new Vector3(joint.connectedBody.transform.position.x, joint.connectedBody.transform.position.y - wheelRadius,joint.connectedBody.transform.position.z - 5);
	}

	public void SlidingDetection() {
		float skidValue; // absolutna hodnota smyku

		// pomer rychlosti otacania kolesa a rychlosti kolesa (t.j. aku vzdialenost preslo)
		skidValue = Mathf.Abs(joint.jointSpeed / joint.rigidbody2D.velocity.magnitude );

		// TODO: prevod na tvar 1.000 +- 0.02
		this.temp = skidValue;
		if ( ( skidValue > skidConstant + 60 || skidValue < skidConstant - 60 ) && this.groundContact && joint.rigidbody2D.velocity.x > 0.1f) {
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
		if ( Mathf.Abs(this.joint.rigidbody2D.velocity.magnitude) > Wheel.smokingLargeSpeed && this.groundContact ) {
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
		if (car.GetDirection() == 0) {
			// vypnem tah motora (ala neutral)
			newMotor.maxMotorTorque = 0;
			
			// vypnutie brzdy na prednom kolese
			this.joint.useMotor = false;
		}
		// uzivatel stlaca plyn a auto ide do opacneho smeru
		else if(car.GetDirection() * this.joint.rigidbody2D.velocity.x > 0 ) {
			// zapnem tah motora
			newMotor.maxMotorTorque = maxTorgueSpeed;
			
			// nulova rychlost motora = brzda
			newMotor.motorSpeed = 0;
		}
		// uzivatel stlaca plyn a auto ide do rovnakeho smeru
		else {
			newMotor.motorSpeed = car.GetDirection() * maxMotorSpeed;
			
			if( Mathf.Abs(newMotor.motorSpeed) < Mathf.Abs(this.joint.motor.motorSpeed) ) {
				newMotor.motorSpeed = this.joint.motor.motorSpeed;
			}
			
			// zapnem tah motora
			newMotor.maxMotorTorque = maxTorgueSpeed;
			
			// vypnutie brzdy na prednom kolese
			this.joint.useMotor = false;	
		}

		// aktualizovanie objektu motora
		this.joint.motor = newMotor;
	}
	
	public void updateBreak() {
		// v pripade nepohananeho kolesa sa motor chova ako brzda
		
		JointMotor2D newMotor = this.joint.motor;

		if( car.IsCarBreaking() == true ) {
			// zapnem motor (brzdu)
			newMotor.maxMotorTorque = maxTorgueBreak;
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
		if (input_value > 5) {
			this.hugeFallLastTime = Time.time;
		}
	}
}
