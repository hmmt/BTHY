using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AgentWorkSelectMenu : MonoBehaviour {
    public MenuScript menuScript;
    public ScrollRect scroll;

    public void Awake() {
        //OnClick();
    }

    public void OnClick() {
        RectTransform rect = menuScript.GetSelectedRect();
        scroll.content = rect;
    }

}
