using UnityEngine;
using System.Collections;

public class BloodMapObject : MonoBehaviour {

	public BloodMapObjectModel model;

	public Sprite[] bloodImgs;

	//public SpriteRenderer renderer;

	private SpriteRenderer renderer;

	private float elapsedTime;

	void Awake()
	{
		renderer = GetComponent<SpriteRenderer> ();

		if (renderer == null)
		{
			Debug.Log ("renderer not found");
		}
	}


	public void Init()
	{
		if (model == null) {
			Debug.Log ("BloodMapObject >> model not found");
		}
		if (renderer != null)
		{
			//renderer.sprite = bloodImgs [Random.Range (0, bloodImgs.Length)];
			renderer.sprite = model.bloodSprite;
		}
	}

	void FixedUpdate()
	{
		elapsedTime += Time.deltaTime;
	}
}
