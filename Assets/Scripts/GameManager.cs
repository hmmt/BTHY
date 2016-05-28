using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

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


	// global models
	private EnergyModel energyModel;
	private PlayerModel playerModel;

    public static GameManager currentGameManager
    {
        get { return _currentGameManager; }
    }

    void Awake()
    {
		InitFirst ();

		state = GameState.STOP;

        

        Screen.fullScreen = true;
        _currentGameManager = this;

		energyModel = EnergyModel.instance;
		playerModel = PlayerModel.instance;
    }

	void InitFirst()
	{
		if (AutoCommandManager.instance == null) {
			new GameObject ("AgentAI").AddComponent<AutoCommandManager>();
		}
	}

    void Start()
    {
		InitGame ();
    }

	public void InitGame()
	{
		AgentLayer.currentLayer.Init();
		CreatureLayer.currentLayer.Init();
		OfficerLayer.currentLayer.Init();
		//AgentListScript.instance.Init();
		AgentAllocateWindow.instance.Init();

		PlayerModel.instance.OpenArea("1");
		//AgentManager.instance.AddAgentModel ();
		/*
		AgentModel a = null;
		if (PlayerModel.instance.GetDay() == 0)
		{
			PlayerModel.instance.OpenArea("1");
			//PlayerModel.instance.OpenArea("4");

			a = AgentManager.instance.AddAgentModel();

		}
		*/

		foreach (AgentModel agent in AgentManager.instance.GetAgentList())
		{
			agent.ReturnToSefira();
		}

		foreach (OfficerModel officer in OfficerManager.instance.GetOfficerList()) {
			officer.ReturnToSefira();
		}

		/*
		stageUI.CancelSefiraAgent(a);
		a.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("1"));
		a.SetCurrentSefira("1");
		if (!a.activated)
			AgentManager.instance.activateAgent(a, "1");

		a.SetCurrentNode (MapGraph.instance.GetNodeById ("sefira-malkuth-5"));
		*/

		StartStage();
		//stageUI.Close ();
		//StartGame();
		//OpenStoryScene("start");
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
        stageUI.Open(StageUI.UIType.START_STAGE);
        // StartGame();
    }

    // start managing isolate
    public void StartGame()
    {
        state = GameState.PLAYING;
        //briefingText.SetNarrationByDay();

        currentUIState = CurrentUIState.DEFAULT;

		GetComponent<RootTimer>().AddTimer(NoticeName.EnergyTimer, 5);
		GetComponent<RootTimer>().AddTimer(NoticeName.CreatureFeelingUpdateTimer, 10);

        int day = PlayerModel.instance.GetDay();
        stageTimeInfoUI.StartTimer(StageTypeInfo.instnace.GetStageGoalTime(day), this);
        /*
        foreach (Sefira s in SefiraManager) { 
            
        }
        */
        SefiraManager.instance.GetSefira(SefiraName.Malkut).InitAgentSkillList();
        CreatureManager.instance.OnStageStart();

        foreach (OfficerModel om in OfficerManager.instance.GetOfficerList()) {
            StartCoroutine(om.StartAction());
        }

        foreach (AgentModel am in AgentManager.instance.GetAgentList()) {
            am.MakeAccessoryByTraits();
            //am.ResetAnimator();
            
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
        
		GetComponent<RootTimer>().RemoveTimer(NoticeName.EnergyTimer);
		GetComponent<RootTimer>().RemoveTimer(NoticeName.CreatureFeelingUpdateTimer);
    }

    void FixedUpdate()
    {
		FixedUpdateProccess ();
        if (state == GameState.PLAYING)
        {
            Notice.instance.Send(NoticeName.FixedUpdate);
        }
    }

	public void FixedUpdateProccess()
	{
		int day = PlayerModel.instance.GetDay();
		float needEnergy = StageTypeInfo.instnace.GetEnergyNeed(day);
		float energy = EnergyModel.instance.GetEnergy();

		if (energy >= needEnergy)
		{
			TimeOver ();
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
}
