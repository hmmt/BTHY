using UnityEngine;
using System.Collections;

public class MatchGirl  : CreatureBase {

	public override void OnInit()
	{
		base.OnInit();
		//model.SetCurrentNode (MapGraph.instance.GetNodeById("malkuth-1-4"));
	}

	public override void OnFixedUpdate (CreatureModel creature)
	{
		base.OnFixedUpdate (creature);
		/*
		if (model.energyPoint < 80)
		{
			model.Escape ();
		}
		*/
	}

	public override void OnReturn ()
	{
		model.energyPoint = 130;
	}

    public override void OnSkillFailWorkTick(UseSkill skill)
	{
		if(Random.value <= 0.4)
		{
            ActivateSkill(skill);
		}
	}

	public void ActivateSkill(UseSkill skill)
	{
        Debug.Log("MatchGirl ActivateSkill");
        // show effect

		/*
        skill.PauseWorking();
        SoundEffectPlayer.PlayOnce("creature/match_girl/matchgirl_ability", skill.targetCreatureView.transform.position);

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_out_typo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 6);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_out_typo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 5)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_out_typo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 4)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_out_typo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        

        skill.targetCreature.ShowNarrationText("special_ability1", skill.agent.name);
        TimerCallback.Create(3.0f, delegate() { skill.targetCreature.ShowNarrationText("special_ability2", skill.agent.name); });
        TimerCallback.Create(6.0f,
            delegate() {
                skill.targetCreature.ShowNarrationText("special_ability3", skill.agent.name);
                float[] randomTable = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f };

                float damage = randomTable[Random.Range(0, randomTable.Length)];

                skill.agent.TakePhysicalDamage(skill.agent.hp - (int)(skill.agent.hp * (1 - damage)));
                skill.agent.TakeMentalDamage(skill.agent.mental - (int)(skill.agent.mental * (1 - damage)));

                SoundEffectPlayer.PlayOnce("creature/match_girl/matchgirl_ability_damage", skill.targetCreatureView.transform.position);

                string[] typos = {"typo/matchgirl/01_matchGirl_specialAttack_00",
                    "typo/matchgirl/01_matchGirl_specialAttack_01",
                    "typo/matchgirl/01_matchGirl_specialAttack_02"};


                TypoEffect.Create(skill.targetCreatureView, typos[Random.Range(0, typos.Length)], 0, 2);

                skill.CheckLive();
            });
       */
	}

    public override void OnSkillNormalAttack(UseSkill skill)
    {
        /*
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_commonAttack_00", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 7)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
         * */

        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        unit.animTarget.SendMessage("Attack");

        string[] typos = {"typo/matchgirl/01_matchGirl_commonAttack_00",
                    "typo/matchgirl/01_matchGirl_commonAttack_01",
                    "typo/matchgirl/01_matchGirl_commonAttack_02"};

		skill.agent.TakePhysicalDamage(1, DamageType.CUSTOM);
        skill.CheckLive();
        if (skill.agent.isDead())
        {
            AgentUnit agentUnit = AgentLayer.currentLayer.GetAgent(skill.agent.instanceId);
            //agentUnit.animTarget.PlayMatchGirlDead();
        }


        TypoEffect.Create(skill.targetCreatureView, typos[Random.Range(0, typos.Length)], 0, 2);
    }

	public override void OnEnterRoom(UseSkill skill)
	{
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        //unit.animTarget.SendMessage("Attack");
		/*
		skill.PauseWorking ();
		//SoundEffectPlayer.PlayOnce("match_strike_1.wav", skill.targetCreature.transform.position);


		OutsideTextEffect effect = OutsideTextEffect.Create (skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 8);
		effect.transform.localScale = new Vector3(1.1f,1.1f,1);

		// skill이 이미 release 될 상황 고려 필요
		effect.GetComponent<DestroyHandler> ().AddReceiver (delegate() {skill.ResumeWorking();});


		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 7)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 6)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 5)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 4, 4)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 5, 3)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 6, 2)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		*/
        skill.CheckLive();
        if (skill.agent.isDead())
        {
            AgentUnit agentUnit = AgentLayer.currentLayer.GetAgent(skill.agent.instanceId);
            //agentUnit.animTarget.PlayMatchGirlDead();
        }
	}
}
