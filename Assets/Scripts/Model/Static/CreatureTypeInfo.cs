using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

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
	public long id;
	public string name;
	public string codeId;
	public string level;

    public int stackLevel;
    public int observeLevel;

	public string intelligence;

	public CreatureAttackType attackType;

	public float attackProb;
	public int physicsDmg;
	public int mentalDmg;
    
	public float horrorProb;
	public int horrorDmg;

			
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

    public string desc;
    public string observe;
    public string[] observeList;

    public XmlNodeList nodeInfo;
    public XmlNodeList edgeInfo;

}
