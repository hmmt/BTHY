using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreaturePriority : MonoBehaviour {
    /*
    public GameObject Item;
    public RectTransform parent;
    public List<PriorityItem> list;

    private int cnt;
    private Sefira.CreaturePriority priority;
    private Sefira.CreaturePriority.Priority currentPriority;
    private int max;
    private CreatureModel model;

    public void Init(CreatureModel model) {
        this.model = model;
        
        priority = SefiraManager.instance.getSefira(model.sefiraNum).priority;
        currentPriority = priority.GetPriorityByModel(model);
        max = priority.Max;
        Sefira.CreaturePriority.Priority[] tempAry = priority.GetList();

        float unit = parent.rect.width / max;
        float posx =  -parent.rect.width/2;
        for (int i = 0; i < max; i++) {
            GameObject newObj = Instantiate(Item);
            RectTransform rect = newObj.GetComponent<RectTransform>();

            newObj.GetComponent<RectTransform>().SetParent(parent);
            rect.localPosition = Vector2.zero;
            rect.localScale = Vector2.one;
            rect.sizeDelta = new Vector2(unit, unit);
            rect.anchoredPosition = new Vector2(posx + rect.rect.width / 2, 0.0f);
            posx += rect.rect.width;

            PriorityItem script = newObj.GetComponent<PriorityItem>();

            script.Init(priority,model, this);
            script.index = i;
            
            this.list.Add(script);
            //add event Trigger
        }
        int index = 0;
        foreach (Sefira.CreaturePriority.Priority p in tempAry) {
            if (p.priority == 0) {
                continue;
            }
            list[index].SetInfo(p.model);
        }
    }

    public int GetCnt()
    {
        return cnt;
    }


    public void OnClick(int i) {
        //Button Clicked
        this.cnt = i;
        if (currentPriority.priority == i + 1) {
            return;
        }

    }

    public PriorityItem GetScript(CreatureModel model) {
        PriorityItem output = null;

        foreach (PriorityItem p in list) {
            if (p.model == model) {
                output = p;
                break;
            }
        }

        return output;
    }
    */

    public GameObject Item;
    public RectTransform parent;
    public List<PriorityItem> list;

    private CreatureModel currentModel;

    private Sefira.PrioritySystem prioritySystem;
    private Sefira.PrioritySystem.Priority currentPriority;

    private int max;

    public void Init(CreatureModel model) {
        this.currentModel = model;
        prioritySystem = SefiraManager.instance.GetSefira(currentModel.sefiraNum).priority;
        max = prioritySystem.list.Count;
        
        float unitSize = parent.rect.width / max;
        float posx = -parent.rect.width / 2;
        for (int i = 0; i < max; i++) {
            GameObject newObj = Instantiate(Item);
            RectTransform rect = newObj.GetComponent<RectTransform>();
            PriorityItem script = newObj.GetComponent<PriorityItem>();
            script.Init(i, this);
            rect.SetParent(parent);
            rect.localPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(unitSize, unitSize);
            rect.anchoredPosition = new Vector2(posx + rect.rect.width / 2, 0.0f);
            posx += rect.rect.width;

            list.Add(script);
        }

        for (int i = 0; i < max; i++) {

            if (prioritySystem.list[i].priority != -1)
            {
                /*
                list[i].SetModel(prioritySystem.list[i].model);
                list[i].SetInfo();
                 */
                list[prioritySystem.list[i].priority].SetModel(prioritySystem.list[i].model);
                list[prioritySystem.list[i].priority].SetInfo();
            }
        }

    }

    public void SetPriority(int index)
    {
        /*
        Sefira.PrioritySystem.Priority older = prioritySystem.GetPriorityByLevel(index);
        if (older != null) {
            GetItemByModel(older.model).Clear();
        }*/
        

        PriorityItem older = GetItemByModel(currentModel);
        if (older != null) {
            older.Clear();
        }

        PriorityItem newItem = this.list[index];
        newItem.SetModel(currentModel);
        newItem.SetInfo();
        prioritySystem.SetPriority(currentModel, index);
    }

    public void RemovePriority(int index) {
        PriorityItem target = this.list[index];
        prioritySystem.SetPriorityNull(target.model);
        target.Clear();
    }

    public PriorityItem GetItemByModel(CreatureModel model) {
        PriorityItem output = null;
        foreach (PriorityItem temp in this.list) {
            if (model.Equals(temp.model)) {
                output = temp;
                break;
            }
        }
        return output;
    }
   

}
