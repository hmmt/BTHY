using UnityEngine;
using System.Collections;

public class AgentExtendedScript : MonoBehaviour {
    public AgentAttributes attr;
    public AgentModel model;
    public InfoSlotScript info;

    public void SetValue(AgentSlotScript script) {
        this.model = script.model;
        attr.Name.text = model.name;

        attr.hair.sprite = ResourceCache.instance.GetSprite(model.hairImgSrc);
        attr.face.sprite = ResourceCache.instance.GetSprite(model.faceImgSrc);
        attr.body.sprite = ResourceCache.instance.GetSprite(model.bodyImgSrc);
        info.SelectedAgent(script);
    }


	
}
