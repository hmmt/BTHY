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
    public int section;
    public float genValue;

    public EnergyGenInfo(int section, float genValue)
    {
        this.section = section;
        this.genValue = genValue;
    }

    public static int SectionSortComparison(EnergyGenInfo a, EnergyGenInfo b)
    {
        return a.section - b.section;
    }
}

[System.Serializable]
public class FeelingSectionInfo
{
    public int section;
    public float energyGenValue;
    public CreatureFeelingState feelingState;
    public SkillBonusInfo[] preferList;
    public SkillBonusInfo[] rejectList;
}

[System.Serializable]
public class CreatureTypeInfo
{
	public long id;
	public string name;
	public string codeId;
	public string level;

    public int stackLevel;
    public int observeLevel;

	public string attackType;
	public string intelligence;
    
	public float horrorProb;
	public int horrorDmg;
			
	public float physicsProb;
	public int physicsDmg;
			
	public float mentalProb;
	public int mentalDmg;
			
	public int feelingMax;
	public float feelingDownProb;
	public float feelingDownValue;

	//public SkillTypeInfo specialSkill;

    public FeelingSectionInfo[] feelingSectionInfo;

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
