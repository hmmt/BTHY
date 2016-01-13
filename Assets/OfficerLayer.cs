using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OfficerLayer : MonoBehaviour, IObserver {

    public static OfficerLayer currentLayer { private set; get; }
    public GameObject target;
    private List<OfficerUnit> officerList;

    private int zCount;

    void Awake() {
        currentLayer = this;
        officerList = new List<OfficerUnit>();
        zCount = 0;
    }

    void OnEnable() {
        Notice.instance.Observe(NoticeName.AddOfficer, this);
        Notice.instance.Observe(NoticeName.RemoveOfficer, this);
    }

    void OnDisalbe()
    {
        Notice.instance.Remove(NoticeName.AddOfficer, this);
        Notice.instance.Remove(NoticeName.RemoveOfficer, this);
    
    }

    public void Init() {
        
        ClearOfficer();
        foreach (OfficerModel model in OfficeManager.instance.GetOfficerList()) {
            //Debug.Log("in layer" + model.name);
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

        unit.zValue = -zCount;

        Vector3 unitScale = unit.transform.localScale;
        unitScale.z = 0.001f;
        unit.transform.localScale = unitScale;

        zCount = (zCount+ 1)% 1000;

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
}
