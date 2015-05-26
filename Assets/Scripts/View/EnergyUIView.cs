using UnityEngine;
using System.Collections;

public class EnergyUIView : MonoBehaviour, IObserver {

	public UnityEngine.UI.Image mustEnergyGage;
	public UnityEngine.UI.Image leftEnergyGage;
	public UnityEngine.UI.Image chargeEnergyGage;
	public UnityEngine.UI.Image leftChargeEnergyGage;

	public UnityEngine.UI.Text mustEnergyNum;
	public UnityEngine.UI.Text leftEnergyNum;

    public Animator energyView;
  
	private float mustFillEnergy = 100;
	private float leftFillEnergy = 400;

	private float chargeTick=0;

    public void openSettings()
    {
        Debug.Log("UI버튼");

        if(energyView.GetBool("isHidden"))
            energyView.SetBool("isHidden",false);
        else
            energyView.SetBool("isHidden", true);
    }

	void Start()
	{
		SetEnergy (0);
	}

	void OnEnable()
	{
		Notice.instance.Observe ("UpdateEnergy", this);
	}

	void OnDisable()
	{
		Notice.instance.Remove ("UpdateEnergy", this);
	}

	public void SetEnergy(float energy)
	{
        //energy = energy / 
        mustFillEnergy = StageTypeInfo.instnace.GetEnergyNeed(PlayerModel.instnace.GetDay());
		
		float leftChargeEnergy = energy - mustFillEnergy;

		if(energy>mustFillEnergy)
		{
			energy = (int)mustFillEnergy;
		}

		if(leftChargeEnergy > leftFillEnergy)
		{
			leftChargeEnergy = (int)leftFillEnergy;
		}

		chargeEnergyGage.GetComponent<RectTransform>().localScale = new Vector3(Mathf.Clamp(energy/mustFillEnergy,0,1),1,1);

		leftChargeEnergyGage.GetComponent<RectTransform>().localScale = new Vector3(Mathf.Clamp(leftChargeEnergy/leftFillEnergy,0,1),1,1);

		mustEnergyNum.text = (int)energy+" / "+ mustFillEnergy;

		if(leftChargeEnergy <= 0)
			leftEnergyNum.text = "0 / "+ leftFillEnergy;

		else
			leftEnergyNum.text = (int)leftChargeEnergy+" / "+leftFillEnergy;
	}


	public void OnNotice(string notice, params object[] param)
	{
		if(notice == "UpdateEnergy")
		{
			SetEnergy(EnergyModel.instance.GetEnergy());
		}
	}
}
