using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomInput : MonoBehaviour{
	static Dictionary<string,float> inputs = new Dictionary<string, float>();



	static public float GetAxis(string _axis){
		if(!inputs.ContainsKey(_axis)){
			inputs.Add(_axis, 0);
		}
		return inputs[_axis];
	}
	static public void SetAxis(string _axis, float _value){
		if(!inputs.ContainsKey(_axis)){
			inputs.Add(_axis, 0);
		}
		inputs[_axis] = _value;
	}
	void Start(){
		//GameObject kadlec = GameObject.Find("GUI_left");
		//guiLeft = kadlec.guiTexture;
	 	//kadlec = GameObject.Find("GUI_right");
		//guiRight = kadlec.guiTexture;
		//cam = GameObject.FindGameObjectWithTag("MainCamera").camera;
	}
	void Update(){
		// Check to see if the screen is being touched
		//Debug.Log (Input.touchCount);
		if (Input.touchCount > 0)
		{
			if(Input.touchCount  > 3){
				Application.LoadLevel ("APC-test"); 
			}
			// Get the touch info
			Touch t = Input.GetTouch(0);
			Debug.Log(t.position.x.ToString());
			// Did the touch action just begin?
			if (t.phase == TouchPhase.Began)
			{
				Debug.Log("zacal sem stlacat");
				// Are we touching the left arrow?
				if (Mathf.Abs(t.position.x) > Screen.width/2)
				{
					CustomInput.SetAxis ("Vertical", -1);
					Debug.Log("dopredu");
				}
				else{
					CustomInput.SetAxis ("Vertical", 1);
					Debug.Log("dozadu");
				}
				
		

			}
			
			// Did the touch end?
			if (t.phase == TouchPhase.Ended)
			{
				CustomInput.SetAxis ("Vertical", 0);
				Debug.Log("nic");
			}
		}
	}
}
