using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class StoryMenuScript : MonoBehaviour {
    private static StoryMenuScript _instance = null;
    public static StoryMenuScript instance {
        get {
            return _instance;
        }
    }
    [System.Serializable]
    public class ManualObject {
        public GameObject prefabs;
        public string name;
        public bool exported = false;
    }

    public Menu[] menu;
    public int level;
    public GameObject[] menuLevel;
    public List<ManualObject> manualList;
    private Menu _selected;
    private GameObject _currentLevel;

    public void Awake() {
        _instance = this;
    }

    public void Start() {
        _selected = null;
        level = 0;
        _currentLevel = menuLevel[level];
        Init();
    }

    public void Init() {
        SetActiveByLevel();
    }

    public void OnClick(int i)
    {
        switch (level) {
            case 0:
                {
                    foreach (Menu m in menu) {
                        m.panel.gameObject.SetActive(false);
                    }
                    menu[i].panel.gameObject.SetActive(true);
                }
                break;
            case 1:
                { 
                    //call onclick(string);
                }
                break;
            default:
                return;
        }
        level++;
        SetActiveByLevel();
    }

    public void OnClick(string target) {
        ManualObject output = null;
        Debug.Log(target);
        /*
        foreach (ManualObject mo in manualList)
        {
            if (mo.name.Equals(target)) {
                if (mo.exported == true) { 
                    //이미 얻음
                    
                    return;
                }
                mo.exported = true;
                output = mo;
                Debug.Log("found");
                break;
            }
        }
        */
        output = GetManual(target);
        if (output.exported) {
            return;
        }
        output.exported = true;

        if (output != null) {
            Animator anim = output.prefabs.GetComponent<Animator>();
            anim.SetBool("Close", false);
            anim.SetBool("Play", true);
            ManualScript script = output.prefabs.GetComponent<ManualScript>();
            StartCoroutine(script.DelayToClose());
        }
    }

    public void CloseManual(string str) {
        ManualObject mo = GetManual(str);
        mo.exported = false;
    }

    public ManualObject GetManual(string s) {
        ManualObject output = null;
        foreach (ManualObject mo in manualList)
        {
            if (mo.name.Equals(s)) {
                output = mo;
                break;
            }
        }

        return output;
    }

    public void OnRightClick(BaseEventData eventData) {
        PointerEventData pointer = eventData as PointerEventData;

        if (pointer.button == PointerEventData.InputButton.Right) {
            if (level > 0) {
                level--;
                SetActiveByLevel();
            }
        }
    }
    public void SetActiveByLevel() {
        for (int i = 0; i < menuLevel.Length; i++) {
            if (i != level)
            {
                menuLevel[i].gameObject.SetActive(false);
            }
            else
                menuLevel[i].gameObject.SetActive(true);
        }
    }
}
