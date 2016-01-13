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
        attr.hair.sprite = ResourceCache.instance.GetSprite(model.hairImgSrc);
        attr.face.sprite = ResourceCache.instance.GetSprite(model.faceImgSrc);
        attr.body.sprite = ResourceCache.instance.GetSprite(model.bodyImgSrc);
        info.SelectedAgent(model);
    }

}
