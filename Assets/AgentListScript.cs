using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AgentListScript : MonoBehaviour {
    public RectTransform scrollTarget;
    public RectTransform displayArea;
    public RectTransform Addbutton;

    private List<RectTransform> child;

    private float scrollSize;
    private float initial_y;
    private float scroll_ypos;
    private float display_height;

    public void initList() {
        scrollSize = scrollTarget.rect.height;
        initial_y = scrollSize / 2;
        scroll_ypos = scrollTarget.localPosition.y;
        display_height = displayArea.rect.height;
    }

    public void makeNewSlot(AgentModel model) {
        GameObject slot = Prefab.LoadPrefab("Slot/SlotPanel");
        slot.SetActive(true);

        

        slot.transform.SetParent(scrollTarget, false);
        child.Add(slot.GetComponent<RectTransform>());
        
    }



    /*
    public void SetSetting(AgentModel model, GameObject slot) {
        //내부 속성 셋팅
        AgentSlotScript slotPanel = slot.GetComponent<AgentSlotScript>();
        AgentModel copied = model;

        slotPanel.attr1.Add.gameObject.SetActive(false);

        if (copied.currentSefira != StageUI.instance.currentSefriaUi)
            slotPanel.attr1.Add.gameObject.SetActive(true);

        slotPanel.attr1.Add.onClick.AddListener(() => StageUI.instance.SetAgentSefriaButton(copied));
        slotPanel.display = new AgentGetAttributes();
        slotPanel.display.name = model.name;
        slotPanel.display.hp = "HP : " + model.hp + "/" + model.maxHp;
        slotPanel.display.level = "" + model.level;

        switch (copied.currentSefira)
        {
            case "0":
                slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/None_Icon");
                break;
            case "1":
                slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Malkuth_Icon");
                break;
            case "2":
                slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Netzzach_Icon");
                break;
            case "3":
                slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Hod_Icon");
                break;
            case "4":
                slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Yessod_Icon");
                break;
            default:
                slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/None_Icon");
                break;
        }

        slotPanel.display.hair = ResourceCache.instance.GetSprite(unit.hairImgSrc);
        slotPanel.display.face = ResourceCache.instance.GetSprite(unit.faceImgSrc);
        slotPanel.display.body = ResourceCache.instance.GetSprite(unit.bodyImgSrc);

        slotPanel.model = model;
        slotPanel.DisplayItems();
        slotPanel.ShowPromotionButton(copied);
    }
    */
    


}
