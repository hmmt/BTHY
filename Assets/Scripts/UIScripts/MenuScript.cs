using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Menu {
    public Button button;
    public RectTransform panel;
}

public class MenuScript : MonoBehaviour {
    public Menu[] menus;

    private Menu selected;

    public void Start() {
        foreach (Menu m in menus) {
            m.panel.gameObject.SetActive(false);

        }
        selected = menus[0];
        SelectMenu();
    }

    public void SelectMenu() {
        foreach (Menu m in menus) {
            if (m.Equals(selected))
            {
                m.panel.gameObject.SetActive(true);
            }
            else {
                m.panel.gameObject.SetActive(false);
            }
        }
    }

    public void OnClick(Button target) {
        foreach (Menu m in menus) {
            if (target.Equals(m.button)) {
                selected = m;
            }
        }
        SelectMenu();
    }

    public RectTransform GetSelectedRect() {
        foreach (Menu m in menus) {
            if (m.Equals(selected)) {
                return m.panel;
            }
        }
        return null;
    }

    
}
