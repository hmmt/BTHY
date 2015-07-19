using UnityEngine;
using System.Collections;

public class VoidBook : CreatureBase {

    public override void OnSkillStart(UseSkill skill)
    {
        float prob = 0.3f;

        if (skill.agent.HasTrait(10015)) // 호기심이 강함
            prob = 0.8f;

        if (Random.value < prob)
        {
            ActivateSkill(skill);
        }
    }

    private void ActivateSkill(UseSkill skill)
    {
        Debug.Log("VoidBook ActivateSkill");
        long traitId = 20003; // 알 수 없는 집착

        if (skill.agent.HasTrait(traitId) == false)
        {
            skill.agent.traitList.Add(TraitTypeList.instance.GetTraitWithId(traitId));
            skill.agent.applyTrait(TraitTypeList.instance.GetTraitWithId(traitId));
        }

        if (Random.value < 0.5f)
            skill.agent.TakeMentalDamage(50);
        else
            skill.agent.TakePhysicalDamage(3);

        // 5분 후에 사라져야 함
        // 5분 동안 작업 불가해야 함
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/voidBook/voidBook_enterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 4);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/voidBook/voidBook_enterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
    }
}
