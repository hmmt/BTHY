using UnityEngine;
using System.Collections;

public class EnergyUIView : MonoBehaviour, IObserver {

	void OnEnable()
	{
		Notice.instance.Observe ("UpdateEnergy", this);
	}

	void OnDisable()
	{
		Notice.instance.Remove ("UpdateEnergy", this);
	}

	public void SetEnergy(int energy)
	{
		GetComponent<TextMesh> ().text = "ENERGY : " + energy;
	}

	public void OnNotice(string notice, params object[] param)
	{
		if(notice == "UpdateEnergy")
		{
			SetEnergy(EnergyModel.instance.GetEnergy());
		}
	}
}
