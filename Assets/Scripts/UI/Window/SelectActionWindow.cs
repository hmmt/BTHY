using UnityEngine;
using System.Collections;

public class SelectActionWindow : MonoBehaviour {

	private Vector2 offset = new Vector2 (0, 0);

	private GameObject[] selectedAgentList;
	private GameObject targetCreature;

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
			Vector3 targetPos = targetCreature.transform.position;
			
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
			UseSkill.InitUseSkillAction(SkillTypeList.instance.GetData(10001), selectedAgentList[0].GetComponent<AgentUnit>(), targetCreature.GetComponent<CreatureUnit>());

			CloseWindow();
		}
		else if(action == "b")
		{
			UseSkill.InitUseSkillAction(SkillTypeList.instance.GetData(20001), selectedAgentList[0].GetComponent<AgentUnit>(), targetCreature.GetComponent<CreatureUnit>());
			
			CloseWindow();
		}
		else if(action == "c")
		{
			UseSkill.InitUseSkillAction(SkillTypeList.instance.GetData(20001), selectedAgentList[0].GetComponent<AgentUnit>(), targetCreature.GetComponent<CreatureUnit>());
			
			CloseWindow();
		}
	}

	public void ShowSelectActon(GameObject[] selectedAgentList, GameObject targetCreature)
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
