using UnityEngine;
using System.Collections;

public class TypoEffect : MonoBehaviour {
    private float elapsedTime = 0;

    private float startTime = 0;
    private float goalTime = 5;

    // Use this for initialization
    void Start()
    {
        elapsedTime = 0;
    }

    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;

        UpdateState();
    }

    private void UpdateState()
    {
        Color color = GetComponent<SpriteRenderer>().color;

        float elapsed = elapsedTime - startTime;
        elapsed = elapsed > 0 ? elapsed : 0;

        color.a = MathUtil.UnitStep(0.5f - elapsed) * (elapsed/0.5f) 
                + MathUtil.UnitStep(elapsed - 0.5f) * MathUtil.UnitStep(goalTime - 0.5f - elapsed)
                + MathUtil.UnitStep(elapsed - (goalTime - 0.5f)) * (goalTime - elapsed)/0.5f;

        GetComponent<SpriteRenderer>().color = color;

        if (elapsed > goalTime)
        {
            Destroy(gameObject);
        }
    }
    public static TypoEffect Create(CreatureUnit creature, string typoKey, float start, float time)
    {
        //
        GameObject newEffect = Prefab.LoadPrefab("TypoEffect");

        newEffect.transform.position = creature.transform.position + new Vector3(0, 0.9f, 0);
        //newEffect.transform.SetParent(agent.transform, false);

        TypoEffect effect = newEffect.GetComponent<TypoEffect>();

        Texture2D tex = Resources.Load<Texture2D>("Sprites/" + typoKey);
        effect.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        effect.startTime = start;
		effect.goalTime = time;

        effect.transform.localScale = new Vector3(0.4f, 0.4f, 1);
        effect.UpdateState();

        return effect;
    }
}
