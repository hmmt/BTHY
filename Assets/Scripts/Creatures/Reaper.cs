using UnityEngine;
using System.Collections;

public class Reaper : CreatureBase {

    public override void SkillTickUpdate(UseSkill skill)
    {
        // 혼자 투입 시

        if (false)
        {
            skill.agent.hp -= skill.agent.hp / 2;
        }
    }
}
