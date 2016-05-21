using UnityEngine;
using System.Collections.Generic;

public class SefiraMapLayer : MonoBehaviour, IObserver {

    public SefiraObject[] sefiras;

    void Start()
    {
		OnInit ();
    }

#if FALSE
	// temporary
	void CancelSefiraAgent(AgentModel unit)
	{
		if (unit.currentSefira.Equals("1"))
		{
			for (int i = 0; i < AgentManager.instance.malkuthAgentList.Count; i++)
			{
				if (unit.instanceId == AgentManager.instance.malkuthAgentList[i].instanceId)
				{
					AgentManager.instance.malkuthAgentList.RemoveAt(i);
					break;
				}
			}
		}
		if(unit.activated)
			AgentManager.instance.deactivateAgent(unit);
	}

	void TempInit()
	{
		GameStaticDataLoader.LoadStaticData ();

		MapGraph.instance.ActivateArea("1");
		//PlayerModel.instance.OpenArea ("1");


		Sefira s = SefiraManager.instance.getSefira("1");
		s.activated = true;
		s.initCreatureArray();
		s.initOfficerGroup();

		foreach (OfficerModel officer in OfficeManager.instance.GetOfficerList()) {
			officer.ReturnToSefira();
		}


		AgentModel a = AgentManager.instance.AddAgentModel();
		CancelSefiraAgent (a);
		a.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("1"));
		a.SetCurrentSefira("1");
		if (!a.activated)
			AgentManager.instance.activateAgent(a, "1");

		SefiraManager.instance.getSefira(SefiraName.Malkut).InitAgentSkillList();
		foreach (OfficerModel om in OfficeManager.instance.GetOfficerList()) {
			StartCoroutine(om.StartAction());
		}

		string str = "";
		for (int i = 0; i < 10; i++)
		{
			Vector3 offset = MapGraph.instance.GetNodeById ("left-upper-way2"+ str).GetPosition () - MapGraph.instance.GetNodeById ("left-upper-way2" ).GetPosition();
			CreatureManager.instance.AddCreature (100001, "left-upper-way2"+str, -8f+offset.x, -3f+offset.y, "1");
			CreatureManager.instance.AddCreature (100002, "left-upper-way3"+str, -15f+offset.x, -3f+offset.y, "1");
			CreatureManager.instance.AddCreature (100003, "right-upper-way2"+str, 8f+offset.x, -3f+offset.y, "1");
			CreatureManager.instance.AddCreature (100004, "right-upper-way3"+str, 15f+offset.x, -3f+offset.y, "1");
			CreatureManager.instance.AddCreature (100005, "left-down-way2"+str, -8+offset.x, -10f+offset.y, "1");
			CreatureManager.instance.AddCreature (100006, "right-down-way2"+str, 8+offset.x, -10f+offset.y, "1");
			str += "(d)";
		}
		/*
		CreatureManager.instance.AddCreature(100001, "N-top-up-way3", -41, -1.5f, "2"); // 마법소녀
		CreatureManager.instance.AddCreature(100002, "N-top-down-way3", -41, -13.5f, "2"); // 보고 싶은 사신
		CreatureManager.instance.AddCreature(100003, "N-bottom-way2-left2", -41, -31.5f, "2"); // 없는 책
		CreatureManager.instance.AddCreature(100004, "N-bottom-way2-right2", -13, -31.5f, "2"); // 삐에로

		CreatureManager.instance.AddCreature(100001, "H-top-up-way3", 41, -1.5f, "3"); // 남자 초상화
		CreatureManager.instance.AddCreature(100002, "H-top-down-way3", 41, -13.5f, "3"); // 벽 여인
		CreatureManager.instance.AddCreature(100003, "H-bottom-way2-left2", 41, -31.5f, "3"); // 잭이 없는 콩나무
		CreatureManager.instance.AddCreature(100004, "H-bottom-way2-right2", 13, -31.5f, "3"); // (아무 것도 없는)

		CreatureManager.instance.AddCreature(100001, "T-left-way2", -9.6f, -26, "4"); // 테레지아
		CreatureManager.instance.AddCreature(100002, "T-left-way3", -17.2f, -26, "4"); // 아무말 없는 수녀

		CreatureManager.instance.AddCreature(100003, "T-right-way2", 9.6f, -26, "4"); // 마법소녀
		CreatureManager.instance.AddCreature(100004, "T-right-way3", 17.2f, -26, "4"); // 마법소녀
		*/

		SefiraManager.instance.getSefira(SefiraName.Malkut).InitAgentSkillList();
		foreach (OfficerModel om in OfficeManager.instance.GetOfficerList()) {
			StartCoroutine(om.StartAction());
		}
	}
#endif
	public void OnInit()
	{
		OnLoadMapGraph();

		// temp
		//TempInit();

		// temp

		foreach (SefiraObject sefira in sefiras)
		{
			if (PlayerModel.instance.openedAreaList.Contains(sefira.sefiraName))
				sefira.gameObject.SetActive(true);
			else
				sefira.gameObject.SetActive(false);
		}


	}
	private void UpdateSefiraFog(SefiraObject sefira, float fogDensity)
	{
		/*
		Color color = sefira.sepfiraFog.GetComponent<SpriteRenderer>().color;
		color.a = fogDensity;
		sefira.sepfiraFog.GetComponent<SpriteRenderer>().color = color;

		color = sefira.way1Fog.GetComponent<SpriteRenderer>().color;
		color.a = fogDensity;
		sefira.way1Fog.GetComponent<SpriteRenderer>().color = color;

		color = sefira.way2Fog.GetComponent<SpriteRenderer>().color;
		color.a = fogDensity;
		sefira.way2Fog.GetComponent<SpriteRenderer>().color = color;

		color = sefira.way3Fog.GetComponent<SpriteRenderer>().color;
		color.a = fogDensity;
		sefira.way3Fog.GetComponent<SpriteRenderer>().color = color;

		color = sefira.way4Fog.GetComponent<SpriteRenderer>().color;
		color.a = fogDensity;
		sefira.way4Fog.GetComponent<SpriteRenderer>().color = color;

		color = sefira.elevatorFog1.GetComponent<SpriteRenderer>().color;
		color.a = fogDensity;
		sefira.elevatorFog1.GetComponent<SpriteRenderer>().color = color;

		if (sefira.elevatorFog2.GetComponent<SpriteRenderer>().sprite != null)
		{
			color = sefira.elevatorFog2.GetComponent<SpriteRenderer>().color;
			color.a = fogDensity;
			sefira.elevatorFog2.GetComponent<SpriteRenderer>().color = color;
		}
		*/

		foreach (GameObject fogObject in sefira.fogs)
		{
			SpriteRenderer fogRenderer = fogObject.GetComponent<SpriteRenderer>();
			Color c = fogRenderer.color;
			c.a = fogDensity;
			fogRenderer.color = c;
		}
	}

	private void CheckSefiraFog()
	{
		foreach (SefiraObject sefira in sefiras)
		{
			if (sefira.sefiraName == "1")
			{
				UpdateSefiraFog (sefira, 0f);
				/*
				if (CreatureManager.instance.malkuthState == SefiraState.NOT_ENOUGH_AGENT)
					UpdateSefiraFog (sefira, 0.9f);
				else
					UpdateSefiraFog (sefira, 0f);
				*/
			}

			if (sefira.sefiraName == "2")
			{
				if (CreatureManager.instance.nezzachState == SefiraState.NOT_ENOUGH_AGENT)
					UpdateSefiraFog (sefira, 0.9f);
				else
					UpdateSefiraFog (sefira, 0f);
			}

			if (sefira.sefiraName == "3")
			{
				if (CreatureManager.instance.hodState == SefiraState.NOT_ENOUGH_AGENT)
					UpdateSefiraFog (sefira, 0.9f);
				else
					UpdateSefiraFog (sefira, 0f);
			}

			if (sefira.sefiraName == "4")
			{
				if (CreatureManager.instance.yessodState == SefiraState.NOT_ENOUGH_AGENT)
					UpdateSefiraFog (sefira, 0.9f);
				else
					UpdateSefiraFog (sefira, 0f);
			}
		}
	}

    void Update()
	{
		CheckSefiraFog ();
    }

    void OnEnable()
    {
        Notice.instance.Observe(NoticeName.AddMapObject, this);
		Notice.instance.Observe(NoticeName.AddBloodMapObject, this);
        Notice.instance.Observe(NoticeName.AddPassageObject, this);
        Notice.instance.Observe(NoticeName.AreaOpenUpdate, this);

        Notice.instance.Observe(NoticeName.OpenPassageDoor, this);
        Notice.instance.Observe(NoticeName.ClosePassageDoor, this);

        Notice.instance.Observe(NoticeName.PassageWhitle, this);
        Notice.instance.Observe(NoticeName.PassageBlackOut, this);
        Notice.instance.Observe(NoticeName.PassageAlpha, this);

		Notice.instance.Observe (NoticeName.ResetMapGraph, this);
    }

    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.AreaOpenUpdate, this);
        Notice.instance.Remove(NoticeName.AddMapObject, this);
		Notice.instance.Remove(NoticeName.AddBloodMapObject, this);
        Notice.instance.Remove(NoticeName.AddPassageObject, this);

        Notice.instance.Remove(NoticeName.OpenPassageDoor, this);
        Notice.instance.Remove(NoticeName.ClosePassageDoor, this);

        Notice.instance.Remove(NoticeName.PassageWhitle, this);
        Notice.instance.Remove(NoticeName.PassageBlackOut, this);
        Notice.instance.Remove(NoticeName.PassageAlpha, this);

		Notice.instance.Remove (NoticeName.ResetMapGraph, this);
    }

    public void SetSefiraActive(string sefiraName, bool b)
    {
        foreach (SefiraObject sefira in sefiras)
        {
            if (sefira.sefiraName == sefiraName)
            {
                sefira.gameObject.SetActive(b);
                break;
            }
        }
    }

    private void UpdateSefiraStateView()
    {
        // 나중에
    }
    private void OnUpdateSefiraState()
    {
    }

    private void AddPassageObject(PassageObjectModel model)
    {
        foreach (SefiraObject sefira in sefiras)
        {
            if (sefira.sefiraName == model.GetSefiraName())
            {
                //sefira.AddPassageObject(model);
				sefira.InitPassageObject(model);
                break;
            }
        }
    }

    private void AddPassageDoor(PassageObjectModel model, DoorObjectModel doorModel)
    {
        foreach (SefiraObject sefira in sefiras)
        {
            if (sefira.sefiraName == model.GetSefiraName())
            {
                sefira.AddPassageDoor(model, doorModel);
                break;
            }
        }
    }

    private void AddMapObject(MapObjectModel model)
    {
        foreach (SefiraObject sefira in sefiras)
        {
            if (sefira.sefiraName == model.passage.GetSefiraName())
            {
                sefira.AddMapObject(model);
                break;
            }
        }
    }

	private void AddBloodMapObject(BloodMapObjectModel model)
	{
		foreach (SefiraObject sefira in sefiras)
		{
			if (sefira.sefiraName == model.passage.GetSefiraName())
			{
				sefira.AddBloodMapObject(model);
				break;
			}
		}
	}
    /*
    private void ClosePassage(PassageObjectModel model, DoorObjectModel doorModel)
    {
        Debug.Log("SefiraMapLayer -> CALL ClosePassage");
        foreach (SefiraObject sefira in sefiras)
        {
            if (sefira.sefiraName == model.GetSefiraName())
            {
                sefira.ClosePassage(model, doorModel);
                break;
            }
        }
    }

    private void OpenPassage(PassageObjectModel model, DoorObjectModel doorModel)
    {
        foreach (SefiraObject sefira in sefiras)
        {
            if (sefira.sefiraName == model.GetSefiraName())
            {
                sefira.OpenPassage(model, doorModel);
                break;
            }
        }
    }
    */
    private void OnLoadMapGraph()
    {
        foreach (PassageObjectModel passage in MapGraph.instance.GetPassageObjectList())
        {
            foreach (SefiraObject sefira in sefiras)
            {
                if (sefira.sefiraName == passage.GetSefiraName())
                {
                    sefira.InitPassageObject(passage);
                    break;
                }
            }
        }
		foreach (ElevatorPassageModel e in MapGraph.instance.GetElevatorPassageList())
		{
			foreach (SefiraObject sefira in sefiras)
			{
				if (sefira.sefiraName == e.GetNode().GetAreaName())
				{
					sefira.AddElevatorPassage(e);
					break;
				}
			}
		}
    }

    public SefiraObject GetSefiraObject(string sefiraName) {
        SefiraObject output = null;
        foreach(SefiraObject sefira in this.sefiras){
            if (sefira.sefiraName == sefiraName) {
                output = sefira;
                break;
            }
        }
        return output;
    }

    private PassageObject GetPassageObject(PassageObjectModel model)
    {
        PassageObject po = null;
        SefiraObject sefira = this.GetSefiraObject(model.GetSefiraName());
        if (sefira == null)
        {
            Debug.LogError("Error in Getting SefiraObject");
            return null;
        }
        po = sefira.GetPassageObject(model.GetId());
        return po;
    }

    private void SetPassageBlackOut(PassageObjectModel model) {
        PassageObject unit = GetPassageObject(model);
        unit.SetBlackOut();
    }


    private void SetPassageWhite(PassageObjectModel model)
    {
        PassageObject unit = GetPassageObject(model);
        unit.SetWhite();
    }

    private void SetPassageAlpha(PassageObjectModel model, int value) {
        PassageObject unit = GetPassageObject(model);
        unit.SetAplphaRecursive((float)value);
    }

	private void Reset()
	{
		foreach (SefiraObject sefira in sefiras)
		{
			sefira.Clear ();
		}

		OnInit ();
	}

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.AreaOpenUpdate)
        {
            SetSefiraActive((string)param[0], true);
        }
        else if (notice == NoticeName.AreaUpdate)
        {
            SetSefiraActive((string)param[0], (bool)param[1]);
        }
        else if (notice == NoticeName.AddPassageObject)
        {
            AddPassageObject((PassageObjectModel)param[0]);
        }
        else if (notice == NoticeName.AddMapObject)
        {
            AddMapObject((MapObjectModel)param[0]);
        }
		else if (notice == NoticeName.AddBloodMapObject)
		{
			AddBloodMapObject((BloodMapObjectModel)param[0]);
		}
		else if(notice == NoticeName.AddPassageDoor)
		{
			AddPassageDoor ((PassageObjectModel)param[0], (DoorObjectModel)param [1]);
		}
        else if(notice == NoticeName.PassageBlackOut){
            SetPassageBlackOut((PassageObjectModel)param[0]);
        }
        else if (notice == NoticeName.PassageWhitle) {
            SetPassageWhite((PassageObjectModel)param[0]);
        }

        else if (notice == NoticeName.PassageAlpha) {
            SetPassageAlpha((PassageObjectModel)param[0], (int)param[1]);
        }
		else if(notice == NoticeName.ResetMapGraph)
		{
			
		}

			
        /*else if (notice == NoticeName.ClosePassageDoor)
        {
            ClosePassage((PassageObjectModel)param[0], (DoorObjectModel)param[1]);
        }
        else if (notice == NoticeName.OpenPassageDoor)
        {
            OpenPassage((PassageObjectModel)param[0], (DoorObjectModel)param[1]);
        }*/
    }
}
