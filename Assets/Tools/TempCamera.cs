using UnityEngine;
using System.Collections;

public class TempCamera : MonoBehaviour, IObserver {

	AgentModel agent;
	// Use this for initialization
	void Start () {
		agent = AgentManager.instance.GetAgentList () [0];
		Notice.instance.Observe ("nico", this);
	}

	public Vector3 vel = new Vector3(0,0,0);

	float startTime = 0;
	float elapsed = 0;

	bool start = false;

	public void OnSetAI()
	{
		start = true;
		if (SelectWorkAgentWindow.currentWindow != null) {
			SelectWorkAgentWindow.currentWindow.CloseWindow ();
		}
	}


	public void OnNotice(string name, params object[] param)
	{
		OnSetAI ();
	}
	
	void FixedUpdate()
	{
		Notice.instance.Send(NoticeName.FixedUpdate);



		if (start) {
			/*
			CreatureModel found = null;
			foreach (CreatureModel c in CreatureManager.instance.GetCreatureList()) {
				if (c.metaInfo.id == 100004) {
					found = c;
					break;
				}
			}

			AgentManager.instance.GetAgentList()[0].ManageCreature(found, SkillTypeList.instance.GetData(10001));
*/
			elapsed += Time.deltaTime;


			if (elapsed < 13f) {
				Vector3 cPos = new Vector3 (transform.position.x, transform.position.y, 0);
				cPos = Vector3.SmoothDamp (cPos, agent.GetCurrentViewPosition (), 
					ref vel, 0.3f);

				transform.position = new Vector3 (cPos.x, cPos.y, transform.position.z);
			} else {
				float elapsed2 = elapsed - 13f;

				transform.position = new Vector3 (0, 0, transform.position.z);

				float size = Camera.main.orthographicSize;
				Camera.main.orthographicSize = size + Time.deltaTime * 2;
			}
		}

	}

}
