using UnityEngine;
using System.Collections;

public class AgentModel {

	public long metadataId = -1;

	public int hp = 3;
	public int mental = 3;
	public float speed;

	public long skillId1;
	public long skillId2;
	public long skillId3;
	                                                                       
	public int[] abilities;

	public string preferSkillType;
	public float preferP;
	public string skillType;
	public float fp;


	public string name = "unknown";
	public float moveSpeed = 1.2f;
	public float ability = 1.0f;

	public int movement;
	public int work;

	public AgentModel()
	{
	}

	public AgentModel(AgentTypeInfo info)
	{
		metadataId = info.id;

		name = info.name;
		hp = info.hp;
		mental = info.mental;
		work = info.work;
	}

	public AgentModel clone()
	{
		AgentModel output = new AgentModel ();
		output.name = name;
		output.hp = hp;
		output.mental = mental;
		output.moveSpeed = moveSpeed;

		return output;
	}
}
