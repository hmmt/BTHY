using UnityEngine;
using System.Collections;

public class VoidBook : CreatureBase {

    public override void EnterRoom(UseSkill skill)
    {
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.room, "typo/voidBook/voidBook_enterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 4);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.room, "typo/voidBook/voidBook_enterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
    }
}
