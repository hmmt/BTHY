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
    public Button[] SefiraList;
    public int index;
    public bool state;
    
    public void Change(bool flag) {
        if (flag)
        {
            state = true;
            Extend.SetActive(true);
        }
        else {
            state = false;
            Extend.SetActive(false);
        }
        
    }

    public void InitSefia() { 
        string[] list = {"1","2","3","4","5","6","7","8","9","10"};
        bool[] isopend = new bool[10];
        for (int i = 0; i < 10; i++) {
            isopend[i] = PlayerModel.instance.IsOpenedArea(list[i]);
            SefiraList[i].gameObject.SetActive(isopend[i]);
        }
        
    }

    public void setIndex(int i)
    {
        index = i;
    }

    public float getSize() {
        if (state)
        {
            return 0.3f;
        }
        else
            return 0.15f;
    }

    public void ListIndex() {
        AgentList script = GameObject.FindWithTag("SefiraAgentListPanel").GetComponent<AgentList>();
        script.extended = index;        
        script.ShowAgentListD();
    }
    

}
