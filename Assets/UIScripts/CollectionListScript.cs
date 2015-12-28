using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SefiraButtons {
    
}

public class CollectionListScript : MonoBehaviour {
    public bool isOpened = false;
    public Transform scrollTarget;
    public Transform anchor;
    public Text SefiraName;
    public Image SefiraIcon;
    public Animator slideAnim;
    public string currentSefira = "1";
    public int selected = -1;
    public bool selectSefiraEnabled = false;
    public GameObject SefiraList;
    public GameObject PanelBg;
    private int state1 = 0;
    private RectTransform startPos;
    public RectTransform sizeStd;
    public AgentList otherWindow;
    private Sprite[] sefiraImage = new Sprite[10];

    List<CreatureModel> creatureList = new List<CreatureModel>();

    public static CollectionListScript currentWindow = null;
    public int extended = -1;

    public void Start() {
        startPos = PanelBg.GetComponent<RectTransform>();

        sefiraImage[0] = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Malkuth_Icon");
        sefiraImage[1] = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Netzzach_Icon");
        sefiraImage[2] = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Hod_Icon");
        sefiraImage[3] = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Yessod_Icon");
    }

    public void ShowCollectionList()
    {
        SetCurrentList();

        foreach (Transform child in scrollTarget.transform) {
            Destroy(child.gameObject);
        }

        GameObject[] list = new GameObject[creatureList.Count];

        int cnt = 0;
        foreach (CreatureModel unit in creatureList) {
            GameObject slot = Prefab.LoadPrefab("Slot/CollectionListPanelObject");
            slot.SetActive(true);
            slot.transform.SetParent(scrollTarget, false);
            list[cnt] = slot;

            CollectionPanelScript slotPanel = slot.GetComponent<CollectionPanelScript>();
            slotPanel.index = cnt;

            if (cnt % 2 == 0)
            {
                slotPanel.PanelImage.sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Semi");
            }
            else
            {
                slotPanel.PanelImage.sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Dark");
            }
            CreatureModel copied = unit;
           
            slotPanel.SetPanel(copied);
            cnt++;
        }

        float posy = 0.0f;
        foreach (GameObject child in list) {
            //float size;
            RectTransform rt = child.GetComponent<RectTransform>();

            rt.localPosition = new Vector3(0.0f, posy, 0.0f);
            posy -= rt.rect.height;
        }

        Vector2 scrollSize = scrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollSize.y = -posy;
        scrollTarget.GetComponent<RectTransform>().sizeDelta = scrollSize;

    }

    /*
       for(int i = 0; i < creatureList.Count; i++){
           CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(creatureList[i].instanceId);
       }
        */


    public void SetCurrentList() {
        creatureList = CreatureManager.instance.GetSelectedList(currentSefira);

        switch (currentSefira)
        {
            case "1":
                SefiraName.text = "지휘감시팀";
                SefiraIcon.sprite = sefiraImage[0];
                break;
            case "2":
                SefiraName.text = "비상계획팀";
                SefiraIcon.sprite = sefiraImage[1];
                break;
            case "3":
                SefiraName.text = "자재관리팀";
                SefiraIcon.sprite = sefiraImage[2];
                break;
            case "4":
                SefiraName.text = "솔루션계획팀";
                SefiraIcon.sprite = sefiraImage[3];
                break;
        }

    }

    //called by button or etc. Setting Current Sefira and It will be called other method
    public void SetCurrentSefira(string sefira) {
        this.currentSefira = sefira;
        SetCurrentList();
    }

    public void CollectionListAnimButton() {
        if (slideAnim.GetBool("Slide"))
        {
            isOpened = false;
            slideAnim.SetBool("Slide", false);
        }
        else {
            isOpened = true;
            slideAnim.SetBool("Slide", true);
            if (otherWindow.isOpened) {
                otherWindow.isOpened = false;
                otherWindow.slideAnim.SetBool("Slide", false);
            }
            ShowCollectionList();
        }
    }

    public void SelectSefira() {
        if (selectSefiraEnabled) return;
        selectSefiraEnabled = true;
        SefiraList.gameObject.SetActive(true);

        Vector2 s_delta = new Vector2(PanelBg.GetComponent<RectTransform>().sizeDelta.x, PanelBg.GetComponent<RectTransform>().sizeDelta.y - SefiraList.GetComponent<RectTransform>().rect.height);
        PanelBg.GetComponent<RectTransform>().sizeDelta = s_delta;

        ShowCollectionList();
    }

    public void CloseSelectWindow(string sefira) {
        if (!selectSefiraEnabled) return;
        selectSefiraEnabled = false;
        SefiraList.gameObject.SetActive(false);

        Vector2 s_delta = new Vector2(PanelBg.GetComponent<RectTransform>().sizeDelta.x, PanelBg.GetComponent<RectTransform>().sizeDelta.y + SefiraList.GetComponent<RectTransform>().rect.height);
        PanelBg.GetComponent<RectTransform>().sizeDelta = s_delta;

        SetCurrentSefira(sefira);
        ShowCollectionList();
    }

    public void OverlaySefira(string sefira) {
        switch (sefira)
        {
            case "1":
                SefiraName.text = "지휘감시팀";
                SefiraIcon.sprite = sefiraImage[0];
                break;
            case "2":
                SefiraName.text = "비상계획팀";
                SefiraIcon.sprite = sefiraImage[1];
                break;
            case "3":
                SefiraName.text = "자재관리팀";
                SefiraIcon.sprite = sefiraImage[2];
                break;
            case "4":
                SefiraName.text = "솔루션계획팀";
                SefiraIcon.sprite = sefiraImage[3];
                break;
        }
    }
}
