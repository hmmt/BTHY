using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

[System.Serializable]
public class SkillBonusInfo
{
    public string skillType;
    public float bonus;
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

	public SkillTypeInfo specialSkill;

	public float[] genEnergy;
			
    public SkillBonusInfo[] preferList;

    public SkillBonusInfo[] rejectList;

	public string imgsrc;
    public string roomsrc;
    public string framesrc;

	public string script;

	public Dictionary<string, string> typoTable;
	public Dictionary<string, string> narrationTable;

    public Dictionary<string, string> soundTable;

    public string desc;
    public string observe;
    public string[] observeList;

    public XmlNodeList nodeInfo;
    public XmlNodeList edgeInfo;
}
