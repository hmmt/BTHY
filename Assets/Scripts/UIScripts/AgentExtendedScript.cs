using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AgentExtendedScript : MonoBehaviour {
    public AgentAttributes attr;
    public AgentModel model;
    public InfoSlotScript info;
    public Text characteristic;

    public void SetValue(AgentModel model) {
        this.model = model;
        attr.Name.text = model.name;
        characteristic.text = model.LifeStyle();
        attr.hair.sprite = model.tempHairSprite;
        attr.face.sprite = model.tempFaceSprite;
        //attr.body.sprite = 
        attr.Level.text = model.level + " Level";
        info.SelectedAgent(model);
    }

}
