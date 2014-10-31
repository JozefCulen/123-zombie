using UnityEngine;
using System.Collections;

public class gui : MonoBehaviour {
	static public string value = "default";

	void OnGUI () {
		// Make a background box
		//GUI.Box(new Rect(10,10,100,90), "Loader Menu");
		GUI.Label(new Rect(10,10,1000,200), value);

	

		/*// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.RepeatButton(new Rect(20,40,80,20), "BRZDA")) {
			CustomInput.SetAxis ("Vertical", 1);
		}
		
		// Make the second button.
		else if(GUI.RepeatButton(new Rect(500,40,80,20), "PLYN")) {
			CustomInput.SetAxis ("Vertical", -1);
		}
		else {
			CustomInput.SetAxis ("Vertical", 0);
		}*/


	}

	static public void setValue(string v){
		value = v;
	}

	static public void changeText(string text){
		GUI.Label(new Rect(10,10,100,90), text);
	}
}