using UnityEngine;
using System.Collections;

public class NullCreature : CreatureBase {

    private static int changeCondition = 25;
    private static string transImage = "Unit/creature/NullThing";

    private bool changed = false;


    private void ChangeBody()
    {
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        unit.spriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + transImage);
        unit.spriteRenderer.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }
    private void ChangeNormal()
    {
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        unit.spriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + model.metaInfo.imgsrc);
        Texture2D tex = unit.spriteRenderer.sprite.texture;
        unit.spriteRenderer.gameObject.transform.localScale = new Vector3(200f / tex.width, 200f / tex.height, 1);
    }

    // 변신
    public override void OnFixedUpdate(CreatureModel creature)
    {
        if (creature.feeling <= changeCondition)
        {
            if (!changed)
            {
                ChangeBody();
                changed = true;
            }
        }
        else
        {
            if (changed)
            {
                ChangeNormal();
                changed = false;
            }
        }

        if (creature.feeling <= 0)
        {
            creature.Escape();
        }
    }

    public override SkillTypeInfo GetSpecialSkill()
    {
        if (changed)
        {
            // 임시
            return SkillTypeList.instance.GetData(400010);
        }
        else
        {
            return null;
        }
    }
}
