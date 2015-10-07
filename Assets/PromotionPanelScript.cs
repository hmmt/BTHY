using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class PromotionPanelScript : MonoBehaviour {
    public Image Hair;
    public Image Body;
    public Image Face;
    public Text name;
    public Button direct;
    public Button indirect;
    public Button block;
    public Button confirm;
    public AgentModel model;
	// Use this for initialization
	void Start () {
        model = transform.parent.GetComponent<AgentSlotScript>().model;
        Hair.sprite = ResourceCache.instance.GetSprite(model.hairImgSrc);
        Body.sprite = ResourceCache.instance.GetSprite(model.bodyImgSrc);
        Face.sprite = ResourceCache.instance.GetSprite(model.faceImgSrc);
        name.text = model.name;
        direct.image.sprite = model.WorklistSprites[0];
        indirect.image.sprite = model.WorklistSprites[1];
        block.image.sprite = model.WorklistSprites[2];
        

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
