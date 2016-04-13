using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WorkInventory : MonoBehaviour {
    public GameObject work;
    public Transform parent;
    private int workCnt = 3;//기분 수치 구간의 갯수
    private List<WorkSlot> list;
    public WorkSlot selected;
    private static float sizey;
    private static float sizex;
    private bool extended = false;
    private float unitSize;
    public float minimalSize;
    public Sprite nonSelected;
    public Sprite isSelected;
    private int previous = -1;
    public GameObject WorkList;

	public CreatureModel targetCreature;

    public void Init() {
        Debug.Log("initializing");
        sizey = parent.GetComponent<RectTransform>().rect.height;
        sizex = parent.GetComponent<RectTransform>().rect.width;
        list = new List<WorkSlot>();

		workCnt = AgentManager.instance.GetAgentList ().Length;

        //init 일단 여기서 불러놓음
        //Init();
        //delete;
        extended = false;
        unitSize = sizey/workCnt;
        
        foreach (WorkSlot o in list)
        {
            Debug.Log("InitDestroy" + o.NormalState.childCount);
            Destroy(o.gameObject);
        }

        list.Clear();

        for (int i = 0; i < workCnt; i++) {
            CreatePanel(i);

        }

        WorkList.gameObject.SetActive(false);
    }

    public void WindowDestroy() {
        foreach (WorkSlot w in list) {
            w.CloseWindow();
        }
    }

	private void CreatePanel(int index) {
        GameObject newObj = Instantiate(work);
        RectTransform rect = newObj.GetComponent<RectTransform>();
        WorkSlot script = newObj.GetComponent<WorkSlot>();
        //Debug.Log("createPanel"+ script.NormalState.childCount);

		WorkSettingElement setting = AutoCommandManager.instance.GetWorkSetting (targetCreature);

        //script.ClearCurrentSkill();
		script.Init(setting, index);

        rect.SetParent(parent);
        newObj.transform.localScale = Vector3.one;
        //newObj.GetComponent<Image>().sprite = nonSelected;
        
        float height = unitSize;

        script.SetInventoryScript(this);

        rect.localPosition = Vector3.zero;
        SetPos(rect, index);

        AddEventTrigger(script);
		script.agentTargetForProto = AgentManager.instance.GetAgentList () [index];
        list.Add(script);
    }

    public void AddEventTrigger(WorkSlot script) {
        GameObject target = script.GetLeftImage();

        EventTrigger trigger1 = target.AddComponent<EventTrigger>();
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerClick;
        entry1.callback.AddListener((eventdata) => { this.OnClick(script.index); });
        trigger1.triggers.Add(entry1);

    }

    public void OnClick(int i) {
        selected = list[i];
        //선택 효과 만들기
        
        if (previous != -1) {
            list[previous].SelectImage.SetActive(false);
            
        }
        previous = i;
        selected.SelectImage.SetActive(true);
        
        if (!extended)
        {
            WorkList.gameObject.SetActive(true);
            extended = true;
        }
        this.GetComponent<WorkListScript>().Select(selected);
    }

    public void Close() {
        extended = false;
        selected = null;
        list[previous].SelectImage.SetActive(false);
        previous = -1;
        WorkList.gameObject.SetActive(false);
    }

    public void Extend(int target) {

        
        if (extended) return;
        extended = true;
        float size = sizey - (minimalSize * (workCnt-1));
        float posy = sizey / 2;
        for (int i = 0; i < list.Count; i++) {
            RectTransform rect = list[i].GetComponent<RectTransform>();
            float pos;
            if (i.Equals(target))
            {
               
                rect.sizeDelta = new Vector2(sizex, size);
                pos = posy - size / 2;

            }
            else {
                rect.sizeDelta = new Vector2(sizex, minimalSize);
                pos = posy - minimalSize / 2;
            }
            rect.anchoredPosition = new Vector2(0.0f, pos);
            posy -= rect.sizeDelta.y;
        }
    }

    public void Reduce()
    {
        extended = false;
        for (int i = 0; i < list.Count; i++) {
            RectTransform rect = list[i].GetComponent<RectTransform>();
            SetPos(rect, i);
        }
    }

    private void SetPos(RectTransform rect, int index) {
        rect.sizeDelta = new Vector2(sizex, unitSize);
        rect.anchoredPosition = new Vector2(0.0f, (sizey - unitSize* (1+2*index))/2);
        // sizey/2 - unitsize/2 - unitsize*index
    }

}
