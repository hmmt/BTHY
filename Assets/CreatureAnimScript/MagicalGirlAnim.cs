using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MagicalGirlAnim : CreatureAnimScript, IAnimatorEventCalled{
    public GameObject Normal;
    public GameObject Snake;

    public Animator NormalAnim;
    public Animator SnakeAnim;

    public MagicalLaserScript laser;
    public MagicalGirl script;

    public void SetScript(MagicalGirl scr) {
        script = scr;
    }

    public void ChangeMagicalState(bool isDark) {
        if (isDark)
        {
            animator = SnakeAnim;
            Normal.gameObject.SetActive(false);
            Snake.gameObject.SetActive(true);

        }
        else {
            animator = NormalAnim;
            NormalAnim.gameObject.SetActive(true);
            Snake.gameObject.SetActive(false);
        }
    }

    void Move()
    {
        animator.SetBool("Move", true);
    }

    void Stop()
    {
        animator.SetBool("Move", false);
    }

    void Kill()
    {
        //animator.SetInteger("Kill", 1);
    }

    public void OnCalled()
    {
        script.Attack();
    }

    public void OnCalled(int i)
    {
        if (i == 0) {
            script.Escape();
        }
    }

    public void AgentReset()
    {
        throw new NotImplementedException();
    }

    public void AnimatorEventInit()
    {
        //throw new NotImplementedException();
        
    }

    public void CreatureAnimCall(int i, CreatureBase script)
    {
        throw new NotImplementedException();
    }

    public void TakeDamageAnim(int isPhysical)
    {
        throw new NotImplementedException();
    }

    public void AttackCalled(int i)
    {
        throw new NotImplementedException();
    }

    public void SoundMake(string src)
    {
        throw new NotImplementedException();
    }
}
