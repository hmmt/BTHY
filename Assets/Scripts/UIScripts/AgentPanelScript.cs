using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AgentPanelScript : MonoBehaviour, IPointerClickHandler{
    public AgentModel model;
    
    public Image hairIcon;
    public Image faceIcon;
    public Text nameText;
    public Text HPText;
    public Button promotion;
    public Text agentLevel;
    public Button addSefira;
    public Image currentSefira;
    public GameObject infoslot;
    private bool state = false;//false = small, true = extended;

    public GameObject small;
    public GameObject extended;

    public void ChangePrefab(){
        small.SetActive(!state);
        extended.SetActive(state);

    }

    public void Start() {
        ChangePrefab();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        state = !state;
        //GameObject infoslot = GameObject.FindWithTag("InfoSlotPanel");
        //infoslot.GetComponent<InfoSlotScript>().SelectedAgent(gameObject);
    }
}
