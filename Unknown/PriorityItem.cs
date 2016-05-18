using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PriorityItem : MonoBehaviour {
    /*
    public CreatureModel model;
    public CreatureModel targetModel;
    public int index;
    public Image img;
    public int priorityLevel;

    private bool _info = false;
    private CreaturePriority cp;
    private Sefira.CreaturePriority prioritySystem;
    private Sefira.CreaturePriority.Priority priority = null;
    
    public void Start()
    {
    }

    public void Init(Sefira.CreaturePriority p, CreatureModel mo, CreaturePriority cp) {
        prioritySystem = p;
        targetModel = mo;
        this.cp = cp;
    }

    public void SetInfo(CreatureModel model) {
        _info = true;
        this.model = model;
        if (model == null) return;
        priority = prioritySystem.GetPriorityByModel(model);
        Debug.Log(model + " " + index);
        img.sprite = ResourceCache.instance.GetSprite("Sprites/" + model.metaInfo.imgsrc);
    }

    public void Clear() {
        if (priority == null) return;
        prioritySystem.PriorityClear(priority);
        img.sprite = ResourceCache.instance.GetSprite("Sprites/" + "Logo");
        _info = false;
    }

    public void OnClick(BaseEventData eventData) {
        PointerEventData pointer = eventData as PointerEventData;

        if (pointer.button.Equals(PointerEventData.InputButton.Right)) {
            Clear();
        }
        else if (pointer.button.Equals(PointerEventData.InputButton.Left))
        {
            PriorityItem temp = null;
            if (( temp = cp.GetScript(targetModel)) != null) {
                temp.Clear();
            }

            prioritySystem.SetPriority(targetModel, index + 1);
            SetInfo(targetModel);
        }
    }
    */
    public CreatureModel model;
    public int index;
    public Image Portrait;

    private static Sprite nullImage;
    private CreaturePriority cp;//manageSystem

    public void Awake() {
        nullImage = ResourceCache.instance.GetSprite("Sprites/Logo");
    }

    public void Init(int index, CreaturePriority cp) {
        this.index = index;
        //this.model = model;
        this.cp = cp;
    }

    public void SetModel(CreatureModel model) {
        this.model = model;
    }

    public void SetInfo() {
        if (model == null)
            return;
        Portrait.sprite = ResourceCache.instance.GetSprite("Sprites/" + model.metaInfo.imgsrc);
    }

    public void Clear() {
        Portrait.sprite = nullImage;
        this.model = null;
    }
    
    public void OnClick(BaseEventData eventData) {
        PointerEventData pointer = eventData as PointerEventData;
        if (pointer.button.Equals(PointerEventData.InputButton.Right))
        {
            cp.RemovePriority(this.index);
        }
        else if (pointer.button.Equals(PointerEventData.InputButton.Left))
        {
            cp.SetPriority(this.index);
        }
    }
}
