using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

/*
 * Ukladanie spociva vo viacerych krokoch. Ak by sa pri ukladani hry len zapisalo do suboru save.dat,
 * mohlo by sa stat ze pocas zapisu spadne aplikacia alebo nastane nejaka chyba. Tym by sa mohli stratit
 * vsetky ulozene data. Preto sa vyuzivaju pomocne subore, ktore by tomuto MALI (=SHOULD) zabranit.
 */
public static class GameControl {
	public static SaveFile2 save;
	private static int version = 2;
	
	private static SaveFile1 Init1() {
		SaveFile1 save = new SaveFile1 ();
		save.path = Application.persistentDataPath;
		SaveUser1 test = new SaveUser1 ();
		test.score = 123;
		test.username = "Queen Elsa 1";
		save.users.Add(test);
		return save;
	}
	
	private static SaveFile2 Init2() {
		SaveFile2 save = new SaveFile2 ();
		save.path = Application.persistentDataPath;
		SaveUser1 test = new SaveUser1 ();
		test.score = 123;
		test.username = "Queen Elsa 2";
		save.users.Add(test);
		save.score = 0;
		return save;
	}
	
	public static void Init() {
		save = Init2 ();
	}
	
	public static void Save() {
		try
		{
			// vytvorenie pomocneho suboru (ak nahodou uz existuje, prepise sa)
			FileStream file = File.Create (Application.persistentDataPath + "/save" + version + "-phase1.dat");
			
			// nahranie obsahu do pomocneho suboru
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize (file, save);
			
			// zatvorenie pomocneho suboru
			file.Close ();

            // premenujem pomocny subor na iny nazov aby som vedel, ze subor je validny
            FileUtil.MoveFileOrDirectory(Application.persistentDataPath + "/save" + version + "-phase1.dat", Application.persistentDataPath + "/save" + version + "-phase2.dat");
			
			// odstrananie hlavneho suboru
			if( File.Exists (Application.persistentDataPath + "/save" + version + ".dat") ) {
				File.Delete(Application.persistentDataPath + "/save" + version + ".dat");
			}
			
			// skopirovanie pomocneho suboru na hlavny subor
			File.Copy (Application.persistentDataPath + "/save" + version + "-phase2.dat", Application.persistentDataPath + "/save" + version + ".dat");
			
			// odstranenie pomocneho suboru
			File.Delete(Application.persistentDataPath + "/save" + version + "-phase2.dat");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}
	
	private static void FinishUncompletedSavingProcess(int version) {
		// skontrolovat ci existuje phase 2
		if (File.Exists (Application.persistentDataPath + "/save" + version + "-phase2.dat")) {
			// skontrolovat ci existuje phase 1
			if( File.Exists (Application.persistentDataPath + "/save" + version + "-phase1.dat") ) {
				// odstranenie pomocneho suboru
				File.Delete(Application.persistentDataPath + "/save" + version + "-phase2.dat");

                // premenujem pomocny subor na iny nazov aby som vedel, ze subor je validny
                FileUtil.MoveFileOrDirectory (Application.persistentDataPath + "/save" + version + "-phase1.dat", Application.persistentDataPath + "/save" + version + "-phase2.dat");
			}
			
			// odstrananie hlavneho suboru
			if( File.Exists (Application.persistentDataPath + "/save" + version + ".dat") ) {
				File.Delete(Application.persistentDataPath + "/save" + version + ".dat");
			}
			
			// skopirovanie pomocneho suboru na hlavny subor
			File.Copy (Application.persistentDataPath + "/save" + version + "-phase2.dat", Application.persistentDataPath + "/save" + version + ".dat");
			
			// odstranenie pomocneho suboru
			File.Delete(Application.persistentDataPath + "/save" + version + "-phase2.dat");
		}
		// skontrolovat ci existuje phase 1
		else if( File.Exists (Application.persistentDataPath + "/save" + version + "-phase1.dat") ) {
			// odstranit phase 1
			File.Delete(Application.persistentDataPath + "/save" + version + "-phase1.dat");
		}
	}
	
	public static void Load() {
		FinishUncompletedSavingProcess (version);
		
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/save" + version + ".dat", FileMode.Open);
		save = (SaveFile2)bf.Deserialize (file);
		file.Close ();
	}
	
	private static void ConvertSaveFileVersion1 () {
		SaveFile2 newSave = Init2 ();
		
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/save1.dat", FileMode.Open);
		SaveFile1 oldSave = (SaveFile1)bf.Deserialize (file);
		file.Close ();
		
		// prepisat defaultne hodnoty v novem save ze stareho savu
		newSave.path = oldSave.path;
		newSave.users = oldSave.users;
		
		save = newSave;

		File.Delete(Application.persistentDataPath + "/save1.dat");
	}
	
	private static void ConvertSaveFileToNewerVersion ( int fromVersion) {
		if (fromVersion == 1) {
			ConvertSaveFileVersion1();
		}/* else if (fromVersion == 2) {
			ConvertSaveFileVersion2();
		}*/

		Save ();
	}
	
	public static bool ValidSaveFile() {
		if( !File.Exists (Application.persistentDataPath + "/save" + version + ".dat") && !File.Exists (Application.persistentDataPath + "/save" + version + "-phase2.dat") ) {
			for (int i = 1; i < version; i++) {
				if( File.Exists (Application.persistentDataPath + "/save" + i + "-phase2.dat") ) {
					FinishUncompletedSavingProcess(i);
				}
				if( File.Exists (Application.persistentDataPath + "/save" + i + ".dat") ) {
					ConvertSaveFileToNewerVersion(i);
				}
			}
		}
		
		return File.Exists (Application.persistentDataPath + "/save" + version + ".dat") || File.Exists (Application.persistentDataPath + "/save" + version + "-phase2.dat");
	}
	
	public static void Destroy() {
		for (int i = 1; i < version; i++) {
			if( File.Exists (Application.persistentDataPath + "/save" + version + ".dat") ) {
				File.Delete(Application.persistentDataPath + "/save" + version + ".dat");
			}
			if( File.Exists (Application.persistentDataPath + "/save" + version + "-phase1.dat") ) {
				File.Delete(Application.persistentDataPath + "/save" + version + "-phase1.dat");
			}
			if( File.Exists (Application.persistentDataPath + "/save" + version + "-phase2.dat") ) {
				File.Delete(Application.persistentDataPath + "/save" + version + "-phase2.dat");
			}
		}
	}
}

[Serializable]
public class SaveUser1
{
	public string username;
	public int score;
}

[Serializable]
public class SaveFile1
{
	public string path;
	public List<SaveUser1> users  = new List<SaveUser1>();
}

[Serializable]
public class SaveFile2
{
	public string path;
	public List<SaveUser1> users  = new List<SaveUser1>();
	public double score;
}