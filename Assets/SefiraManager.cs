using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sefira {
    
    public string name;
    public int index;
    public string indexString;

    public Sprite sefiraSprite;
    public bool activated = false;
    public int departmentNum;
    public List<MapNode>[] departmentList;
    public List<OfficerModel> officerList;
    public List<CreatureModel> creatureList = new List<CreatureModel>();
    
    private CreatureModel[] creatureAry;
    private bool[] isWorking;
    public List<int> idleList;
    private List<int> workingList;

    public Sefira(string name, int index, string indexString) {
        this.name = name;
        this.index = index;
        this.indexString = indexString;
        officerList = new List<OfficerModel>();
        workingList = new List<int>();
        idleList = new List<int>();
    }

    public void initCreatureArray() {
        this.creatureList = CreatureManager.instance.GetSelectedList(this.indexString);


    }

    public void initCreatureArray(ref List<CreatureModel> list, ref List<int> idle){
        list = CreatureManager.instance.GetSelectedList(this.indexString);

        Debug.Log("초기화" + list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            idle.Add(i);
        }
    }

    public void initDepartmentNodeList(int i)
    {
        this.departmentList = new List<MapNode>[i];
        this.departmentNum = i;
        for (int a = 0; a < i; a++) {
            this.departmentList[a] = new List<MapNode>();
        }
    }

    public MapNode[] GetDepartNodeToArray(int index) {
        if (index > departmentList.Length) {
            Debug.Log("Error to get department list index");
            return null;
        }
        return departmentList[index].ToArray();
    }

    public CreatureModel GetIdleCreature() {
        Debug.Log("idleList count : " + creatureList.Count);
        if (idleList.Count == 0)
        {
            Debug.Log("All creature is not idle state");
            return null;
        }
        int randCnt = Random.Range(0, idleList.Count);
        isWorking[randCnt] = true;
        idleList.Remove(randCnt);
        workingList.Add(randCnt);
        return creatureAry[randCnt];
    }

    public void EndCreatureWork(CreatureModel cm) {
        int index = creatureList.FindIndex(x => x.Equals(cm));
        if (index == -1) {
            Debug.Log("Cannot find input CreatureModel");
            return;
        }

        isWorking[index] = false;
        idleList.Add(index);
        workingList.Remove(index);
        return;
    }
}

public class SefiraName {
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

public class SefiraManager : MonoBehaviour {
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
