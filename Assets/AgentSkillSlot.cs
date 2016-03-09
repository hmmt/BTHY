using UnityEngine;
using System.Collections;

public class AgentSkillSlot : MonoBehaviour {
    public SkillCategory unit;
    public UnityEngine.UI.Text context;

    public void Init(SkillCategory target)
    {
        this.unit = target;
        context.text = target.name + " " + target.tier;
    }

}
