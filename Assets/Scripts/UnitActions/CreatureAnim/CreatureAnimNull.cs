using UnityEngine;
using System.Collections;

public class CreatureAnimNull : CreatureAnimBase {

    public override void Init()
    {
        /*
        RuntimeAnimatorController ctrl = Resources.Load<RuntimeAnimatorController>("NullAnimator");

        Animator anim = unit.creatureAnimator;
        anim.runtimeAnimatorController = ctrl;
        //anim.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.5f);
        */
    }
    public override void LateUpdate()
    {
        /*
        //unit.creatureAnimator.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        unit.SetScaleFactor(0.5f, 0.5f, 0.5f);
        if (unit.model.state == CreatureState.ESCAPE_ATTACK)
        {
            if (unit.creatureAnimator.GetInteger("AnimCon") == 2)
                unit.creatureAnimator.transform.localPosition = new Vector3(-0.24f, 0.12f, 0);
            unit.creatureAnimator.SetInteger("AnimCon", 2);
        }
        else if (unit.model.GetMovableNode().IsMoving())
        {
            if(unit.creatureAnimator.GetInteger("AnimCon") == 1)
                unit.creatureAnimator.transform.localPosition = new Vector3(-0.24f, 0.12f, 0);
            unit.creatureAnimator.SetInteger("AnimCon", 1);
        }
        else
        {
            if (unit.creatureAnimator.GetInteger("AnimCon") == 0)
                unit.creatureAnimator.transform.localPosition = new Vector3(0, 0.3f, 0);
            unit.creatureAnimator.SetInteger("AnimCon", 0);
        }
        */
        
    }
}
