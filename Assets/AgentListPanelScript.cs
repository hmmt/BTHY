using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AgentListPanelScript : MonoBehaviour {
    public AgentModel model;
    public GameObject Extend;
    public string name;
    public string level;
    public string sefira;

    public Image hair;
    public Image face;
    public Image body;
    public Text Name;
    public Text Level;
    public Image Sefira;
    public Image[] skill;

    public bool state;

    public void Start() {
        Extend.SetActive(false);
    }

    public void Change(bool flag) {
        if (flag)
        {
            state = true;
            Extend.SetActive(true);
            DisplaySprites();
        }
        else {
            state = false;
            Extend.SetActive(false);
        }
        
    }

    public void DisplaySprites() {
        hair.sprite = ResourceCache.instance.GetSprite(model.hairImgSrc);
        body.sprite = ResourceCache.instance.GetSprite(model.bodyImgSrc);
        face.sprite = ResourceCache.instance.GetSprite(model.faceImgSrc);

        //sefira image setting;

    }

    public float getSize() {
        if (state)
        {
            return 0.3f;
        }
        else
            return 0.15f;
    }

}
