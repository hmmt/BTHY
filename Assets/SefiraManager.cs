using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sefira
{
    public string name;
    public int index;
    public string indexString;

    public int OfficerMentalReturn;
    public Sprite sefiraSprite;
    public bool activated = false;
    public int departmentNum;
    public List<MapNode>[] departmentList;
    public List<OfficerModel> officerList;
    public List<AgentModel> agentList;
    public List<CreatureModel> creatureList;
    public List<SkillTypeInfo>[] agentSkill;//속한 직원들의 스킬 정보

    private int maxOfficerCnt = 15;
    private CreatureModel[] creatureAry;
    private bool[] isWorking;
    private List<int> idleList;
    private List<int> workingList;
    private int officerCnt;

    public Sefira(string name, int index, string indexString)
    {
        this.name = name;
        this.index = index;
        this.indexString = indexString;
        creatureList = new List<CreatureModel>();
        officerList = new List<OfficerModel>();
        workingList = new List<int>();
        idleList = new List<int>();
        agentSkill = new List<SkillTypeInfo>[3];
        agentList = new List<AgentModel>();

        for (int i = 0; i < 3; i++) {
            agentSkill[i] = new List<SkillTypeInfo>();
        }

        officerCnt = 0;
        OfficerMentalReturn = 20;
        
    }

    public void AddUnit(OfficerModel add) {
        officerList.Add(add);
        officerCnt++;
    }

    public void AddAgent(AgentModel add) {
        agentList.Add(add); Debug.Log("agentList");
        foreach (AgentModel am in agentList) {
                        Debug.Log(am.name);
        }
    }

    public void RemoveAgent(AgentModel unit) {
        agentList.Remove(unit);
    }

    public void initCreatureArray()
    {

        creatureAry = creatureList.ToArray();
        for (int i = 0; i < creatureList.Count; i++)
        {
            idleList.Add(i);
        }
        isWorking = new bool[creatureList.Count];
    }

    public void initDepartmentNodeList(int i)
    {
        this.departmentList = new List<MapNode>[i];
        
        this.departmentNum = i;

        for (int a = 0; a < i; a++)
        {
            this.departmentList[a] = new List<MapNode>();
        }
    }

    private void AssignOfficerDept() {
        int max = officerList.Count;
        int dept = 0;


        for (int j = 0; j < max; j++)
        {
            if (departmentNum == 0) departmentNum = 1;
            officerList[j].deptNum = (dept++) % departmentNum;
        }
    }

    public void printDeptNodeList()
    {
        foreach (List<MapNode> list in departmentList)
        {
            foreach (MapNode mn in list)
            {
                //Debug.Log(mn.GetGroupName());
            }
            Debug.Log(" ");
        }
    }

    public MapNode[] GetDepartNodeToArray(int index)
    {
        if (index > departmentList.Length)
        {
            Debug.Log("Error to get department list index");
            return null;
        }
        return departmentList[index].ToArray();
    }

    public MapNode GetDepartNodeByRandom(int index) {
        int randVal = UnityEngine.Random.Range(0, departmentList[index].Count);

        return departmentList[index][randVal];
    }

    public MapNode GetOtherDepartNode(int index) {
        int pos;
        while ((pos = UnityEngine.Random.Range(0, departmentList.Length)) == index) {
            continue;
        }
        int randVal = UnityEngine.Random.Range(0, departmentList[pos].Count);
        return departmentList[pos][randVal];
    }

    public CreatureModel GetIdleCreature()
    {
        if (idleList.Count == 0)
        {
            //Debug.Log("All creature is not idle state");
            return null;
        }
        int randCnt = Random.Range(0, idleList.Count);
        int randVal = idleList[randCnt];
        isWorking[randVal] = true;
        idleList.Remove(randVal);
        workingList.Add(randVal);
        return creatureAry[randVal];
    }

    public void EndCreatureWork(CreatureModel cm)
    {
        int index = -1;
        index = creatureList.FindIndex(x => x.Equals(cm));
        if (index == -1)
        {
            Debug.Log("Cannot find input CreatureModel");
            return;
        }

        isWorking[index] = false;
        idleList.Add(index);
        workingList.Remove(index);
        return;
    }

    public int GetOfficerCount() {
        return officerCnt;
    }

    public void WorkerDie(WorkerModel worker) { 
        //나중에 양쪽 다 커버 가능하도록 고칠 것 ->관리직, 사무직
        officerList.Remove((OfficerModel)worker);
        officerCnt--;
    }

    public void SetOfficerMentalRecoverValue(int value) {
        this.OfficerMentalReturn = value;
    }

    public int GetOfficerMentalRecoverValue() {
        return this.OfficerMentalReturn;
    }

    public void initOfficerGroup() {
        List<MapNode>[] tempList = new List<MapNode>[departmentList.Length];
        for (int i = 0; i < departmentList.Length; i++) {
            tempList[i] = new List<MapNode>(departmentList[i]);
        }

        /*
        for (int i = 0; i < maxOfficerCnt; i++) {
            OfficeManager.instance.CreateOfficerModel(indexString);
        }
        */

        OfficeManager.instance.CreateOfficerModel(indexString);
        AssignOfficerDept();
        foreach (OfficerModel om in officerList) {
            int deptNum = om.deptNum;
            int randVal = UnityEngine.Random.Range(0, tempList[deptNum].Count);
            MapNode tempNode = tempList[deptNum][randVal];

            //om.ReturnToSefira();
            om.MoveToNode(tempNode.GetId());
            //tempList[deptNum].Remove(tempNode);
        }
    }

    public void InitAgentSkillList() {
        
    }

    public SkillTypeInfo[] GetSkills() {
        return null;
    }

    private bool CheckSkillDuplicate(AgentModel model) {
        for (int i = 0; i < 3; i++) {


            foreach (SkillTypeInfo s in agentSkill[i]) { 
                
            }
        }
        return false;
    }
}

public class OfficerDept
{
    public List<MapNode> deptNode = new List<MapNode>();
    public int index;

    public OfficerDept(int i)
    {
        index = i;
    }

    public MapNode GetRandomNode()
    {
        int randVal = -1;
        if (0 == deptNode.Count)
        {
            Debug.Log("Not initialized or assigned MapNode to dept " + this.index);
            return null;
        }
        randVal = UnityEngine.Random.Range(0, deptNode.Count);
        return deptNode[randVal];
    }
}

public class SefiraName {
    public enum Sefira { 
        MALKUT,
        YESOD,
        HOD,
        NETACH,
        TIPERERTH,
        GEBURAH,
        CHESED,
        BINAH,
        CHOKHMAH,
        KETHER,
        DAAT
    }

    public static string Malkut = "Malkut";
    public static string Yesod = "Yesod";
    public static string Hod = "Hod";
    public static string Netach = "Netach";
    public static string Tiphereth = "Tiphereth";
    public static string Geburah = "Geburah";
    public static string Chesed = "Chesed";
    public static string Binah = "Binah";
    public static string Chokhmah = "Chokhmah";
    public static string Kether = "Kether";
    public static string Daat = "Daat";
}

public class SefiraManager {
    private static SefiraManager _instance;
    public static SefiraManager instance {
        get {
            if (_instance == null)
                _instance = new SefiraManager();
            return _instance;
        }
    }

    public int sefiraIndexMax = 4;
    private Sefira[] refSefira;
    public List<Sefira> sefiraList;
    
    private SefiraManager() {
        Init();
        Debug.Log("sefiraManager initialized");
    }

    private void Init() {
        sefiraList = new List<Sefira>();
        refSefira = new Sefira[11];
        refSefira[0] = new Sefira(SefiraName.Malkut, 1, "1");
        refSefira[1] = new Sefira(SefiraName.Yesod, 2, "2");
        refSefira[2] = new Sefira(SefiraName.Hod, 3, "3");
        refSefira[3] = new Sefira(SefiraName.Netach, 4, "4");
        refSefira[4] = new Sefira(SefiraName.Tiphereth, 5, "5");
        refSefira[5] = new Sefira(SefiraName.Geburah, 6, "6");
        refSefira[6] = new Sefira(SefiraName.Chesed, 7, "7");
        refSefira[7] = new Sefira(SefiraName.Binah, 8, "8");
        refSefira[8] = new Sefira(SefiraName.Chokhmah, 9, "9");
        refSefira[9] = new Sefira(SefiraName.Kether, 10, "10");
        refSefira[10] = new Sefira(SefiraName.Daat, 11, "11");

        refSefira[0].sefiraSprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Malkuth_Icon");
        refSefira[3].sefiraSprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Netzzach_Icon");
        refSefira[2].sefiraSprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Hod_Icon");
        refSefira[1].sefiraSprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Yessod_Icon");
        
        for (int i = 0; i < sefiraIndexMax; i++) {
            sefiraList.Add(refSefira[i]);
        }
    }

    public Sefira getSefira(int index) {
        if (index > sefiraIndexMax|| index < 0) {
            Debug.Log("out of sefira index");
            return null;
        }
        foreach (Sefira s in sefiraList) {
            if (s.index.Equals(index)) return s;
        }
        return null;
    }

     /// <summary>
     /// Searched by index string
     /// </summary>
     /// <param name="str"></param>
     /// <returns></returns>
    public Sefira getSefira(string str) {
        foreach (Sefira s in sefiraList) {
            if (s.indexString.Equals(str) || s.name.Equals(str))
            {
                return s;
            }
        }
        return null;
    }


}
