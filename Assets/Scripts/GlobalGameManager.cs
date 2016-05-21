using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GlobalGameManager : MonoBehaviour {

	private static GlobalGameManager _instance = null;
	public static GlobalGameManager instance
	{
		get
		{
			return _instance;
		}
	}

	private string saveFileName;

	public string language = "en";

	void Awake()
	{
		if (_instance != null) {
			Destroy (gameObject);
			return;
		}

		_instance = this;

		DontDestroyOnLoad (gameObject);

		saveFileName = Application.persistentDataPath + "/saveData1.txt";
		GameStaticDataLoader.LoadStaticData();
		MapGraph.instance.LoadMap ();

		AgentManager.instance.Init ();
		//OfficerManager.instance.Init ();
	}

	void Start()
	{
		/*
		if (SceneManager.GetActiveScene ().name == "Main")
		{
			GameScreen.instance.gameObject.SetActive (true);
		}
		else
		{
			GameScreen.instance.gameObject.SetActive (false);
		}
		*/
	}

	void OnLevelWasLoaded()
	{
		/*
		if (SceneManager.GetActiveScene ().name == "Main")
		{
			GameScreen.instance.gameObject.SetActive (true);
			GameManager.currentGameManager.InitGame ();
		}
		else
		{
			GameScreen.instance.gameObject.SetActive (false);
		}
		*/
	}

	void Update()
	{
		//if (Application.loadedLevelName == "StartScene")
		if (SceneManager.GetActiveScene ().name == "StartScene")
		{
			Debug.Log ("load....");
			//Application.LoadLevel ("Main");
			SceneManager.LoadScene("StorySceneTemp");
		}
	}

	public void InitGame()
	{
		OfficerManager.instance.Clear ();
		AgentManager.instance.Clear ();
		CreatureManager.instance.Clear ();
		PlayerModel.instance.Init ();

		PlayerModel.instance.OpenArea("1");
		//PlayerModel.instance.OpenArea("4");

		AgentManager.instance.AddAgentModel();
	}

	public void ResetGame()
	{
		OfficerManager.instance.Clear ();
		AgentManager.instance.Clear ();
		CreatureManager.instance.Clear ();
		PlayerModel.instance.Init ();

		MapGraph.instance.Reset ();
	}

	public void SaveData()
	{
		Dictionary<string, object> dic = new Dictionary<string, object>();

		dic.Add("agents", AgentManager.instance.GetSaveData());
		//dic.Add("officers", OfficerManager.instance.GetSaveData());
		dic.Add("creatures", CreatureManager.instance.GetSaveData());
		dic.Add("playerData", PlayerModel.instance.GetSaveData());

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(saveFileName);
		bf.Serialize(file, dic);
		file.Close();
	}

	public void LoadData()
	{
		if (File.Exists(saveFileName) == false)
		{
			Debug.Log("save file don't exists");
			return;
		}

		ResetGame ();

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(saveFileName, FileMode.Open);
		Dictionary<string, object> dic = (Dictionary<string, object>)bf.Deserialize(file);
		file.Close();

		Dictionary<string, object> agents = null, creatures = null, playerData = null;
		Dictionary<string, object> officers = null;
		GameUtil.TryGetValue(dic, "agents", ref agents);
		//GameUtil.TryGetValue(dic, "officers", ref officers);
		GameUtil.TryGetValue(dic, "creatures", ref creatures);
		GameUtil.TryGetValue(dic, "playerData", ref playerData);

		PlayerModel.instance.LoadData(playerData);
		AgentManager.instance.LoadData(agents);
		//OfficerManager.instance.LoadData(officers);
		CreatureManager.instance.LoadData(creatures);
	}
}