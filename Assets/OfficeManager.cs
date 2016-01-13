using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

public class OfficeManager : IObserver {

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

    private static OfficeManager _instance;
    public static OfficeManager instance {
        get {
            if (_instance == null)
                _instance = new OfficeManager();
            return _instance;
        }
    }
    private string[] nameRandomList = {"A", "B", "C", "D", "E", "F", "G", "H", "I" ,"J"};
    private int nextInstId = 1;
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

    public OfficeManager() {
        Init();    
    }

    public void Init() {
        officeList = new List<OfficerModel>();
        
        malkuthList = new List<OfficerModel>();
        hodList = new List<OfficerModel>();
        nezzachList = new List<OfficerModel>();
        yesodList = new List<OfficerModel>();
        deadList = new List<OfficerModel>();

    }

    public OfficerModel CreateOfficerModel(string sefira)
    {
        AgentTypeInfo info = AgentTypeList.instance.GetData(1);
        if (info == null) {
            return null;
        }

        OfficerModel unit = new OfficerModel(nextInstId++, "1");
        unit.name = GetRandomName(sefira);

        unit.hp = unit.maxMental = info.hp;
        unit.mental = unit.maxMental = info.mental;
        unit.movement = info.movement;
        unit.workSpeed = info.work;

        unit.gender = info.gender;

        unit.speechTable = new Dictionary<string, string>(info.speechTable);
        unit.panicType = info.panicType;

        unit.faceSpriteName = SetRandomSprite(8);
        unit.hairSpriteName = SetRandomSprite(9);
        unit.bodySpriteName = SetRandomSprite(1);
        unit.panicSpriteName = SetRandomSprite(3);

        unit.GetPortrait("hair", unit.hairSpriteName);
        unit.GetPortrait("face", unit.faceSpriteName);
        unit.GetPortrait("body", null);

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
        Debug.Log("created" + unit.name);
        //OfficerLayer.currentLayer.AddOfficer(unit);

        SefiraManager.instance.getSefira(sefira).AddUnit(unit);

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
                return OfficeManager.instance.malkuthList.ToArray();
                break;
            case "1":
                return OfficeManager.instance.malkuthList.ToArray();
                break;
            case "2":
                return OfficeManager.instance.nezzachList.ToArray();
                break;
            case "3":
                return OfficeManager.instance.hodList.ToArray();
                break;
            case "4":
                return OfficeManager.instance.yesodList.ToArray();
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
                code = OfficeManager.instance.malkuthList.Count.ToString();
                break;
            case "1":
                code = OfficeManager.instance.malkuthList.Count.ToString();
                break;
            case "2":
                code = OfficeManager.instance.nezzachList.Count.ToString();
                break;
            case "3":
                code = OfficeManager.instance.hodList.Count.ToString();
                break;
            case "4":
                code = OfficeManager.instance.yesodList.Count.ToString();
                break;
            default:
                code = OfficeManager.instance.officeList.Count.ToString();
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
        
        unit.sefira = "0";//No idea
    }

    public void RemoveOfficer(OfficerModel model) {
        switch (model.sefira)
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
            if (node.CheckInRange(officer.GetMovableNode())) {
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
}
