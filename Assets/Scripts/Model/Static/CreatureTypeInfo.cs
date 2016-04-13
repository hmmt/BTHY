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
    public class ObserveTable {

        public int codeNo;
        public int portrait;
        public int size;
        public int weight;
        public int name;
        public int attackType;
        public int creatureType;
        public int horrorLevel;
        public int riskLevel;
        public int intelligence;
        public int physical;
        public int mental;
        public int specialName;
        public int specialInfo;
        public int aggression;
        public int gorgeous;
        public int rejectWork;
        public int preferWork;
        public int solution;

        public List<int> desc;
        public List<int> record;

        public ObserveTable() {
            this.codeNo = 0;
            this.portrait = 0;
            this.size = 0;
            this.weight = 0;
            this.name = 0;
            this.attackType = 0;
            this.creatureType = 0;
            this.horrorLevel = 0;
            this.riskLevel = 0;
            this.intelligence = 0;
            this.physical = 0;
            this.mental = 0;
            this.specialInfo = 0;
            this.specialName = 0;
            this.aggression = 0;
            this.gorgeous = 0;
            this.aggression = 0;
            this.preferWork = 0;
            this.solution = 0;

            this.desc = new List<int>();
            this.record = new List<int>();
        }

    }
    
	public long id;

    public int stackLevel;

	public CreatureAttackType attackType;

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
	public int energyPointChange;

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
    public ObserveTable observeTable = new ObserveTable();

    public int MaxObserveLevel;
    public int CurrentObserveLevel;

    public int sizeLevel;
    public string weightLevel;
    public string name;
    public string codeId;
    public string level;//riskLevel
    public string portraitSrc;
    public Sprite tempPortrait;
    public string creatureType;
    public int horrorLevel;

	public int intelligence;
    public int physicalAttackLevel;
    public int mentalAttackLevel;
    public string specialSkillName;
    public string specialSkillInfo;
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
