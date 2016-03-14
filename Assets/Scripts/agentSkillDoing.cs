using UnityEngine;
using System.Collections;

public class agentSkillDoing : MonoBehaviour {

    public SpriteRenderer skillIcon;
        
    public void showDoingSkillIcon(SkillTypeInfo nowSkill, AgentModel nowAgent)
    {
        Debug.Log("agentSkillDoing >> need resouces");
        return;
		skillIcon.sprite = Resources.Load<Sprite>(nowSkill.imgsrc);
		/*
        if (nowSkill.type == "direct")
        {
            skillIcon.sprite = Resources.Load<Sprite>("Sprites/" + nowAgent.directSkill.imgsrc);
        }

        else if (nowSkill.type == "indirect")
        {
            skillIcon.sprite = Resources.Load<Sprite>("Sprites/" + nowAgent.indirectSkill.imgsrc);
        }

        else if (nowSkill.type == "block")
        {
            skillIcon.sprite = Resources.Load<Sprite>("Sprites/" + nowAgent.blockSkill.imgsrc);
        }

        else if (nowSkill.type == "unique")
        {
            skillIcon.sprite = Resources.Load<Sprite>("Sprites/UI/skill/Work_Unique");
        }

        else
        {
            Debug.Log("agentSkillDoing에서 에러다 시벌");
        }*/
    }

    public void turnOnDoingSkillIcon(bool turnOn)
    {
        skillIcon.gameObject.SetActive(turnOn);
    }
}
