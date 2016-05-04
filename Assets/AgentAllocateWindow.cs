using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AgentAllocateWindow : MonoBehaviour {
    public class AgentListSortModule {
        int currentMode = 0;
    /*
     *  직원들을 정렬한다
     * total = 전체 직원들 오브젝트가 들어가있는 리스트
     * mode = 정렬 모드를 선택 (0 = 기본 정렬모드: 즉, 생성 순서대로 저장됨
     *                          1 = 이름
     *                          2 = 등급
     *                          3 = 부서
     *                          4 = 가치관)
     *                          가치관의 순서는 1:합리주의
     *                                          2:낙천주의
     *                                          3:원칙주의
     *                                          4:평화주의
     */
        public void SortList(int mode, List<AgentModel> list, List<AgentAllocateSlot> slotList) {
            if (this.currentMode == mode)
            {
                InverseList(list);
            }
            else
            {
                this.currentMode = mode;
                switch (mode)
                {
                    case 0:
                        list.Sort(AgentModel.CompareByID);
                        break;
                    case 1:
                        list.Sort(AgentModel.CompareByName);
                        break;
                    case 2:
                        list.Sort(AgentModel.CompareByLevel);
                        break;
                    case 3:
                        list.Sort(AgentModel.CompareBySefira);
                        break;
                    case 4:
                        list.Sort(AgentModel.CompareByLifestyle);
                        break;
                }
            }

            Reflect(list, slotList);
        }

        public void InverseList(List<AgentModel> list) {
            AgentModel[] temp = new AgentModel[list.Count];
            list.CopyTo(temp);
            list.Clear();
            for (int i = temp.Length - 1; i >= 0; i--) {
                list.Add(temp[i]);
            }
        }

        public void Reflect(List<AgentModel> list, List<AgentAllocateSlot> slotList)
        {
            List<AgentAllocateSlot> copyList = new List<AgentAllocateSlot>(slotList.ToArray());
            slotList.Clear();

            foreach (AgentModel model in list) {
                AgentAllocateSlot script = null;
                foreach (AgentAllocateSlot slot in copyList) {
                    if (slot.id == model.instanceId) {
                        script = slot;
                        copyList.Remove(slot);
                        break;
                    }
                }
                if (script == null) {
                    Debug.LogError("Slot finding failed");
                    return;
                }
                slotList.Add(script);
            }
        }
    }

    private static AgentAllocateWindow _instance;
    public static AgentAllocateWindow instance {
        get {
            return _instance;
        }
    }

    public GameObject slotObject;

    public RectTransform agentScrollTarget;
    public RectTransform addButton;

    public float verticalSpacing;
    public List<AgentModel> agentList = new List<AgentModel>();

    List<AgentAllocateSlot> currentSlotList = new List<AgentAllocateSlot>();

    float initialPosy;
    float currentPosy;

    AgentListSortModule sort = new AgentListSortModule();

    public void Awake() {
        _instance = this;

        foreach (RectTransform rect in this.agentScrollTarget) {
            if (rect.GetComponent<AgentAllocateSlot>() != null) {
                Destroy(rect.gameObject);
            }
        }

        initialPosy = agentScrollTarget.localPosition.y;
        currentPosy = 0f;
        SetScrollRect(currentPosy);
    }

    public void ClearList() {
        foreach (AgentAllocateSlot item in currentSlotList)
        {
            Destroy(item.gameObject);
        }
        currentSlotList.Clear();
        foreach (AgentModel agentModel in agentList) { 
            //makeSlot;
            AddAgent(agentModel);
        }
        //ShowAgentList();
    }

    public void AddAgent(AgentModel model)
    {
        agentList.Add(model);//이 위치가 맞는 것일까
        GameObject newObj = Instantiate(slotObject);
        newObj.transform.SetParent(this.agentScrollTarget);
        AgentAllocateSlot script = newObj.GetComponent<AgentAllocateSlot>();
        script.SetModel(model);
        script.SetInitialTransform();
        model.calcLevel();
        script.SetPos(-currentPosy);
        script.Init();
        
        currentPosy += (script.GetHeight() + verticalSpacing);
        currentSlotList.Add(script);

        this.SetScrollRect(currentPosy);
    }

    public void ShowAgentList() {
        float posy = 0f;

        foreach (AgentAllocateSlot script in currentSlotList) {
            script.SetPos(-posy);
            posy += (script.GetHeight() + verticalSpacing);
        }

        SetScrollRect(posy);
    }

    public void SetScrollRect(float size) {
        float lastHeight = addButton.rect.height;
        addButton.anchoredPosition = new Vector2(0f, -size);
        float totalSize = size + lastHeight;

        this.agentScrollTarget.sizeDelta = new Vector2(this.agentScrollTarget.sizeDelta.x,
                                                       totalSize);
    }

    public void SrotAgentList(int mode) {
        sort.SortList(mode, this.agentList, this.currentSlotList);
        ShowAgentList();
    }

    public void SetAddButtonState(bool b) {
        this.addButton.gameObject.SetActive(b);
    }
}
