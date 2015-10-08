using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SefiraObject : MonoBehaviour {

    public string sefiraName;
    public GameObject sefira;
    public GameObject sepfiraFog;

    public GameObject way1Fog;
    public GameObject way2Fog;
    public GameObject way3Fog;
    public GameObject way4Fog;

    public GameObject elevatorFog1;
    public GameObject elevatorFog2;


    public void OnClick()
    {
        SelectSefiraAgentWindow.CreateWindow(sefira, sefiraName);
    }
}