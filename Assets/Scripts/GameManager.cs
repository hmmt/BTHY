using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

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

        if (PlayerModel.instance.GetDay() == 0)
        {
            PlayerModel.instance.OpenArea("1"); ;
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

        int day = PlayerModel.instance.GetDay();
        stageTimeInfoUI.StartTimer(StageTypeInfo.instnace.GetStageGoalTime(day), this);
	}

	public void EndGame()
	{
        Debug.Log("EndGame");
        EnergyModel.instance.SetLeftEnergy((int)EnergyModel.instance.GetLeftEnergy()+EnergyModel.instance.GetStageLeftEnergy());

        GetComponent<RootTimer> ().RemoveTimer ("EnergyTimer");
        GetComponent<RootTimer>().RemoveTimer("CreatureFeelingUpdateTimer");
	}

    public void TimeOver()
    {
        EndGame();

        int day = PlayerModel.instance.GetDay();
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
        int day = PlayerModel.instance.GetDay();
        PlayerModel.instance.SetDay(day + 1);
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

    public void Quit()
    {
        Application.Quit();
    }

    public void SaveData()
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();

        dic.Add("agents", AgentManager.instance.GetSaveData());
        dic.Add("creatures", CreatureManager.instance.GetSaveData());
        dic.Add("playerData", PlayerModel.instance.GetSaveData());

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveData1.txt");
        bf.Serialize(file, dic);
        file.Close();
    }

    public void LoadData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file =  File.Open(Application.persistentDataPath + "/saveData1.txt", FileMode.Open);
        Dictionary<string, object> dic = (Dictionary<string, object>)bf.Deserialize(file);
        file.Close();

        Dictionary<string, object> agents = null, creatures = null, playerData = null;

        GameUtil.TryGetValue(dic, "agents", ref agents);
        GameUtil.TryGetValue(dic, "creatures", ref creatures);
        GameUtil.TryGetValue(dic, "playerData", ref playerData);

        PlayerModel.instance.LoadData(playerData);
        AgentManager.instance.LoadData(agents);
        CreatureManager.instance.LoadData(creatures);
    }
}
