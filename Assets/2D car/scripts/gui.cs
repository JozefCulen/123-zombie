using UnityEngine;
using System.Collections;

public class gui : MonoBehaviour {

	void OnGUI () {
		// Make a background box
		//GUI.Box(new Rect(10,10,100,90), "Loader Menu");
		GUI.Label(new Rect(10,10,100,90), "status");
	}
	public void changeText(string text){
		GUI.Label(new Rect(10,10,100,90), text);
	}
}