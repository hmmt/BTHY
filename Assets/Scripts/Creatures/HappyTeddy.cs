using UnityEngine;
using System.Collections;

public class HappyTeddy  : CreatureBase {

	public override void OnSkillStart(UseSkill skill)
	{
		if(skill.skillId == 40002) // 
		{
			if(skill.targetCreature.feeling <= 50)
			{
				if(Random.value <= 0.65)
				{
					//skill.agent.hp -= 1; // temp
				}
			}
		}
	}

    public override void OnSkillTickUpdate(UseSkill skill)
	{
		if(skill.skillId == 40002)
		{
            //if(skill.targetCreature.feeling <= 50)
            {
                if(Random.value <= 0.45)
                {
                    //skill.agent.hp -= 1; // temp
                    ActivateSkill(skill);
                }
            }
		}
	}

    // temporary
    public override void OnSkillFailWorkTick(UseSkill skill)
    {
        //ActivateSkill(skill);
    }

    public void ActivateSkill(UseSkill skill)
    {
        // show effect

        skill.PauseWorking();
        ///SoundEffectPlayer.PlayOnce("creature/match_girl/matchgirl_ability.wav", skill.targetCreature.transform.position);

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.room, "typo/happyteddy/teddyBear_AttackTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 4.5f);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.room, "typo/happyteddy/teddyBear_AttackTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 0.5f, 4.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/happyteddy/teddyBear_AttackTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.0f, 3.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/happyteddy/teddyBear_AttackTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.5f, 3.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/happyteddy/teddyBear_AttackTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.0f, 2.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/happyteddy/teddyBear_AttackTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.5f, 2.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);

        skill.targetCreature.ShowNarrationText("special_ability1", skill.agent.name);
		SoundEffectPlayer.PlayOnce("creature/happy_teddy/happyTeddy_Ability_Special", skill.targetCreature.transform.position);

        TimerCallback.Create(4.0f, delegate() {
            skill.agent.hp -= 1; // temporary value
        });
    }

    //

    public override void OnEnterRoom(UseSkill skill)
    {
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.room, "typo/happyteddy/happyBear_EnterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 4);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });


        OutsideTextEffect.Create(skill.targetCreature.room, "typo/happyteddy/happyBear_EnterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.room, "typo/happyteddy/happyBear_EnterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 2)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
    }
}
