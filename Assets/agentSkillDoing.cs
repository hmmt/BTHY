using UnityEngine;
using System.Collections;

public class agentSkillDoing : MonoBehaviour {

    /*
    public SpriteRenderer mentalZeroRenderer;
    public SpriteRenderer mentalFullRenderer;

    public void SetMentalRate(float rate)
    {
        Color color = mentalFullRenderer.color;
        color.a = rate;
        mentalFullRenderer.color = color;
    }
    */

    public SpriteRenderer skillIcon;

    public void showDoingSkillIcon(SkillTypeInfo nowSkill, AgentUnit nowAgent)
    {
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
            skillIcon.sprite = Resources.Load<Sprite>("Sprites/UI/skill/Work_Feed_back");
        }

        else
        {
            Debug.Log("agentSkillDoing에서 에러다 시벌");
        }
    }

    public void turnOnDoingSkillIcon(bool turnOn)
    {
        skillIcon.gameObject.SetActive(turnOn);
    }
}
