using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OfficerLayer : MonoBehaviour, IObserver {
    

    public static OfficerLayer currentLayer { private set; get; }
    public GameObject target;

    private List<OfficerUnit> officerList;

    private int zCount;

    public List<WorkerSpriteSet> spriteList;
    
    void Awake() {
        currentLayer = this;
        officerList = new List<OfficerUnit>();
        zCount = 0;
    }

    public WorkerSpriteSet GetOfficerSpriteSet(Sefira targetSefira)
    {
        WorkerSpriteSet output = null;

        foreach (WorkerSpriteSet os in this.spriteList)
        {
            if (targetSefira.index == os.targetSefira) {
                output = os;
                break;
            }
        }
        return output;
    }

    void OnEnable() {
        Notice.instance.Observe(NoticeName.AddOfficer, this);
        Notice.instance.Observe(NoticeName.RemoveOfficer, this);
    }

    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.AddOfficer, this);
        Notice.instance.Remove(NoticeName.RemoveOfficer, this);
    
    }

    public void Init() {
        
        ClearOfficer();
        foreach (OfficerModel model in OfficerManager.instance.GetOfficerList()) {
            AddOfficer(model);
        }
    }

    public void AddOfficer(OfficerModel model) {
        GameObject newUnit = Instantiate(target);
        newUnit.transform.SetParent(transform, false);
        OfficerUnit unit = newUnit.GetComponent<OfficerUnit>();

        unit.GetComponent<SpriteRenderer>().sprite = null;
        unit.model = model;

        officerList.Add(unit);

        unit.zValue = -zCount * 0.001f;

        Vector3 unitScale = unit.transform.localScale;
        unitScale.z = 0.0005f;
        unit.transform.localScale = unitScale;

        zCount = (zCount + 1)% 1000;
        

        // 바뀔 수 있음
        /*
        if (unit.animTarget != null && hairList.Length > 0)
        {
            unit.animTarget.SetHair(hairList[Random.Range(0, hairList.Length)]);
            if (faceListTemp.Length > 0)
            {
                unit.animTarget.SetFace(faceListTemp[Random.Range(0, faceListTemp.Length)]);
            }
        }
        */

        if (unit.animTarget != null) {
            unit.animTarget.SetHair(unit.model.hairSprite);
            unit.animTarget.SetFace(unit.model.faceSprite);
        }
    }

    public void RemoveOfficer(OfficerModel model) {
        OfficerUnit unit = GetOfficer(model.instanceId);
        officerList.Remove(unit);
        Destroy(unit.gameObject);
    }

    public void ClearOfficer() {
        foreach (OfficerUnit unit in officerList) {
            Destroy(unit.gameObject);
            
        }
        officerList.Clear();
    }

    public OfficerUnit GetOfficer(long id) {
        foreach (OfficerUnit officer in officerList) {
            if (officer.model.instanceId == id) {
                return officer;
            }
        }

        return null;
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.AddOfficer) {
            foreach (object obj in param) {
                //Debug.Log("make?");
                AddOfficer((OfficerModel)obj);
            }
        }
        else if (notice == NoticeName.RemoveOfficer){
            foreach (object obj in param) {
                RemoveOfficer((OfficerModel)obj);
            }
        }
    }

    public void OnStageStart()
    {
        foreach (OfficerUnit unit in this.officerList)
        {
            unit.animTarget.SetSprite();
        }
    }
}
