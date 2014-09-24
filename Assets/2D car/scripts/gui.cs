using UnityEngine;
using System.Collections;

public class gui : MonoBehaviour {
	static public string value = "default";

	void OnGUI () {
		// Make a background box
		//GUI.Box(new Rect(10,10,100,90), "Loader Menu");
		GUI.Label(new Rect(10,10,1000,90), value);

	}

	static public void setValue(string v){
		value = v;
	}

	static public void changeText(string text){
		GUI.Label(new Rect(10,10,100,90), text);
	}
}