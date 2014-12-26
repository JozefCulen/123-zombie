using UnityEngine;
using System.Collections;

public class gui : MonoBehaviour {
	static public string value = "default";
	public Texture2D speedMeterPointer;
	public Texture2D speedMeterBackground;
	public Texture2D gasRemainingPointer;
	public Texture2D gasRemainingBackground;
	public float temp1 = 0.0f;
	public float temp2 = 0.0f;
	public float temp3 = 128.0f;
	public float temp4 = 0.0f;
	static public float speedMeterValue = 0.0f;
	static public float gasRemainingValue = 0.0f;
	public Texture2D scoreIcon;
	static public double score = 0.0f;
	public GUIStyle scoreStyle;

	public GUISkin skin = null;
	public float widthPercent = 0.5f;
	public float heightPercent = 0.5f;
	public Texture2D logo = null;
	static public bool finish = false;
	public string optionQuality = "Medium";
	public string optionParticles = "ON";
	public string optionSounds = "ON";
	
	protected string level = "APC-test";

	void OnGUI () {
		//Screen.width
		//Screen.height

		// Make a background box
		//GUI.Box(new Rect(10,10,100,90), "Loader Menu");
		GUI.Label(new Rect(300, 10, 1000, 200), value);
		/*
		if (GUI.Button (new Rect (10,10, 100, 50), icon)) {
			print ("you clicked the icon");
		}
		*/

		drawSpeedMeter();
		drawGasRemaining();
		drawScore();

		/*
		Rect guiRect = new Rect(Screen.width / 2.0f, Screen.height / 2.0f, 128.0f, 128.0f);
		float xValue = ((guiRect.x + guiRect.width / 2.0f));
		float yValue = ((guiRect.y + guiRect.height - 35.0f));
		Vector2 Pivot = new Vector2(xValue, yValue);


		float rotationAngle = rotationAngleX * 270.0f - 135.0f;

		GUIUtility.RotateAroundPivot(rotationAngle, Pivot);
		GUI.Label(guiRect, icon);
		GUI.matrix = Matrix4x4.identity;
		*/

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
		Rect r = new Rect(Screen.width * (1 - widthPercent) / 2,
		                  Screen.height * (1 - heightPercent) / 2,
		                  Screen.width * widthPercent,
		                  Screen.height * heightPercent); 
		
		if (gui.finish) {
			drawFinish(r);
		}

	}
	
	void drawFinish(Rect r)	{
		GUILayout.BeginArea(r);
		GUILayout.BeginVertical("box");
		
		GUILayout.Label( "Elapsed time: 1m 10s" , scoreStyle);
		GUILayout.Label( "Earned score: " + score.ToString() , scoreStyle);
		GUILayout.Label( "Best score: 0", scoreStyle);

		if (GUILayout.Button("Restart"))
			Application.LoadLevel(this.level);
		
		if (GUILayout.Button("Quit"))
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
		float guiSizeX = 128.0f;
		float guiSizeY = 128.0f;
		float guiPositionX = Screen.width / 2.0f - guiSizeX - 50.0f;
		float guiPositionY = Screen.height - 180;
		float pivotPointShift = 25.0f;
		float pivotAngleRange = 270.0f;

		GUI.Label(new Rect(Screen.width / 2.0f - guiSizeX - 68, Screen.height - 165, 170, 170), speedMeterBackground);

		Rect guiRect = new Rect(guiPositionX, guiPositionY, guiSizeX, guiSizeY);
		float xValue = ((guiRect.x + guiRect.width / 2.0f));
		float yValue = ((guiRect.y + guiRect.height - pivotPointShift));
		Vector2 Pivot = new Vector2(xValue, yValue);

		float rotationAngle = speedMeterValue * (pivotAngleRange) - (pivotAngleRange / 2.0f);
		
		GUIUtility.RotateAroundPivot(rotationAngle, Pivot);
		GUI.Label(guiRect, speedMeterPointer);
		GUI.matrix = Matrix4x4.identity;
	}
	
	private void drawGasRemaining()
	{
		float guiSizeX = 128.0f;
		float guiSizeY = 128.0f;
		float guiPositionX = Screen.width / 2.0f + 50.0f;
		float guiPositionY = Screen.height - 180;
		float pivotPointShift = 25.0f;
		float pivotAngleRange = 120.0f;
		
		GUI.Label(new Rect(Screen.width / 2.0f + guiSizeX - 97, Screen.height - 165, 170, 170), gasRemainingBackground);

		Rect guiRect = new Rect(guiPositionX, guiPositionY, guiSizeX, guiSizeY);
		float xValue = ((guiRect.x + guiRect.width / 2.0f));
		float yValue = ((guiRect.y + guiRect.height - pivotPointShift));
		Vector2 Pivot = new Vector2(xValue, yValue);
		
		float rotationAngle = gasRemainingValue * (pivotAngleRange) - (pivotAngleRange / 2.0f);
		
		GUIUtility.RotateAroundPivot(rotationAngle, Pivot);
		GUI.Label(guiRect, gasRemainingPointer);
		GUI.matrix = Matrix4x4.identity;
	}

	static public void setValue(string v){
		value = v;
	}

	static public void changeText(string text){
		GUI.Label(new Rect(10,10,100,90), text);
	}
}