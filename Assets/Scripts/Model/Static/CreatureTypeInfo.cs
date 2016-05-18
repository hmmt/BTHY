using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public enum CreatureType { 
    BIO,//biology
    ABS,//abstract
    THING,//thing
    NONE,//dummy
    ALL,//for working type
}

public enum CreatureFeelingState
{
    GOOD,
    NORM,
    BAD
}
public enum SkillBonusAttr
{
    CATEGORY_TYPE,
    CATEGORY_ID
}

public enum CreatureAttackType
{
	PHYSICS,
	MENTAL,
	COMPLEX
}
[System.Serializable]
public class SkillBonusInfo
{
    public SkillBonusAttr attr;

    public string skillType;
    public long skillId;
    public float bonus;
}

[System.Serializable]
public class EnergyGenInfo
{
    public int upperBound;
    public float genValue;

	public EnergyGenInfo(int upperBound, float genValue)
    {
		this.upperBound = upperBound;
        this.genValue = genValue;
    }

    public static int SectionSortComparison(EnergyGenInfo a, EnergyGenInfo b)
    {
		return a.upperBound - b.upperBound;
    }
}

/*
[System.Serializable]
public class FeelingSectionInfo
{
    public int section;
    public float energyGenValue;
    public CreatureFeelingState feelingState;
    public SkillBonusInfo[] preferList;
    public SkillBonusInfo[] rejectList;
}
*/
[System.Serializable]
public class CreatureTypeInfo
{
    public static string[] stringData = { "codeNo", "portrait", "weight", "name", "creatureType", "specialName", "specialInfo" };
    public static string[] intData = { "horrorLevel", "intelligence", "physical", "mental", "aggression", "gorgeous" };
    /*
    public class CreatureTypeElement {
        public object data;
        public int openLevel;
    }

    public class CreatureTypeElementTable {
        public List<CreatureTypeElement> list;
        public string type;

        public CreatureTypeElementTable()
        {
            this.list = new List<CreatureTypeElement>();
        }

        public T GetData<T>(int level) where T : Object {
            T output = null;
            foreach (CreatureTypeElement element in list) {
                if (element.openLevel == level) {
                    output = (T)element.data;
                    break;
                }
            }
            return output;
        }
    }*/
    /*
    public class CreatureTypeElement {
        public object data;
        public int openLevel;
        public string type;
    }

    public class CreatureTypeDictionary {
        public Dictionary<string, CreatureTypeElement> dictionary = new Dictionary<string, CreatureTypeElement>();
        public string currentItemName;

        public void Add( CreatureTypeElement element) {
            dictionary.Add(currentItemName, element);
        }

        public CreatureTypeElement GetNamedItem() { 
            
        }
    }

    public class CreatureTypeElementTable {
        public Dictionary<string, CreatureTypeElement> elementDictionary;
        public Dictionary<int, List<CreatureTypeElement>> dictionary = new Dictionary<int,List<CreatureTypeElement>>();

        public void AddList(List<CreatureTypeElement> list, int level) {
            dictionary.Add(level, list);
        }

        public List<CreatureTypeElement> GetListByLevel(int level) {
            List<CreatureTypeElement> output = null;
            if (dictionary.TryGetValue(level, out output)) {
                return output;
            }
            Debug.Log("No data founded current level");
            return null;
        }
    }*/

    public enum CreatureDataType { 
        SPRITE,
        INT,
        LONG,
        SHORT,
        STRING,
        ETC
    }

    public class CreatureData {
        public object data;
        public int openLevel;
    }

    public class CreatureDataList {
        public string itemName;
        List<CreatureData> list = new List<CreatureData>();

        public object GetData(int level) {
            object output = null;
            foreach (CreatureData data in list) {
                if (data.openLevel <= level)
                {
                    output = data;
                }
                if (data.openLevel > level) {
                    break;
                }
            }
            return output;
        }

        public int GetLevel(int level) {
            int output = 1000;
            foreach (CreatureData data in list)
            {
                if (data.openLevel <= level)
                {
                    output = data.openLevel;
                }
                if (data.openLevel > level)
                {
                    break;
                }
            }
            return output;
        }

        public void AddData(CreatureData data) {
            this.list.Add(data);
        }

        public int GetCount() {
            return this.list.Count; 
        }
    }

    public class CreatureDataTable {
        public Dictionary<string, CreatureDataList> dictionary = new Dictionary<string, CreatureDataList>();

        public CreatureDataList GetList(string key) {
            CreatureDataList output = null;
            if (dictionary.TryGetValue(key, out output)) {
                return output;
            }
            Debug.LogError("Dictionary error in collection");
            return null;
        }

        public void PrintElementName() {
            foreach (KeyValuePair<string, CreatureDataList> list in dictionary) {
                Debug.Log(list.Value.itemName + " " + list.Value.GetCount());
            }
        }
    }



    public class ObserveTable {
        public List<int> desc;
        public List<int> record;

        public ObserveTable() {
            this.desc = new List<int>();
            this.record = new List<int>();
        }

    }
    
	public long id;

    public int stackLevel;

	public CreatureAttackType attackType = CreatureAttackType.PHYSICS;

	public float attackProb;
	public int physicsDmg;
	public int mentalDmg;
	public int horrorDmg;

    public float horrorProb;
    public float horrorMin;
    public float horrorMax;
    		
	//public float physicsProb;
			
	//public float mentalProb;
			
	public int feelingMax;
	//public float feelingDownProb;
	public float feelingDownValue;

	//public SkillTypeInfo specialSkill;

    //public FeelingSectionInfo[] feelingSectionInfo;

	public EnergyGenInfo[] energyGenInfo;
	//public int energyPointChange;

	public string imgsrc = "";
    public string roomsrc;
    public string framesrc;
    public string animSrc =""; // 

    public string roomReturnSrc; // 제압당한 환상체 돌아가는 이미지

	public string script;
    //public string animatorScript;

	public Dictionary<string, string> typoTable;
	public Dictionary<string, string> narrationTable;
    public Dictionary<string, string> soundTable;

    public string observe;
    public string[] observeList;

    public XmlNodeList nodeInfo;
    public XmlNodeList edgeInfo;

	public GameObject workerAnim;


    //이하 도감정보
    public CreatureDataTable dataTable = new CreatureDataTable();
    public ObserveTable observeTable = new ObserveTable();

    public int MaxObserveLevel;
    public int CurrentObserveLevel;

    public int sizeLevel;
    public string weightLevel;
    public string name;
    public string collectionName
    {
        get
        {
            string output = (string)this.dataTable.GetList("name").GetData(CurrentObserveLevel);
            if (output == null)
            {
                return "NoData";
            }
            return output;
        }
    }
    public string codeId
    {
        get
        {
            string output = (string)this.dataTable.GetList("codeNo").GetData(CurrentObserveLevel);
            if (output == null)
            {
                return "NoData";
            }
            return output;
        }
    }
    public string level
    {
        get
        {
            //int output = (int)this.dataTable.GetList("horrorLevel").GetData(CurrentObserveLevel);
            int output = 0;
            if (output == null)
            {
                return "NoData";
            }
            return output.ToString();
        }
    }//riskLevel
    public string portraitSrc
    {
        get
        {
            string output = (string)this.dataTable.GetList("portrait").GetData(CurrentObserveLevel);
            if (output == null)
            {
                return "";
            }
            return output;
        }
    }
    public Sprite tempPortrait;
    public string creatureType;
    public int horrorLevel
    {
        get
        {
            int output = (int)this.dataTable.GetList("horrorLevel").GetData(CurrentObserveLevel);
            if (output == null)
            {
                return 0;
            }
            return output;
        }
    }

    public int intelligence
    {
        get
        {
            int output = (int)this.dataTable.GetList("intelligence").GetData(CurrentObserveLevel);
            if (output == null)
            {
                return 0;
            }
            return output;
        }
    }
    public int physicalAttackLevel
    {
        get
        {
           // int output = (int)this.dataTable.GetList("physical").GetData(CurrentObserveLevel);
            int output = 0;
            if (output == null)
            {
                return 0;
            }
            return output;
        }
    }
    public int mentalAttackLevel
    {
        get
        {
            //int output = (int)this.dataTable.GetList("mental").GetData(CurrentObserveLevel);
            int output = 0;
            if (output == null)
            {
                return 0;
            }
            return output;
        }
    }
    public string specialSkillName{
        get
        {
            string output = (string)this.dataTable.GetList("specialName").GetData(CurrentObserveLevel);
            if (output == null)
            {
                return "NoData";
            }
            return output;
        }
    }
    public string specialSkillInfo
    {
        get
        {
            string output = (string)this.dataTable.GetList("specialInfo").GetData(CurrentObserveLevel);
            if (output == null)
            {
                return "NoData";
            }
            return output;
        }
    }

    public int aggressionLevel;
    public int gorgeousLevel;
    public List<long> rejectWorkList = new List<long>();
    public List<long> preferWorkList = new List<long>();
    public string solution;

    public List<string> desc = new List<string>();
    public List<string> observeRecord = new List<string>();
    //도감정보 끝


	// temp for proto
	public Dictionary<long, float> workEfficiency = new Dictionary<long, float>();

    
}
