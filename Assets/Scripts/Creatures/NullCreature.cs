using UnityEngine;
using System.Collections;

public class NullCreature : CreatureBase {

    private static int changeCondition = 25;
    private static string transImage = "Unit/creature/NullThing";

    private bool changed = false;


    private void ChangeBody()
    {
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        Texture2D tex = Resources.Load<Texture2D>("Sprites/" + transImage);
        unit.spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        unit.spriteRenderer.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }
    private void ChangeNormal()
    {
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        Texture2D tex = Resources.Load<Texture2D>("Sprites/" + model.metaInfo.imgsrc);
        unit.spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
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
}
