using UnityEngine;
using System.Collections;

// UNUSED
public class SelectActionWindow : MonoBehaviour {

	private Vector2 offset = new Vector2 (0, 0);

	private AgentModel[] selectedAgentList;
	private CreatureModel targetCreature;

	// Use this for initialization
	void Awake () {
		offset = transform.localPosition;
		UpdatePosition ();
	}

	void FixedUpdate()
	{
		UpdatePosition ();
	}
	
	private void UpdatePosition()
	{
		if(targetCreature != null)
		{
			Vector3 targetPos = targetCreature.GetCurrentViewPosition();
			
			Vector3 newPos = transform.position;
			newPos.x = targetPos.x+offset.x;
			newPos.y = targetPos.y+offset.y;
			
			transform.position = newPos;
		}
	}


	void OnSelectAction(GameObject obj)
	{
		string action = obj.GetComponent<CustomProperty> ().GetValue ("action");

		if(action == "a")
		{
			//UseSkill.InitUseSkillAction(SkillTypeList.instance.GetData(10001), selectedAgentList[0], targetCreature);

			CloseWindow();
		}
		else if(action == "b")
		{
			//UseSkill.InitUseSkillAction(SkillTypeList.instance.GetData(20001), selectedAgentList[0], targetCreature);
			
			CloseWindow();
		}
		else if(action == "c")
		{
			//UseSkill.InitUseSkillAction(SkillTypeList.instance.GetData(20001), selectedAgentList[0], targetCreature);
			
			CloseWindow();
		}
	}

    public void ShowSelectActon(AgentModel[] selectedAgentList, CreatureModel targetCreature)
	{
		this.selectedAgentList = selectedAgentList;
		this.targetCreature = targetCreature;

		gameObject.SetActive (true);

		UpdatePosition ();
	}

	public void CloseWindow()
	{
		gameObject.SetActive (false);
	}

}
