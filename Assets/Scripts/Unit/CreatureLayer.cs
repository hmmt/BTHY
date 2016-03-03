using UnityEngine;
using System.Collections.Generic;

public class CreatureLayer : MonoBehaviour, IObserver {


    public static CreatureLayer currentLayer { private set; get; }

    private List<CreatureUnit> creatureList;
    private List<int> tempIntforSprite = new List<int>();
    private Dictionary<long, CreatureUnit> creatureDic;

    private string directory = "Sprites/IsolateRoom/isolate_";
    private string dark = "_dark";

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
        //GameObject newCreature = ResourceCache.instance.LoadPrefab("Creature1");
        GameObject newCreature = ResourceCache.instance.LoadPrefab("Unit/CreatureBase");

        CreatureUnit unit = newCreature.GetComponent<CreatureUnit>();

        unit.transform.SetParent(transform, false);

        unit.model = model;
        /*
        if (model.metaInfo.animatorScript != null)
        {
            unit.script = (CreatureAnimBase)System.Activator.CreateInstance(System.Type.GetType(model.metaInfo.animatorScript));
            if (unit.script != null)
            {
                unit.script.SetCreatureUnit(unit);
                unit.script.Init();
            }
        }
        */

        if (model.metaInfo.animSrc != "")
        {
            GameObject animatorObject = Prefab.LoadPrefab(model.metaInfo.animSrc);
            unit.animTarget = animatorObject.GetComponent<CreatureAnimScript>();
            animatorObject.transform.SetParent(unit.transform, false);
        }

        // 나중에 animator들로 다 교체
        //if (model.metaInfo.imgsrc != "")
        {
            unit.spriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + model.metaInfo.imgsrc);
            Texture2D tex = unit.spriteRenderer.sprite.texture;
            unit.SetScaleFactor(200f / tex.width, 200f / tex.height, 1);
        }

        if (model.metaInfo.roomReturnSrc != "")
        {
            unit.returnSpriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + model.metaInfo.roomReturnSrc);
        }

        GameObject creatureRoom = Prefab.LoadPrefab("IsolateRoom");
        creatureRoom.transform.SetParent(transform, false);
        IsolateRoom room = creatureRoom.GetComponent<IsolateRoom>();
        int rand = Random.Range(1, 4);
        tempIntforSprite.Add(rand);
        string spriteDirectory = this.directory + rand;
        room.roomSpriteRenderer.sprite = ResourceCache.instance.GetSprite(spriteDirectory);

		GameObject g = Instantiate (room.roomSpriteRenderer.gameObject);
		g.transform.SetParent (room.transform, false);
		g.transform.localScale = new Vector3(0.16f, 0.16f, 1f);
		g.transform.localPosition = new Vector3 (0, 0, -0.006f);

        room.targetUnit = unit;

        /*
        room.frameSpriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + model.metaInfo.framesrc);
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

    public void OnSpriteButtonClick(bool state) {
        string spriteDirectory = "";
        
        if (state)
        {
            for (int i = 0; i < creatureList.Count; i++) {
                CreatureUnit cu = creatureList[i];
                spriteDirectory = this.directory + tempIntforSprite[i];
                //cu.spriteRenderer.sprite = ResourceCache.instance.GetSprite(spriteDirectory);
                cu.room.roomSpriteRenderer.sprite = ResourceCache.instance.GetSprite(spriteDirectory);
                cu.room.roomSpriteRenderer.transform.localScale = new Vector3(0.16f, 0.16f, 1f);
                cu.room.roomSpriteRenderer.transform.localPosition = Vector3.zero;
            }
        }
        else {
            for (int i = 0; i < creatureList.Count; i++)
            {
                CreatureUnit cu = creatureList[i];
                spriteDirectory = this.directory + tempIntforSprite[i] + this.dark;
                //cu.spriteRenderer.sprite = ResourceCache.instance.GetSprite(spriteDirectory);
                cu.room.roomSpriteRenderer.sprite = ResourceCache.instance.GetSprite(spriteDirectory);
                cu.room.roomSpriteRenderer.transform.localScale = new Vector3(0.16f, 0.16f, 1f);
                cu.room.roomSpriteRenderer.transform.localPosition = Vector3.zero;
            }
        }
    }
}
