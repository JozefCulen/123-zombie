using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class GameControl {

	public static SaveFile save;

	public static void Init() {
		GameControl.save = new SaveFile ();
		GameControl.save.path = Application.persistentDataPath;
		SaveUser test = new SaveUser ();
		test.score = 123;
		test.username = "Elsa";
		GameControl.save.users.Add(test);
	}

	public static void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/save.dat");

		bf.Serialize (file, save);
		file.Close ();
	}
	
	public static void Load() {
		if (!System.IO.File.Exists (Application.persistentDataPath + "/save.dat")) {
			GameControl.Init ();
		} else {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/save.dat", FileMode.Open);
			save = (SaveFile)bf.Deserialize (file);
			file.Close ();
		}
	}
	
	public static void Destroy() {
		File.Delete (Application.persistentDataPath + "/save.dat");
	}
}

[Serializable]
public class SaveUser
{
	public string username;
	public int score;
}

[Serializable]
public class SaveFile
{
	public string path;
	//public SaveUser user;
	public List<SaveUser> users  = new List<SaveUser>();
}