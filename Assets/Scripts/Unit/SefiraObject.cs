using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SefiraObject : MonoBehaviour {

    public string sefiraName;
    public GameObject sefira; // 직원 공간
    public GameObject sepfiraFog;

    public GameObject way1Fog;
    public GameObject way2Fog;
    public GameObject way3Fog;
    public GameObject way4Fog;

    public GameObject elevatorFog1;
    public GameObject elevatorFog2;

    public List<GameObject> fogs;

    private List<PassageObject> passageList;


    void Awake()
    {
        fogs = new List<GameObject>();
        passageList = new List<PassageObject>();
    }
    public void OnClick()
    {
        SelectSefiraAgentWindow.CreateWindow(sefira, sefiraName);
    }


    public void AddPassageObject(PassageObjectModel model)
    {
        //passageObj.
        GameObject passageObj = Prefab.LoadPrefab(model.GetMetaInfo().prefabSrc);

        PassageObject passageScript = passageObj.GetComponent<PassageObject>();

        
        if (passageScript.fogObject != null)
        {
            fogs.Add(passageScript.fogObject);
        }
        passageList.Add(passageScript);

        passageObj.transform.SetParent(transform);

        passageScript.Init(model);
    }

    public void AddMapObject(MapObjectModel mapObjModel)
    {
        foreach (PassageObject passage in passageList)
        {
            if (passage.model.GetId() == mapObjModel.passage.GetId())
            {
                passage.AddMapObject(mapObjModel);
            }
        }
    }

    public void ClosePassage(PassageObjectModel model)
    {
        Debug.Log("SefiraObject -> CALL ClosePassage");
        foreach (PassageObject passage in passageList)
        {
            if (passage.model.GetId() == model.GetId())
            {
                passage.CloseDoor();
            }
        }
    }

    public void OpenPassage(PassageObjectModel model)
    {
        foreach (PassageObject passage in passageList)
        {
            if (passage.model.GetId() == model.GetId())
            {
                passage.OpenDoor();
            }
        }
    }
}