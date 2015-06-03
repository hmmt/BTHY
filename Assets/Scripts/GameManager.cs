using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class GameManager : MonoBehaviour {

    private static GameManager _currentGameManager;

	private CurrentUIState currentUIState;

    public StageTimeInfoUI stageTimeInfoUI;
    public StageUI stageUI;

    public BriefingInfo briefingText;

    public static GameManager currentGameManager
    {
        get { return _currentGameManager; } 
    }

	void Awake()
	{

		Screen.fullScreen = true;
        _currentGameManager = this;

        Camera.main.orthographicSize += 5;


        // 옮겨야 한다.
        MapGraph.instance.LoadMap();
        GameStaticDataLoader.LoadStaticData();
        //EnergyModel.instance.Init();
	}

	void Start()
	{
        AgentLayer.currentLayer.Init();
        CreatureLayer.currentLayer.Init();

        if (PlayerModel.instnace.GetDay() == 0)
        {
            PlayerModel.instnace.OpenArea("1");;
            AgentManager.instance.AddAgentModel(1);
        }

        foreach (AgentModel agent in AgentManager.instance.GetAgentList())
        {
            agent.ReturnToSefira();
        }


		//StartGame ();
        stageUI.Open(StageUI.UIType.START_STAGE);
	}

	// start managing isolate
	public void StartGame()
	{
        briefingText.SetNarrationByDay();

		currentUIState = CurrentUIState.DEFAULT;

		GetComponent<RootTimer> ().AddTimer ("EnergyTimer", 5);
		Notice.instance.Observe ("EnergyTimer", EnergyModel.instance);
		GetComponent<RootTimer> ().AddTimer ("CreatureFeelingUpdateTimer", 10);

        int day = PlayerModel.instnace.GetDay();
        stageTimeInfoUI.StartTimer(StageTypeInfo.instnace.GetStageGoalTime(day), this);
	}

	public void EndGame()
	{
        Debug.Log("EndGame");
        EnergyModel.instance.SetLeftEnergy((int)EnergyModel.instance.GetLeftEnergy()+EnergyModel.instance.GetStageLeftEnergy());

        GetComponent<RootTimer> ().RemoveTimer ("EnergyTimer");
        GetComponent<RootTimer>().AddTimer("CreatureFeelingUpdateTimer", 10);
	}

    public void TimeOver()
    {
        EndGame();

        int day = PlayerModel.instnace.GetDay();
        float needEnergy = StageTypeInfo.instnace.GetEnergyNeed(day);
        float energy = EnergyModel.instance.GetEnergy();
        
        if(energy >= needEnergy)
        {
            stageUI.Open(StageUI.UIType.END_STAGE);
            briefingText.SetNarrationByDay();
        }
        else
        {
            Debug.Log("Game Over..");
        }
    }

    public void ExitStage()
    {
        int day = PlayerModel.instnace.GetDay();
        PlayerModel.instnace.SetDay(day + 1);
        //stageUI.Open(StageUI.UIType.START_STAGE);
        EnergyModel.instance.Init();
        Application.LoadLevel("Menu");
    }

	public void SetCurrentUIState(CurrentUIState state)
	{
		currentUIState = state;
	}
	public CurrentUIState GetCurrentUIState()
	{
		return currentUIState;
	}
}
