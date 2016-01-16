using UnityEngine;
using System.Collections;

public class SefiraCoreRoom : MonoBehaviour {

    SefiraObject sefira;

	// Use this for initialization
	void Start () {
        sefira = GetComponentInParent<SefiraObject>();
        if (sefira == null)
        {
            Debug.Log("sefira object not found");
        }
        else
        {
            sefira.sefiraCore = this;
        }
	}

    public void OnClick()
    {
        if (sefira != null)
        {
            sefira.OnClick();
        }
    }
}
