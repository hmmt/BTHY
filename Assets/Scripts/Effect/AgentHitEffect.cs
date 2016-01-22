using UnityEngine;
using System.Collections;

public class AgentHitEffect : MonoBehaviour
{

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

        color.a = MathUtil.UnitStep(0.2f - elapsed) * (elapsed)
            + MathUtil.UnitStep(elapsed - 0.2f) * MathUtil.UnitStep(goalTime - 0.2f - elapsed)
                + MathUtil.UnitStep(elapsed - (goalTime - 0.2f)) * (goalTime - elapsed);

        GetComponent<SpriteRenderer>().color = color;

        if (elapsed > goalTime)
        {
            Debug.Log("destroy");
            Destroy(gameObject);
        }
    }
    /*

    public static OutsideTextEffect Create(IsolateRoom room, string typoKey, CreatureOutsideTextLayout layout)
    {
        return Create(room, typoKey, layout, 0, 1);
    }
    
    public static OutsideTextEffect Create(IsolateRoom room, string typoKey, CreatureOutsideTextLayout layout, float start, float time)
    {
        return Create(room, typoKey, layout, start, time, null);
    }
    public static OutsideTextEffect Create(IsolateRoom room, string typoKey, CreatureOutsideTextLayout layout, float start, float time, Callback callback)
    {
        GameObject newEffect = Prefab.LoadPrefab("OutsideTextEffect");

        newEffect.transform.SetParent(room.transform);

        //if(layout == CreatureOutsideTextLayout.CENTER_BOTTOM)
        {
            newEffect.transform.localPosition = new Vector3(0, -4, 0);
        }

        OutsideTextEffect effect = newEffect.GetComponent<OutsideTextEffect>();

        Texture2D tex = Resources.Load<Texture2D>("Sprites/" + typoKey);
        effect.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        effect.startTime = start;
        effect.goalTime = time;

        effect.UpdateState();

        return effect;
    }
    */
    //public static AgentHitEffect Create(AgentUnit agent)
    public static AgentHitEffect Create(AgentModel agent)
    {
        // TODO : 바꿔야 
        /*
        //
        GameObject newEffect = Prefab.LoadPrefab("AgentHitEffect");

        newEffect.transform.position = agent.transform.position + new Vector3(0, 0.7f, 0);
        //newEffect.transform.SetParent(agent.transform, false);

        AgentHitEffect effect = newEffect.GetComponent<AgentHitEffect>();

        //Texture2D tex = Resources.Load<Texture2D>("Sprites/effect/agent/agent_attacked");
        Texture2D tex = Resources.Load<Texture2D>("Sprites/effect/agent/agent_attacked");
        effect.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        effect.startTime = 0;
        effect.goalTime = 1;

        effect.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        effect.UpdateState();
        */

        return null;
    }
}