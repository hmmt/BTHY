using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BigBirdAnim : CreatureAnimScript
{
    void Move() {
        animator.SetBool("Move", true);
    }

    void Stop() {
        animator.SetBool("Move", false);
    }

    void Kill() {
        animator.SetInteger("Kill", 1);
    }
}
