using UnityEngine;
using System.Collections;

public class CollectionWindow : MonoBehaviour {

    private CreatureModel creature;
    public GameObject backgroundDefault;
    public GameObject backgroundDiary;

    public UnityEngine.UI.Text descText;
    public UnityEngine.UI.Text observeText;

    //public UnityEngine.UI.Text observeSubText;

	public UnityEngine.UI.Text code;
	public UnityEngine.UI.Text name;
	public UnityEngine.UI.Text dangerLevel;
	public UnityEngine.UI.Text attackType;
	public UnityEngine.UI.Text intLevel;
    public UnityEngine.UI.Text observePercent;

	public UnityEngine.UI.Image profImage;



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

    public void onClickObserveButton()
    {
        Debug.Log("Sibal Clicked");
        SelectObserveAgentWindow.CreateWindow(creature);
    }

	public static void Create(CreatureModel creature)
    {
        GameObject wndObject;
        CollectionWindow wnd;

        if (currentWindow != null)
        {
            wndObject = currentWindow.gameObject;
            //currentWindow.CloseWindow();
        }
        else
        {
            wndObject = Prefab.LoadPrefab("CollectionWindow");
        }
            wnd = wndObject.GetComponent<CollectionWindow>();

        wnd.creature = creature;

        wnd.descText.text = creature.metaInfo.desc;
        wnd.observeText.text = creature.GetObserveText();

		wnd.name.text = creature.metaInfo.name;
		wnd.code.text = creature.metaInfo.codeId;
		wnd.attackType.text = creature.metaInfo.attackType;
		wnd.intLevel.text = creature.metaInfo.intelligence.ToString();
		wnd.dangerLevel.text = creature.metaInfo.level.ToString();
        wnd.observePercent.text = (float)creature.observeProgress / creature.metaInfo.observeLevel * 100+"%";

		wnd.profImage.sprite = Resources.Load<Sprite>("Sprites/" + creature.metaInfo.imgsrc);

        wnd.UpdateBg("default");

        currentWindow = wnd;
    }

    public CreatureModel GetCreature()
    {
        return creature;
    }

    public void CloseWindow()
    {
        //currentWindow = null;
        GameObject.FindGameObjectWithTag("AnimCollectionController")
            .GetComponent<Animator>().SetBool("isTrue", true);
        //Destroy(gameObject);
    }
}
