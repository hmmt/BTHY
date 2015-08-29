using UnityEngine;
using System.Collections.Generic;

public class CreatureLayer : MonoBehaviour, IObserver {


    public static CreatureLayer currentLayer { private set; get; }

    private List<CreatureUnit> creatureList;

    private Dictionary<long, CreatureUnit> creatureDic;

    void Awake()
    {
        currentLayer = this;
        creatureList = new List<CreatureUnit>();
        creatureDic = new Dictionary<long, CreatureUnit>();
    }

    void OnEnable()
    {
        Notice.instance.Observe(NoticeName.AddCreature, this);
    }

    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.AddCreature, this);
    }

    public void Init()
    {
        foreach (CreatureModel model in CreatureManager.instance.GetCreatureList())
        {
            AddCreature(model);
        }
    }

    public void AddCreature(CreatureModel model)
    {
        float st, t1, t2, t3; // for performance test

        st = Time.realtimeSinceStartup;

        GameObject newCreature = Prefab.LoadPrefab("Creature1");

        CreatureUnit unit = newCreature.GetComponent<CreatureUnit>();

        unit.transform.SetParent(transform, false);

        unit.model = model;

        if (model.metaInfo.animatorScript != null)
        {
            unit.script = (CreatureAnimBase)System.Activator.CreateInstance(System.Type.GetType(model.metaInfo.animatorScript));
            if (unit.script != null)
            {
                unit.script.SetCreatureUnit(unit);
                unit.script.Init();
            }
        }

        t1 = Time.realtimeSinceStartup;

        unit.spriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + model.metaInfo.imgsrc);
        Texture2D tex = unit.spriteRenderer.sprite.texture;
        unit.SetScaleFactor(200f / tex.width, 200f / tex.height, 1);



        t2 = Time.realtimeSinceStartup;

        GameObject creatureRoom = Prefab.LoadPrefab("IsolateRoom");
        creatureRoom.transform.SetParent(transform, false);
        IsolateRoom room = creatureRoom.GetComponent<IsolateRoom>();

        room.roomSpriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + model.metaInfo.roomsrc);
        room.targetUnit = unit;

        t3 = Time.realtimeSinceStartup;
        /*
        room.frameSpriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + model.metaInfo.framesrc);
        */
        room.Init();

        creatureRoom.transform.position = (Vector3)model.basePosition;
        room.UpdateStatus();

        unit.room = room;

        creatureList.Add(unit);
        creatureDic.Add(model.instanceId, unit);

        //Debug.Log(model.metaInfo.name);
        //Debug.Log(""+ (t1 - st) + ", " + (t2 - t1) + ", " + (t3 - t2));
    }

    public CreatureUnit GetCreature(long id)
    {
        /*
        foreach (CreatureUnit creature in creatureList)
        {
            if (creature.model.instanceId == id)
            {
                return creature;
            }
        }
        */
        CreatureUnit unit = null;
        creatureDic.TryGetValue(id, out unit);
        return unit;
    }

    public void ClearAgent()
    {
        foreach (CreatureUnit creatureUnit in creatureList)
        {
            Destroy(creatureUnit.gameObject);
        }
        creatureList.Clear();
        creatureDic.Clear();
    }

    // TODO : 기분수치 타이머를 여기에 넣는다.?

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.AddCreature)
        {
            foreach (object obj in param)
            {
                AddCreature((CreatureModel)obj);
            }
        }
    }
}
