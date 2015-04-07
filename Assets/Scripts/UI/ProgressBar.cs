using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {

	private GameObject bar;

	void Awake()
	{
		Transform child = transform.FindChild ("Bar");
		bar = child.gameObject;

		SetVisible (false);
	}

	public void SetVisible(bool b)
	{
		bar.SetActive(b);
	}

	public void SetRate(float rate)
	{
		bar.transform.localScale = new Vector3(rate, 1, 1);
	}
}
