using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuLeftEnergy : MonoBehaviour {
    public Slider slider;
    public bool bothmode;
    public float oldvalue;
    //public UnityEngine.UI.Text menuLeftEnergyNum;

	// Use this for initialization
	void Start () {
        slider.minValue = 0;
       // Debug.Log("Energy value_min max :"+slider.minValue + " " + slider.maxValue);
	}

    public void SetSlider(float value)
    {
        if (bothmode) {
            oldvalue = value/2;
            return;
        }

        slider.maxValue = value;
        oldvalue = value;
        slider.value = slider.maxValue -  value;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        //menuLeftEnergyNum.text = ""+EnergyModel.instance.GetLeftEnergy();
        //Debug.Log(EnergyModel.instance.GetLeftEnergy());
        if (!bothmode)
        {
            slider.value =slider.maxValue -  EnergyModel.instance.GetLeftEnergy();
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
