using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class GameManager : MonoBehaviour {

	private CurrentUIState currentUIState;

	void Awake()
	{
		Screen.fullScreen = true;
	}

	void Start()
	{
		StartGame ();
	}

	// start managing isolate
	public void StartGame()
	{
		currentUIState = CurrentUIState.DEFAULT;

		GameStaticDataLoader.LoadStaticData ();

		EnergyModel.instance.Init ();
		GetComponent<RootTimer> ().AddTimer ("EnergyTimer", 5);
		Notice.instance.Observe ("EnergyTimer", EnergyModel.instance);
		GetComponent<RootTimer> ().AddTimer ("CreatureFeelingUpdateTimer", 10);

        PlayerModel.instnace.OpenArea("1");
        
		AgentUnit agent1 = AgentManager.instance.AddAgent (1);
		AgentManager.instance.AddAgent (2);

        //agent1.traitList.Add(TraitTypeList.instance.GetTraitWithId(10018));
        //agent1.applyTrait(TraitTypeList.instance.GetTraitWithId(10018));

        

        /*
		CreatureManager.instance.AddCreature (10001, "1002001", -8, -1);
		CreatureManager.instance.AddCreature (10002, "1003002", -16, -1);
		CreatureManager.instance.AddCreature (10003, "1004101", 8, -1);
		CreatureManager.instance.AddCreature (10004, "1004102", 17, -1);
		CreatureManager.instance.AddCreature (10005, "1003111-left-1", -10, -9);
		CreatureManager.instance.AddCreature (10006, "1003111-right-1", 10, -9);
         */

        /*
        // Na??
        // CreatureManager.instance.AddCreature(20001, "N-way1-point2", -25, -4); // 남자 초상화
        CreatureManager.instance.AddCreature(20005, "N-way1-point2", -25, -4); // 마법소녀
        CreatureManager.instance.AddCreature(20002, "N-way1-point3", -25, -14); // 보고 싶은 사신
        //CreatureManager.instance.AddCreature(20003, "N-way2-point1", -25, -26); // 벽 여인
        CreatureManager.instance.AddCreature(20006, "N-way2-point1", -25, -26); // 없는 책
        CreatureManager.instance.AddCreature(20004, "N-way2-point2", -25, -36); // 삐에로
        */
        //CreatureManager.instance.AddCreature(10001, "N-center-way-point1", -18, -16);
        //CreatureManager.instance.AddCreature(10001, "N-center-way-point1", -18, -24);

		Notice.instance.Send ("AddPlayerLog", "game start가나다");
	}

	public void EndGame()
	{
		GetComponent<RootTimer> ().RemoveTimer ("EnergyTimer");
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
