﻿using UnityEngine;
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
	/*
    public void sefiraFog(int sefiraNum, float fogDensity)
    {
        Color color = sefiras[sefiraNum].sepfiraFog.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].sepfiraFog.GetComponent<SpriteRenderer>().color = color;

        color = sefiras[sefiraNum].way1Fog.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].way1Fog.GetComponent<SpriteRenderer>().color = color;

        color = sefiras[sefiraNum].way2Fog.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].way2Fog.GetComponent<SpriteRenderer>().color = color;

        color = sefiras[sefiraNum].way3Fog.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].way3Fog.GetComponent<SpriteRenderer>().color = color;

        color = sefiras[sefiraNum].way4Fog.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].way4Fog.GetComponent<SpriteRenderer>().color = color;

        color = sefiras[sefiraNum].elevatorFog1.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].elevatorFog1.GetComponent<SpriteRenderer>().color = color;

        if (sefiras[sefiraNum].elevatorFog2.GetComponent<SpriteRenderer>().sprite != null)
        {
            color = sefiras[sefiraNum].elevatorFog2.GetComponent<SpriteRenderer>().color;
            color.a = fogDensity;
            sefiras[sefiraNum].elevatorFog2.GetComponent<SpriteRenderer>().color = color;
        }

        foreach (GameObject fogObject in sefiras[sefiraNum].fogs)
        {
            SpriteRenderer fogRenderer = fogObject.GetComponent<SpriteRenderer>();
            Color c = fogRenderer.color;
            c.a = fogDensity;
            fogRenderer.color = c;
        }

    }
    */
	private void UpdateSefiraFog(SefiraObject sefira, float fogDensity)
	{
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
		/*
        if (CreatureManager.instance.malkuthState == SefiraState.NOT_ENOUGH_AGENT)
        {
            sefiraFog(0,0.9f);
        }

        else
        {
            sefiraFog(0, 0f);
        }

        if (CreatureManager.instance.nezzachState == SefiraState.NOT_ENOUGH_AGENT)
        {
            sefiraFog(1, 0.9f);
        }

        else
        {
            sefiraFog(1, 0f);
        }

        if (CreatureManager.instance.hodState == SefiraState.NOT_ENOUGH_AGENT)
        {
            sefiraFog(2, 0.9f);
        }

        else
        {
            sefiraFog(2, 0f);
        }

        if (CreatureManager.instance.yessodState == SefiraState.NOT_ENOUGH_AGENT)
        {
            sefiraFog(3, 0.9f);
        }

        else
        {
            sefiraFog(3, 0f);
        }
        */
    }

    void OnEnable()
    {
        Notice.instance.Observe(NoticeName.AddMapObject, this);
        Notice.instance.Observe(NoticeName.AddPassageObject, this);
        Notice.instance.Observe(NoticeName.AreaOpenUpdate, this);

        Notice.instance.Observe(NoticeName.OpenPassageDoor, this);
        Notice.instance.Observe(NoticeName.ClosePassageDoor, this);
    }

    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.AreaOpenUpdate, this);
        Notice.instance.Remove(NoticeName.AddMapObject, this);
        Notice.instance.Remove(NoticeName.AddPassageObject, this);

        Notice.instance.Remove(NoticeName.OpenPassageDoor, this);
        Notice.instance.Remove(NoticeName.ClosePassageDoor, this);
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
		else if(notice == NoticeName.AddPassageDoor)
		{
			AddPassageDoor ((PassageObjectModel)param[0], (DoorObjectModel)param [1]);
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
