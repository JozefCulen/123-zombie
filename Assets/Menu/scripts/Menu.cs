using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	public GUISkin skin = null;

	public float widthPercent = 0.5f;
	public float heightPercent = 0.5f;

	public Texture2D logo = null;

	public bool showOptions = false;

	public string optionQuality = "Medium";
	public string optionParticles = "ON";
	public string optionSounds = "ON";

	protected string level = "APC-test";

	void drawOptions(Rect r)	{
		GUILayout.BeginArea (r);
			GUILayout.BeginVertical("box");  
				GUILayout.BeginHorizontal ();

					GUILayout.Label ("Quality settings:", GUILayout.Width (100));
					if (GUILayout.Button (this.optionQuality))
						this.optionQuality = (this.optionQuality == "Low" ? "Medium" : (this.optionQuality == "Medium" ? "High" : "Low"));

				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				
					GUILayout.Label ("Show particles:", GUILayout.Width (100));
					if (GUILayout.Button (this.optionParticles))
						this.optionParticles = (this.optionParticles == "ON" ? "OFF" : "ON");

				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				
					GUILayout.Label ("Sounds:", GUILayout.Width (100));
					if (GUILayout.Button (this.optionSounds)) 
						this.optionSounds = (this.optionSounds == "ON" ? "OFF" : "ON");

				GUILayout.EndHorizontal ();

				if (GUILayout.Button("Back to Menu"))
					this.showOptions = false;   

			GUILayout.EndVertical();  
		GUILayout.EndArea ();
	}

	void drawMain(Rect r)	{
		GUILayout.BeginArea(r);
			GUILayout.BeginVertical("box");   
		
			if (GUILayout.Button("Play"))
				Application.LoadLevel(this.level);   
		
			if (GUILayout.Button("Options"))
				this.showOptions = true;   
		
			if (GUILayout.Button("Quit"))
				Application.Quit();
		
			GUILayout.EndVertical();        
		GUILayout.EndArea();   
	}

	void OnGUI() {
		GUI.skin = this.skin;

		Rect r = new Rect(Screen.width * (1 - widthPercent) / 2,
		                  Screen.height * (1 - heightPercent) / 2,
		                  Screen.width * widthPercent,
		                  Screen.height * heightPercent); 

		if (this.logo == null) {
			this.logo = Resources.Load ("prefabs/logo/panda_jam") as Texture2D;
			Rect l = new Rect(Screen.width / 2 - this.logo.width / 4,
			                  r.y - this.logo.height / 2 - 10,
			                  this.logo.width / 2,
			                  this.logo.height / 2);
			GUI.DrawTexture(l, this.logo);
		}

		if (this.showOptions) {
			drawOptions(r);
		} 
		else {
			drawMain (r);
		}
	}
}