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
        public bool moving = false;
        public float movingTime;
    }

    public RectTransform from;
    public RectTransform exportArea;
    public RectTransform parent;

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

    public void ExportManual(ManualObject target) {
        if (target.exported == true)
            return;
        else
            target.exported = true;

        StartCoroutine(ManualMovement(target, exportArea, 100));

    }

    /// <summary>
    /// target, dest
    /// unit => how many make routines
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dest"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public IEnumerator ManualMovement(ManualObject target, RectTransform dest, int unit) {
        if (target.moving) yield return null;
        target.moving = true;
        RectTransform rect = target.prefabs.GetComponent<RectTransform>();
        //rect.SetParent(from);
        //rect.localPosition = Vector2.zero;

        Vector2 directionVector2 = dest.anchoredPosition - rect.anchoredPosition;
        Debug.Log(directionVector2);

        int cnt = 0;
        float unitTime = 1 / (float)unit;
        float unitSize = directionVector2.magnitude * unitTime;
        Debug.Log(unitSize + " // " + unitTime + " // " + unit);
        
        Vector3 dir = directionVector2 * unitTime;

        while (cnt < (unit-30)) {
            //Debug.Log(cnt);
            rect.Translate(dir);
            cnt++;
            yield return new WaitForSeconds(unitTime);
        }
        //rect.SetParent(parent);
        target.moving = false;
    }
    
}
