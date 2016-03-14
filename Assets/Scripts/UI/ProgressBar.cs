using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {

	//private GameObject bar;
    private GameObject item;
    private float cnt;

	void Awake()
	{
		//Transform child = transform.FindChild ("Bar");
        item = Resources.Load<GameObject>("Prefabs/isloate_energy_item");
		//bar = child.gameObject;
        cnt = -0.05f;
		//SetVisible (false);
	}

	public void SetVisible(bool b)
	{
		//bar.SetActive(b);
        
	}

	public void SetRate(float rate)
    {
        if (rate > 0.9f) return;
        if (rate > cnt) {
            cnt += 0.1f;
            GameObject temp = Instantiate(Resources.Load<GameObject>("Prefabs/isloate_energy_item"));
            temp.transform.SetParent(this.transform);
            temp.transform.localScale = Vector3.one;
        }
		//bar.transform.localScale = new Vector3(rate, 1, 1);
	}
}
