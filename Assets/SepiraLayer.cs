using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SepiraLayer : MonoBehaviour{

    public static SepiraLayer currentLayer { private set; get; }

    void Awake()
    {
        currentLayer = this;
    }

    void FixedUpdate()
    {
        if (SefiraAgentSlot.instance.MalkuthAgentList.Count == 0)
        {
            MalkuthSkillActive();

            foreach (CreatureModel unit in CreatureManager.instance.MalkuthCreature)
            {
                unit.DangerFeeling();
            }
        }

        if (SefiraAgentSlot.instance.NezzachAgentList.Count == 0 && PlayerModel.instance.IsOpenedArea("2"))
        {
            NezzachSkillActive();

            foreach (CreatureModel unit in CreatureManager.instance.NezzachCreature)
            {

                unit.DangerFeeling();

            }
        }

        if (SefiraAgentSlot.instance.HodAgentList.Count == 0 && PlayerModel.instance.IsOpenedArea("3"))
        {
            HodSkillActive();

            foreach (CreatureModel unit in CreatureManager.instance.HodCreature)
            {

                unit.DangerFeeling();

            }
        }

        if (SefiraAgentSlot.instance.YesodAgentList.Count == 0 && PlayerModel.instance.IsOpenedArea("4"))
        {

            YessodSkillActive();

            foreach (CreatureModel unit in CreatureManager.instance.YessodCreature)
            {

                unit.DangerFeeling();

            }
        }

    }

    public void SepiraWarning(string sepria)
    {

    }

    public void MalkuthSkillActive()
    {

    }

    public void NezzachSkillActive()
    {

    }

    public void HodSkillActive()
    {

    }

    public void YessodSkillActive()
    {

    }


}
