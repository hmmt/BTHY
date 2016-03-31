using UnityEngine;
using System.Collections;

public class MagicalGirl : CreatureBase {

    // 기분상태에 따라 마취약 투여가 가능해야 함.

    private bool isDark = false;
    private static string darkImage = "Unit/creature/magicalGirl_trans";

    private static int darkCondition = 25;

    private GameObject transParticle;

    public override void OnInit()
    {
        base.OnInit();
		//model.SetCurrentNode (MapGraph.instance.GetNodeById("malkuth-0-5"));
    }
	/*
    public override CreatureAttackInfo GetAttackInfo(UseSkill skill)
    {
        if (model.feeling <= darkCondition)
        {
            if (skill.skillTypeInfo.id == 0)
            {
            }
            return base.GetAttackInfo(skill);
        }
        else
        {
            return base.GetAttackInfo(skill);
        }
    }*/
    private void ChangeDark(CreatureModel creature)
    {
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(creature.instanceId);
        unit.spriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + darkImage);
        unit.spriteRenderer.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    private void ChangeNormal(CreatureModel creature)
    {
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(creature.instanceId);

        unit.spriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + creature.metaInfo.imgsrc);
        Texture2D tex = unit.spriteRenderer.sprite.texture;
        unit.spriteRenderer.gameObject.transform.localScale = new Vector3(200f / tex.width, 200f / tex.height, 1);
    }

    // 역변
    public override void OnFixedUpdate(CreatureModel creature)
    {
		/*
        if (creature.feeling <= darkCondition)
        {
            if (isDark == false)
            {
                Debug.Log("MagicalGirl.. darkness");
                isDark = true;
                ChangeDark(creature);
            }
        }
        else
        {
            if (isDark == true)
            {
                Debug.Log("MagicalGirl.. status ok");
                isDark = false;
                ChangeNormal(creature);
            }
        }

        if (creature.feeling <= 0)
        {
            creature.Escape();
        }*/
    }

    private void SkillDarkAttack(UseSkill skill)
    {
        Debug.Log("MagicalGirl dark attack");
		skill.agent.TakePhysicalDamage(4, DamageType.CUSTOM);
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        

        if (isDark)
        {
            SkillDarkAttack(skill);
        }
		/*
		skill.PauseWorking();
        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/magicalGirl/magicalGirl_enterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 6);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/magicalGirl/magicalGirl_enterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 5)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/magicalGirl/magicalGirl_enterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 4)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/magicalGirl/magicalGirl_enterTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/magicalGirl/magicalGirl_enterTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 4, 2)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        */
    }
}
