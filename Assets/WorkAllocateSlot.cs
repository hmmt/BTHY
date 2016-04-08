using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorkAllocateSlot : MonoBehaviour {
    public AgentModel model;

    public Image Face;
    public Image Hair;
    public Text CoolTime;

    public void Init(AgentModel model) {
        this.model = model;
        AgentModel.SetPortraitSprite(model, Face.sprite, Hair.sprite);
        //SetCoolTime;
        SetCoolTime();
    }

    public void SetCoolTime() { 
        
    }
}
