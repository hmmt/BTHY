using UnityEngine;
using System.Collections;

public class AgentSlot : MonoBehaviour {
	public interface IReceiver
	{
		void OnClickSlot(GameObject target);
	}

	public int slotIndex = 0;
	public IReceiver receiver = null;

	void OnClick()
	{
		transform.parent.SendMessage ("OnClickSlot", gameObject, SendMessageOptions.DontRequireReceiver);
		if(receiver != null)
		{
			receiver.OnClickSlot(gameObject);
		}
	}

	public void SetSelect(bool b)
	{
		if(b)
		{
			transform.Find("Name").GetComponent<TextMesh>().color = new Color(0,0,0);
		}
		else
		{
			transform.Find("Name").GetComponent<TextMesh>().color = new Color(1,1,1);
		}
	}
}
