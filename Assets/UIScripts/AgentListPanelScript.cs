using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AgentListPanelScript : MonoBehaviour {
    public AgentModel model;
    public GameObject Extend;
    public string name;
    public string level;
    public string sefira;
    public Image panelImage;
    public Image hair;
    public Image face;
    public Image body;
    public Text Name;
    public Text Level;
    public Image Sefira;
    public Image[] skill;
    //public Button[] SefiraList;
    public int index;
    public Text Mental;
    AgentList script;

    public void Start() {
         script = GameObject.FindWithTag("SefiraAgentListPanel").GetComponent<AgentList>();
    }

    public void setIndex(int i)
    {
        index = i;
    }
    
    public void ListIndex() {
        script.extended = index;
        script.ShowAgentListD();
    }

}
