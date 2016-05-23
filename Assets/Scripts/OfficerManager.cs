using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

public class OfficerManager : IObserver {

    public static string[] nameList
        = {
             "Alpha",
             "Beta",
             "Gamma",
             "Delta",
             "Epsilon",
             "Zeta",
             "Eta",
             "Theta",
             "Iota"
           };

    private static OfficerManager _instance;
    public static OfficerManager instance {
        get {
            if (_instance == null)
                _instance = new OfficerManager();
            return _instance;
        }
    }
    private string[] nameRandomList = {"A", "B", "C", "D", "E", "F", "G", "H", "I" ,"J"};
    private int nextInstId = 100;
    private List<OfficerModel> officeList;
    public List<OfficerModel> malkuthList;
    public List<OfficerModel> hodList;
    public List<OfficerModel> nezzachList;
    public List<OfficerModel> yesodList;

    public List<OfficerModel> deadList;

    public int malkuthCount = 15;
    public int hodCount = 15;
    public int nezzachCount = 15;
    public int yesodCount = 15;

    private static int agentImgRange = 9;
    public static AgentModel statReference;

    public bool isLoadedActionList = false;

    public OfficerManager() {
        Init();    
    }

    public void Init() {
        officeList = new List<OfficerModel>();
        
        malkuthList = new List<OfficerModel>();
        hodList = new List<OfficerModel>();
        nezzachList = new List<OfficerModel>();
        yesodList = new List<OfficerModel>();
        deadList = new List<OfficerModel>();

        statReference = new AgentModel(-1);
        AgentTypeInfo info = AgentTypeList.instance.GetData(1);
        statReference.defaultMaxHp = info.hp;
        statReference.defaultMaxMental = info.mental;
        statReference.defaultMovement = info.movement;
        statReference.defaultWork = info.work;
        statReference.level = 1;
        statReference.SetCurrentSefira("0");
    }

	public void Clear()
	{
		Init ();

		Notice.instance.Send (NoticeName.ClearOfficer);
	}

    public OfficerModel CreateOfficerModel(string sefira)
    {
        AgentTypeInfo info = AgentTypeList.instance.GetData(1);
        if (info == null) {
            return null;
        }

		OfficerModel unit = new OfficerModel(nextInstId++, sefira);
        unit.name = GetRandomName(sefira);
        unit.currentSefira = sefira;
        unit.hp = unit.maxMental = info.hp;
        unit.mental = unit.maxMental = info.mental;
        unit.movement = info.movement;
        //unit.workSpeed = info.work;

        unit.gender = info.gender;
        unit.SetModelSprite();

        unit.speechTable = new Dictionary<string, string>(info.speechTable);

        unit.activated = false;
        officeList.Add(unit);

        switch (sefira) { 
            case "1":
                malkuthList.Add(unit);
                break;
            case "2":
                nezzachList.Add(unit);
                break;
            case "3":
                hodList.Add(unit);
                break;
            case "4":
                yesodList.Add(unit);
                break;
            default :
                malkuthList.Add(unit);
                break;
        }
        //Debug.Log("created" + unit.name);
        //OfficerLayer.currentLayer.AddOfficer(unit);

        SefiraManager.instance.GetSefira(sefira).AddUnit(unit);

        ActivateOfficer(unit);
        return unit;
    }

    public string SetRandomSprite() {
        int i = Random.Range(0, agentImgRange);
        return nameRandomList[i];
    }

    public string SetRandomSprite(int r)
    {
        int i = Random.Range(0, r);
        return nameRandomList[i];
    }

    public OfficerModel[] GetOfficerListBySefira(string sefira) { 
        switch(sefira){
            case "0":
                return OfficerManager.instance.malkuthList.ToArray();
                break;
            case "1":
                return OfficerManager.instance.malkuthList.ToArray();
                break;
            case "2":
                return OfficerManager.instance.nezzachList.ToArray();
                break;
            case "3":
                return OfficerManager.instance.hodList.ToArray();
                break;
            case "4":
                return OfficerManager.instance.yesodList.ToArray();
                break;
            default:
                return null;
        }
    }

    private static string GetRandomName(string sefira) {
        int ranInt = Random.Range(0, nameList.Length-1);
        string code;
        switch (sefira) { 
            case "0":
                code = OfficerManager.instance.malkuthList.Count.ToString();
                break;
            case "1":
                code = OfficerManager.instance.malkuthList.Count.ToString();
                break;
            case "2":
                code = OfficerManager.instance.nezzachList.Count.ToString();
                break;
            case "3":
                code = OfficerManager.instance.hodList.Count.ToString();
                break;
            case "4":
                code = OfficerManager.instance.yesodList.Count.ToString();
                break;
            default:
                code = OfficerManager.instance.officeList.Count.ToString();
                break;
        }
        return nameList[ranInt] + code;
    }

    public void ActivateOfficer(OfficerModel unit) {
        unit.activated = true;
        Notice.instance.Observe(NoticeName.FixedUpdate, unit);
        Notice.instance.Send(NoticeName.AddOfficer, unit);
    }

    public void DeactivateOfficer(OfficerModel unit) {
        unit.activated = false;
        Notice.instance.Remove(NoticeName.FixedUpdate, unit);
        officeList.Remove(unit);
        
        unit.currentSefira = "0";//No idea
    }

    public void RemoveOfficer(OfficerModel model) {
        switch (model.currentSefira)
        {
            case "1":
                malkuthList.Remove(model);
                break;
            case "2":
                nezzachList.Remove(model);
                break;
            case "3":
                hodList.Remove(model);
                break;
            case "4":
                yesodList.Remove(model);
                break;
            default:
                malkuthList.Remove(model);
                break;
        }
        Notice.instance.Remove(NoticeName.FixedUpdate, model);
        officeList.Remove(model);
        Notice.instance.Remove(NoticeName.RemoveOfficer, model);
    }

    public void ClearOfficer() {
        foreach (OfficerModel model in officeList) {
            Notice.instance.Remove(NoticeName.FixedUpdate, model);
            Notice.instance.Send(NoticeName.RemoveOfficer, model);
        }
        OfficerLayer.currentLayer.ClearOfficer();
        officeList = new List<OfficerModel>();
    }

    public OfficerModel[] GetOfficerList() {
        return officeList.ToArray();
    }

    private static bool TryGetValue<T>(Dictionary<string, object> dic, string name, ref T field) {
        object output;
        if (dic.TryGetValue(name, out output)) {
            field = (T)output;
            return true;
        }
        return false;
    }

    public Dictionary<string, object> GetSaveData() {
        Dictionary<string, object> dic = new Dictionary<string, object>();

        dic.Add("nextInstId", nextInstId); 

        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        foreach (OfficerModel officer in officeList) {
            Dictionary<string, object> officerData = officer.GetSaveData();
            list.Add(officerData);
        }
        dic.Add("officerList", list);
        return dic;
    }

    public void LoadData() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/test.txt", FileMode.Open);
        Dictionary<string, object> dic = (Dictionary<string, object>)bf.Deserialize(file);
        file.Close();
        //LoadData(dic);
    
    }

    public void LoadData(Dictionary<string, object> dic) {
        TryGetValue(dic, "nextInstId", ref nextInstId);
        List<Dictionary<string, object>> officerDataList = new List<Dictionary<string, object>>();
        TryGetValue(dic, "officerList", ref officerDataList);
        foreach (Dictionary<string, object> data in officerDataList) {
            int officerId = 0;
            string sefira = "";
            
            TryGetValue(data, "instanceId", ref officerId);
            TryGetValue(data, "curretnSefira", ref sefira);

            OfficerModel model = new OfficerModel(officerId, sefira);
            model.LoadData(data);
            officeList.Add(model);
            Notice.instance.Send(NoticeName.AddOfficer, model);
        }
    }

    public OfficerModel[] GetNearOfficers(MovableObjectNode node) {
        List<OfficerModel> output = new List<OfficerModel>();
        foreach (OfficerModel officer in officeList) {
			if (officer.isDead ())
				continue;
			
			/*
            if (node.CheckInRange(officer.GetMovableNode())) {
                output.Add(officer);
            }
            */

			Vector3 dist = node.GetCurrentViewPosition () - officer.GetMovableNode ().GetCurrentViewPosition ();
			if (node.GetPassage () == officer.GetMovableNode ().GetPassage () &&
				dist.sqrMagnitude <= 3) {
				output.Add(officer);
			}
        }
        return output.ToArray();
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.AddOfficer) {
            Debug.Log("nonstop");
        }
    }

    public AgentModel GetReferenceStat(float scale) {
        AgentModel output = new AgentModel(statReference.instanceId);
        output.defaultMaxHp = (int)(output.defaultMaxHp * scale);
        output.defaultMaxMental = (int)(output.defaultMaxMental * scale);
        output.defaultMovement = (int)(output.defaultMovement * scale);
        output.defaultWork = (int)(output.defaultWork * scale);
        return output;
    }
}
