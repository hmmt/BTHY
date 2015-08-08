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
        GameObject newCreature = Prefab.LoadPrefab("Creature1");

        CreatureUnit unit = newCreature.GetComponent<CreatureUnit>();

        unit.transform.SetParent(transform, false);

        unit.model = model;

        Texture2D tex = Resources.Load<Texture2D>("Sprites/" + model.metaInfo.imgsrc);
        unit.spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        unit.spriteRenderer.gameObject.transform.localScale = new Vector3(200f / tex.width, 200f / tex.height, 1);

        GameObject creatureRoom = Prefab.LoadPrefab("IsolateRoom");
        creatureRoom.transform.SetParent(transform, false);
        IsolateRoom room = creatureRoom.GetComponent<IsolateRoom>();
        tex = Resources.Load<Texture2D>("Sprites/" + model.metaInfo.roomsrc);

        room.roomSpriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        room.targetUnit = unit;
        /*
        tex = Resources.Load<Texture2D>("Sprites/" + model.metaInfo.framesrc);
        room.frameSpriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        */
        room.Init();

        creatureRoom.transform.position = (Vector3)model.basePosition;
        room.UpdateStatus();

        unit.room = room;

        creatureList.Add(unit);
        creatureDic.Add(model.instanceId, unit);
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
