using UnityEngine;
using System.Collections;

public class CollectionWindow : MonoBehaviour {

    private CreatureUnit creature;
    public GameObject backgroundDefault;
    public GameObject backgroundDiary;

    public UnityEngine.UI.Text descText;
    public UnityEngine.UI.Text observeText;

    [HideInInspector]
    public static CollectionWindow currentWindow = null;

    public void UpdateBg(string type)
    {
        if (type == "diary")
        {
            backgroundDefault.SetActive(false);
            backgroundDiary.SetActive(true);
        }
        else
        {
            backgroundDefault.SetActive(true);
            backgroundDiary.SetActive(false);
        }
    }

	public static void Create(CreatureUnit creature)
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }

        GameObject wndObject = Prefab.LoadPrefab("CollectionWindow");

        CollectionWindow wnd = wndObject.GetComponent<CollectionWindow>();

        wnd.creature = creature;

        wnd.descText.text = creature.metaInfo.desc;
        /*
        Vector2 pos = wnd.descText.GetComponent<RectTransform>().anchoredPosition;

        GameObject clone = (GameObject)Instantiate(wnd.descText.gameObject);
        clone.transform.SetParent(wnd.descText.transform.parent);
        clone.GetComponent<RectTransform>().anchoredPosition = pos + new Vector2(1, 0);
        clone.GetComponent<UnityEngine.UI.Text>().color = Color.black;

        clone = (GameObject)Instantiate(wnd.descText.gameObject);
        clone.transform.SetParent(wnd.descText.transform.parent);
        clone.GetComponent<RectTransform>().anchoredPosition = pos + new Vector2(-1, 0);
        clone.GetComponent<UnityEngine.UI.Text>().color = Color.black;

        clone = (GameObject)Instantiate(wnd.descText.gameObject);
        clone.transform.SetParent(wnd.descText.transform.parent);
        clone.GetComponent<RectTransform>().anchoredPosition = pos + new Vector2(0, -1);
        clone.GetComponent<UnityEngine.UI.Text>().color = Color.black;

        clone = (GameObject)Instantiate(wnd.descText.gameObject);
        clone.transform.SetParent(wnd.descText.transform.parent);
        clone.GetComponent<RectTransform>().anchoredPosition = pos + new Vector2(0, 1);
        clone.GetComponent<UnityEngine.UI.Text>().color = Color.black;
        */
        wnd.observeText.text = creature.metaInfo.observe;

        wnd.UpdateBg("default");

        currentWindow = wnd;
    }

    public void CloseWindow()
    {
        currentWindow = null;
        Destroy(gameObject);
    }
}
