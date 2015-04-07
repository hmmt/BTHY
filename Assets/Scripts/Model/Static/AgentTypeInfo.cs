using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentTypeInfo {
	public long id;
	public string name;
	public int hp;
	public int mental;
	public int movement;
	public int work;

	public string prefer;
	public int preferBonus;

	public string reject;
	public int rejectBonus;

	public SkillTypeInfo directSkill;
	public SkillTypeInfo indirectSkill;
	public SkillTypeInfo blockSkill;

	public string imgsrc;

	public Dictionary<string, string> speechTable;

	public string panicType;
}
