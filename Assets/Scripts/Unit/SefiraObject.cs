using UnityEngine;
using System.Collections;

public class SefiraObject : MonoBehaviour {

    public string sefiraName;
    public GameObject sefira;

    public void OnClick()
    {
        SelectSefiraAgentWindow.CreateWindow(sefira, sefiraName);
    }
}