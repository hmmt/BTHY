using UnityEngine;
using System.Collections;

public class LadyLookingAtWall : CreatureBase {

    private const int skillPhysicsDmg = 5;
    private const int skillMentalDmg = 50;

    public override void OnSkillTickUpdate(UseSkill skill)
    {
        float prob = 0.3f;

        // 건망증이 심함, 호기심이 강함
        if (skill.agent.HasTrait(10019) || skill.agent.HasTrait(10015))
        {
            prob = 0.9f;
        }
        if (Random.value <= prob)
        {
            ActivateSkill(skill);
        }
    }

    private void ActivateSkill(UseSkill skill)
    {
        Debug.Log("LadyLookingAtWall ActivateSkill()");
        // 스킬: 뒤를 돌아보지 마
		skill.agent.TakePhysicalDamage(skillPhysicsDmg, DamageType.CUSTOM);
        skill.agent.TakeMentalDamage(skillMentalDmg);
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        //skill.PauseWorking();
        //SoundEffectPlayer.PlayOnce("match_strike_1.wav", skill.targetCreature.transform.position);
    }
}
