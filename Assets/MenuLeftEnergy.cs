using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuLeftEnergy : MonoBehaviour {
    public Slider slider;
    public Slider both1, both2;
    public bool bothmode;
    public float oldvalue;
    //public UnityEngine.UI.Text menuLeftEnergyNum;

	// Use this for initialization
	void Start () {
        slider.minValue = 0;
        Debug.Log("Energy value_min max :"+slider.minValue + " " + slider.maxValue);
	}

    public void SetSlider(float value)
    {
        if (bothmode) {
            both1.maxValue = both2.maxValue = value/2;
            both1.value = both2.value = value / 2;
            oldvalue = value/2;
            return;
        }

        slider.maxValue = value;
        oldvalue = value;
        slider.value = value;
        Debug.Log(slider.minValue + " " + slider.maxValue);
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        //menuLeftEnergyNum.text = ""+EnergyModel.instance.GetLeftEnergy();
        //Debug.Log(EnergyModel.instance.GetLeftEnergy());
        if (!bothmode)
        {
            slider.value = EnergyModel.instance.GetLeftEnergy();
        }
        else { 
            both1.value = both2.value = EnergyModel.instance.GetLeftEnergy()/2;
        }
        /*oldvalue = EnergyModel.instance.GetLeftEnergy();
        if (!oldvalue.Equals(slider.value)) {
            StartCoroutine(AnimatedEnergy());
        }*/
	}

    IEnumerator AnimatedEnergy() {   
        /*
        while (oldvalue != slider.value) {
            yield return new WaitForSeconds(0.1f);
            slider.value -= 1f;
            Debug.Log(slider.value);
        }*/

        for (;oldvalue == slider.value ; ) {
            yield return new WaitForSeconds(0.1f);
            slider.value -= 1f;
            Debug.Log(slider.value);
        }
        
    }
}
