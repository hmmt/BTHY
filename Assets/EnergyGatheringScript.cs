using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnergyGatheringScript : MonoBehaviour {
    public Slider left;
    public Slider right;

    private float max;

    private bool gathered = false;
    public bool Gathered {
        get { return gathered; }
    }

	// Use this for initialization
	void Start () {
        Init();
	}

    public void Init() {
        gathered = false;
        left.value = 0;
        right.value = 0;
        left.maxValue = StageTypeInfo.instnace.GetEnergyNeed(PlayerModel.instance.GetDay());
        right.maxValue = left.maxValue;
        max = left.maxValue;
    }
	
	// Update is called once per frame
	void Update () {
        float value = EnergyModel.instance.GetEnergy();
        if (value < max)
        {
            gathered = false;
        }
        else {
            gathered = true;
        }
        if (!gathered)
        {
            //left
            left.value = EnergyModel.instance.GetEnergy();
        }
        else { 
            //right
            left.value = max;
            if (value - max > right.maxValue)
            {
                value = max;

            }
            else {
                right.value = value- max;
            }
            right.value = value;
        }
	}
}
