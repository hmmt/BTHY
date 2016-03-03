using UnityEngine;
using System.Collections;

public class WorkSlotListScript : MonoBehaviour {
    public Sprite colorSprite;

    public void OnEnable() {
        foreach (Transform child in this.transform) {
            WorkItemScript script = child.GetComponent<WorkItemScript>();
            script.SetBg(colorSprite);
        }
    }
}
