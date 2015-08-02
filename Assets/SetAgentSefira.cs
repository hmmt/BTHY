using UnityEngine;
using System.Collections;

public class SetAgentSefira : MonoBehaviour {

    public bool slotOn;
    public UnityEngine.UI.Text agentLevel;
    public UnityEngine.UI.Text agentName;
    public UnityEngine.UI.Image agentImage;
    public UnityEngine.UI.Button cancelButton;

    public void showSlot(AgentModel slotAgent)
    {
        agentImage.sprite = Resources.Load<Sprite>("Sprites/"+slotAgent.imgsrc);
        agentLevel.text = ""+slotAgent.level;
        agentName.text = slotAgent.name;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
