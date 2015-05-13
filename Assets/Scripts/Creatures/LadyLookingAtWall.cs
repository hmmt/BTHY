using UnityEngine;
using System.Collections;

public class LadyLookingAtWall : CreatureBase {

    public override void SkillTickUpdate(UseSkill skill)
    {
        if (Random.value <= 0.3f)
        {
            // 스킬
        }
    }

    public override void EnterRoom(UseSkill skill)
    {
    }
}
