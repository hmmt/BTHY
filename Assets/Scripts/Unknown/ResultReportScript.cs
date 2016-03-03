using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResultReportScript : MonoBehaviour {
    public AgentModel worker;
    public List<AgentModel> list;

    public GameObject listObject;
    public Image Hair, Face, Body;
    public Text Name;

    public void SetBestAgent(AgentModel model) {
        this.worker = model;

        Hair.sprite = ResourceCache.instance.GetSprite(model.hairImgSrc);
        Face.sprite = ResourceCache.instance.GetSprite(model.faceImgSrc);
        Body.sprite = ResourceCache.instance.GetSprite(model.bodyImgSrc);
        Name.text = model.name;
    }    


	
}
