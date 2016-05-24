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

	public string gender;
	public int level;
	public int workDays;

	public Dictionary<string, string> speechTable;
}
