using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoCharacterScript : MonoBehaviour {
    public GameObject portrait;
    public RectTransform pos;
    public Text name, grade;

    //public AgentModel model;
    //public GameObject InfoSlot;

	// Use this for initialization
	void Start () {
        portrait = null;
        name.text = null;
        grade.text = null;
	}
	
	// Update is called once per frame
	void Update () {
	        
	}

    public void setSlot(AgentSlotPanelStage target) {
        GameObject image;
        if (portrait != null)
        {
            Destroy(portrait);
            portrait = null;
        }
        name.text = target.model.name;
        grade.text = target.model.level + "등급";
        image = Instantiate(target.gameObject.GetComponent<DragScript>().moveImage);
        portrait = image;
        image.transform.SetParent(transform);
        image.transform.position = pos.transform.position;
        //portrait.transform.SetParent(transform);


    }
}
