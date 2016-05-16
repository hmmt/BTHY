using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

public enum GameState
{
    PLAYING,
	STOP,
    PAUSE
}

public enum GameSceneState
{
    STORY,
    MAINGAME
}

public class GameManager : MonoBehaviour
{

    private string saveFileName;
    private static GameManager _currentGameManager;

    private CurrentUIState currentUIState;

    // game mode object
    public GameObject gameStateScreen;
    public StageTimeInfoUI stageTimeInfoUI;
    public StageUI stageUI;

    // story mode object
    public StoryScene storyScene;

    // loading mode object
    public LoadingScreenState loadingScreenState;


    public BriefingInfo briefingText;
    public GameState state;

    public static GameManager currentGameManager
    {
        get { return _currentGameManager; }
    }

    void Awake()
    {
		InitFirst ();

		state = GameState.STOP;

        saveFileName = Application.persistentDataPath + "/saveData1.txt";

        Screen.fullScreen = true;
        _currentGameManager = this;

        //Camera.main.orthographicSize += 5;


        // 옮겨야 한다.
		/*
        GameStaticDataLoader.LoadStaticData();
        MapGraph.instance.LoadMap();
        */
        //EnergyModel.instance.Init();
    }

	void InitFirst()
	{
		if (AutoCommandManager.instance == null) {
			new GameObject ("AgentAI").AddComponent<AutoCommandManager>();
		}
	}

    void Start()
    {
        AgentLayer.currentLayer.Init();
        CreatureLayer.currentLayer.Init();
        OfficerLayer.currentLayer.Init();
        //AgentListScript.instance.Init();

		AgentModel a = null;
        if (PlayerModel.instance.GetDay() == 0)
        {
            PlayerModel.instance.OpenArea("1");
			PlayerModel.instance.OpenArea("4");

			a = AgentManager.instance.AddAgentModel();
            
        }

     
        foreach (AgentModel agent in AgentManager.instance.GetAgentList())
        {
            agent.ReturnToSefira();
        }

        foreach (OfficerModel officer in OfficerManager.instance.GetOfficerList()) {
            officer.ReturnToSefira();
        }
        
        

		stageUI.CancelSefiraAgent(a);
		a.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("1"));
		a.SetCurrentSefira("1");
		if (!a.activated)
			AgentManager.instance.activateAgent(a, "1");

		a.SetCurrentNode (MapGraph.instance.GetNodeById ("sefira-malkuth-5"));

		//a.weapon = AgentWeapon.SHIELD;

		// temp
		AgentModel b = AgentManager.instance.AddAgentModel();
		b.gender = "Male";
		stageUI.CancelSefiraAgent(b);
		b.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("1"));
		b.SetCurrentSefira("1");
		if (!b.activated)
			AgentManager.instance.activateAgent(b, "1");

		b.SetCurrentNode (MapGraph.instance.GetNodeById ("sefira-malkuth-4"));

		//b.weapon = AgentWeapon.SHIELD;

		b = AgentManager.instance.AddAgentModel();
		stageUI.CancelSefiraAgent(b);
		b.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("4"));
		b.SetCurrentSefira("4");
		if (!b.activated)
			AgentManager.instance.activateAgent(b, "4");

		b.SetCurrentNode (MapGraph.instance.GetNodeById ("sefira-malkuth-4"));


		//b.weapon = AgentWeapon.SHIELD;


		/*
		b.Panic ();



		a.StartSuppressAgent (b, sa);
		*/
        StartStage();
		//stageUI.Close ();
		//StartGame();
        //OpenStoryScene("start");
        /*
        gameStateScreen.SetActive(false);
        storyScene.gameObject.SetActive(false);
        loadingScreenState.StartLoading();
        */
    }

    public void OpenStoryScene(string storyName)
    {
        if (storyName == "start")
        {
            loadingScreenState.gameObject.SetActive(false);
            gameStateScreen.SetActive(false);
            storyScene.gameObject.SetActive(true);
            storyScene.LoadStory(storyName);
        }
    }

    public void StartStage()
    {
        /*
        loadingScreenState.gameObject.SetActive(false);
        storyScene.gameObject.SetActive(false);
        gameStateScreen.SetActive(true);
        */
        stageUI.Open(StageUI.UIType.START_STAGE);
        // StartGame();
    }

    // start managing isolate
    public void StartGame()
    {
        state = GameState.PLAYING;
        //briefingText.SetNarrationByDay();

        currentUIState = CurrentUIState.DEFAULT;

        GetComponent<RootTimer>().AddTimer("EnergyTimer", 5);
        Notice.instance.Observe("EnergyTimer", EnergyModel.instance);
        GetComponent<RootTimer>().AddTimer("CreatureFeelingUpdateTimer", 10);

        int day = PlayerModel.instance.GetDay();
        stageTimeInfoUI.StartTimer(StageTypeInfo.instnace.GetStageGoalTime(day), this);
        /*
        foreach (Sefira s in SefiraManager) { 
            
        }
        */
        SefiraManager.instance.getSefira(SefiraName.Malkut).InitAgentSkillList();
        CreatureManager.instance.OnStageStart();

        foreach (OfficerModel om in OfficerManager.instance.GetOfficerList()) {
            StartCoroutine(om.StartAction());
        }

        foreach (AgentModel am in AgentManager.instance.GetAgentList()) {
            am.MakeAccessoryByTraits();
        }

        AgentLayer.currentLayer.OnStageStart();
        OfficerLayer.currentLayer.OnStageStart();

    }

	public void Pause()
	{
		state = GameState.PAUSE;
		stageTimeInfoUI.Pause ();
	}

	public void Resume()
	{
		state = GameState.PLAYING;
		stageTimeInfoUI.Resume ();
	}

    public void EndGame()
    {
		state = GameState.STOP;
        Debug.Log("EndGame");
        if (AgentStatusWindow.currentWindow != null)
            AgentStatusWindow.currentWindow.CloseWindow();

        if (SelectWorkAgentWindow.currentWindow != null)
            SelectWorkAgentWindow.currentWindow.CloseWindow();

        if (WorkAllocateWindow.currentWindow != null)
        {
            WorkAllocateWindow.currentWindow.CloseWindow();
        }

        if (CollectionWindow.currentWindow != null)
            CollectionWindow.currentWindow.CloseWindow();

        if (SelectSefiraAgentWindow.currentWindow != null)
            SelectSefiraAgentWindow.currentWindow.CloseWindow();
        EnergyModel.instance.SetLeftEnergy((int)EnergyModel.instance.GetLeftEnergy() + EnergyModel.instance.GetStageLeftEnergy());
        //직원계산파트 필요
        
        GetComponent<RootTimer>().RemoveTimer("EnergyTimer");
        GetComponent<RootTimer>().RemoveTimer("CreatureFeelingUpdateTimer");
    }

    void FixedUpdate()
    {
        if (state == GameState.PLAYING)
        {
            Notice.instance.Send(NoticeName.FixedUpdate);
        }
    }

    public void TimeOver()
    {
        EndGame();

        int day = PlayerModel.instance.GetDay();
        float needEnergy = StageTypeInfo.instnace.GetEnergyNeed(day);
        float energy = EnergyModel.instance.GetEnergy();

        if (energy >= needEnergy)
        {
            stageUI.Open(StageUI.UIType.END_STAGE);
            EndStage.instance.init(AgentManager.instance.GetAgentList()[0]);
            //briefingText.SetNarrationByDay();
        }
        else
        {
            //storyScene.LoadStory("start");
            //OpenStoryScene("start");
            stageUI.Open(StageUI.UIType.END_STAGE);
            EndStage.instance.init(AgentManager.instance.GetAgentList()[0]);
            Debug.Log("Game Over..");
        }
    }

    public void ExitStage()
    {
        int day = PlayerModel.instance.GetDay();
        PlayerModel.instance.SetDay(day + 1);
        EnergyModel.instance.Init();

        //stageUI.Open(StageUI.UIType.START_STAGE);
        //Application.LoadLevel("Menu");
    }

    public void SetCurrentUIState(CurrentUIState state)
    {
        currentUIState = state;
    }
    public CurrentUIState GetCurrentUIState()
    {
        return currentUIState;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SaveData()
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();

        dic.Add("agents", AgentManager.instance.GetSaveData());
        dic.Add("officers", OfficerManager.instance.GetSaveData());
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
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(saveFileName, FileMode.Open);
        Dictionary<string, object> dic = (Dictionary<string, object>)bf.Deserialize(file);
        file.Close();

        Dictionary<string, object> agents = null, creatures = null, playerData = null;
        Dictionary<string, object> officers = null;
        GameUtil.TryGetValue(dic, "agents", ref agents);
        GameUtil.TryGetValue(dic, "officers", ref officers);
        GameUtil.TryGetValue(dic, "creatures", ref creatures);
        GameUtil.TryGetValue(dic, "playerData", ref playerData);

        PlayerModel.instance.LoadData(playerData);
        AgentManager.instance.LoadData(agents);
        OfficerManager.instance.LoadData(officers);
        CreatureManager.instance.LoadData(creatures);
    }
}
