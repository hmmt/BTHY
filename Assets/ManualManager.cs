using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ManualManager : MonoBehaviour {
    private static ManualManager _instance = null;
    public static ManualManager instance {
        get {
            return _instance;
        }
    }

    [System.Serializable]
    public class ManualObject
    {
        public GameObject prefabs;
        public string name;
        public bool exported = false;
    }

    public List<ManualObject> manualList;

    public void Awake() {
        _instance = this;
    }

    public ManualObject GetManual(int index) {
        if (index < 0 || index > manualList.Count) return null;
        return manualList[index];
    }

    public ManualObject GetManual(string name) {
        ManualObject output = null;
        foreach (ManualObject mo in manualList) {
            if (mo.name.Equals(name)) {
                output = mo;
                break;
            }
        }

        return output;
    }

    public ManualObject GetManual(GameObject target) {
        ManualObject output = null;
        foreach (ManualObject mo in manualList) {
            if (mo.prefabs.Equals(target)) { 
                output = mo;
                break;
            }
        }

        return output;
    }

    public void ExportManual() { 
        
    }


}
