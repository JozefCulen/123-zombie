using UnityEngine;
using System.Collections;

public class gui : MonoBehaviour {
	// obrazky pre GUI
	public Texture2D speedMeterPointer;
	public Texture2D speedMeterBackground;
	public Texture2D gasRemainingPointer;
	public Texture2D gasRemainingBackground;
	public Texture2D scoreIcon;
	public Texture2D menuButton;

	// pisma pre GUI
	public GUIStyle scoreStyle;

	// hodnoty pre GUI
	static public float speedMeterValue;
	static public float gasRemainingValue;
	static public double score;
	public float shiftX = 1;
	public float shiftY = 1;

	// globalne premenne pre aktualnu scenu
	static public float startTime;
	static public float finishTime;
	protected string level = "APC-test";
	static public bool finish;
	static public bool menu;
	static public bool paused;

	// debug vypis
	static public string value = "default";


	// Flashy
	public GUISkin skin = null;
	public float widthPercent = 0.5f;
	public float heightPercent = 0.5f;
	public Texture2D logo = null;
	public string optionQuality = "Medium";
	public string optionParticles = "ON";
	public string optionSounds = "ON";

	void Awake () {
		speedMeterValue = 0.0f;
		gasRemainingValue = 0.0f;
		score = 0.0f;
		finish = false;
		menu = false;
		gui.paused = false;
		Time.timeScale = 1.0f;
	}

	void OnGUI () {
		// debug vypis
		GUI.Label(new Rect(300, 10, 1000, 200), value);

		drawSpeedMeter();
		drawGasRemaining();
		drawScore();

		if( GUI.Button(new Rect(Screen.width - 100, 10, 80, 80), menuButton, GUIStyle.none )) {
			gui.menu = true;
			gui.paused = true;
			Time.timeScale = 0.0f;
		}
		
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

		GUI.skin = this.skin;
		Rect menuWindow = new Rect(Screen.width * (1 - widthPercent) / 2,
			              Screen.height * (1 - heightPercent) / 2,
			              Screen.width * widthPercent,
			              Screen.height * heightPercent); 

		if (gui.finish) {
			drawFinish(menuWindow);
		}
		else if( gui.menu ) {
			drawMenu(menuWindow);
		}

	}
	
	void drawFinish(Rect r)	{
		double elapsedTime = ( (int) ((gui.finishTime - gui.startTime) * 100) / 100.0 );

		GUILayout.BeginArea(r);
		GUILayout.BeginVertical("box");

		GUILayout.Label( "Elapsed time: "+ elapsedTime.ToString() +"s" , scoreStyle);
		GUILayout.Label( "Earned score: " + score.ToString() , scoreStyle);
		GUILayout.Label( "Best score: 0", scoreStyle);

		if (GUILayout.Button("Restart"))
			Application.LoadLevel(this.level);
		
		else if (GUILayout.Button("Quit")) {
			GameControl.save.score += score;
			GameControl.Save();
			Application.LoadLevel("menu");
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	void drawMenu(Rect r)	{
		GUILayout.BeginArea(r);
		GUILayout.BeginVertical("box");
		
		GUILayout.Label( "Actual time: "+ (Time.time - gui.startTime).ToString() +"s" , scoreStyle);
		
		if (GUILayout.Button ("Resume")) {
			gui.menu = false;
			gui.paused = false;
			Time.timeScale = 1.0f;
		}
		else if (GUILayout.Button("Restart"))
			Application.LoadLevel(this.level);
		
		else if (GUILayout.Button("Quit"))
			Application.LoadLevel("menu");
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void drawScore()
	{
		GUI.Label(new Rect(0, 0, 64, 64), scoreIcon);
		GUI.Label(new Rect(56, 0, 64, 64), score.ToString() , scoreStyle);
	}
	private void drawSpeedMeter()
	{
		int positionShiftX = 0;
		int positionShiftY = 0;
		int rectangleSize = 150;
		int arrowSize = 100;
		int arrowRange = 270;

		float positionCentreX = Screen.width / 2 - rectangleSize/2 - positionShiftX;
		float positionCentreY = Screen.height - rectangleSize/2 - positionShiftY;
		
		GUI.Label(new Rect(positionCentreX - rectangleSize/2, positionCentreY - rectangleSize/2, rectangleSize, rectangleSize), speedMeterBackground);
		float rotationAngle = speedMeterValue * arrowRange - arrowRange / 2;
		GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(positionCentreX - 3, positionCentreY));
		GUI.Label(new Rect(positionCentreX - arrowSize/2, positionCentreY - arrowSize/2, arrowSize, arrowSize), speedMeterPointer);
		GUI.matrix = Matrix4x4.identity;
	}
	
	private void drawGasRemaining()
	{
		int positionShiftX = 0;
		int positionShiftY = 0;
		int rectangleSize = 100;
		int arrowSize = 70;
		int arrowRange = 180;
		
		float positionCentreX = Screen.width / 2 + rectangleSize/2 + positionShiftX;
		float positionCentreY = Screen.height - rectangleSize/2 - positionShiftY;
		
		GUI.Label(new Rect(positionCentreX - rectangleSize/2, positionCentreY - rectangleSize/2, rectangleSize, rectangleSize), gasRemainingBackground);
		float rotationAngle = gasRemainingValue * arrowRange - arrowRange / 2;
		GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(positionCentreX - 3, positionCentreY));
		GUI.Label(new Rect(positionCentreX - arrowSize/2, positionCentreY - arrowSize/2, arrowSize, arrowSize), gasRemainingPointer);
		GUI.matrix = Matrix4x4.identity;
	}

	static public void setValue(string v){
		value = v;
	}

	static public void changeText(string text){
		GUI.Label(new Rect(10,10,100,90), text);
	}
}