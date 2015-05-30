using UnityEngine;
using System.Collections;

public class MenuLeftEnergy : MonoBehaviour {

    public UnityEngine.UI.Text menuLeftEnergyNum;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        menuLeftEnergyNum.text = ""+EnergyModel.instance.GetLeftEnergy();
	
	}
}
