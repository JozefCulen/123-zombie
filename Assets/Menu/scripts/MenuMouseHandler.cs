using UnityEngine;
using System.Collections;

public class MenuMouseHandler : MonoBehaviour {
	protected string level = "APC-test";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {
		Application.LoadLevel(this.level);
	}
}
