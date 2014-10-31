using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomInput : MonoBehaviour{
	static Dictionary<string,float> inputs = new Dictionary<string, float>();
	static private bool resettingCar;
	static private bool touching; 

	static public bool ResetCarJump() {
		if( Input.GetKeyDown("space") ) {
			return true;
		} else if ( Input.touchCount == 3 && resettingCar == false) {
			resettingCar = true;
			return true;
		}

		return false;
	}

	static public bool ResetCarRotate() {
		if( Input.GetKey("space") ) {
			return true;
		} else if ( Input.touchCount == 3 && resettingCar == true) {
			return true;
		}
		
		return false;
	}

	static public float GetDirection() {
		// ak uzivatel stlaca klavesnicu vratim hodnotu
		if ( Input.GetAxis ("Vertical") != 0 ) {
			return Input.GetAxis ("Vertical");
		}

		// ak uzivatel nestlaca ani touch input
		if(Input.touchCount == 0) {
			resettingCar = false;
			return 0.0f;
		}

		// uzivatel stlaca touch input
		Touch t = Input.GetTouch(0);
		// Did the touch action just begin?
		if (t.phase == TouchPhase.Began || touching == true)
		{
			touching = true;
			if (Mathf.Abs(t.position.x) > Screen.width/2)
			{
				return -1;
			}
			else{
				return 1;
			}
		}

		// Did the touch end?
		if (t.phase == TouchPhase.Ended)
		{
			touching = false;
		}

		return 0.0f;
	}

	public float GetAxis(string _axis){
		if(!inputs.ContainsKey(_axis)){
			inputs.Add(_axis, 0);
		}
		return inputs[_axis];
	}
	public void SetAxis(string _axis, float _value){
		if(!inputs.ContainsKey(_axis)){
			inputs.Add(_axis, 0);
		}
		inputs[_axis] = _value;
	}
	void Start(){
		resettingCar = false;
		touching = true;
		//cam = GameObject.FindGameObjectWithTag("MainCamera").camera;
	}
	void Update(){
		if (Input.touchCount > 3) {
			Application.LoadLevel ("APC-test"); 
			// Input.touchCount = 1 alebo 2
		}
	}
}
