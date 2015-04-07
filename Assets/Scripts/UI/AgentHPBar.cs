using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentHPBar : MonoBehaviour {

	public Sprite barBackground;
	public Sprite barSprite;

	private int maxHP;
	private GameObject[] barList;

	void Start()
	{

	}

	public void SetMaxHP(int maxHP)
	{
		List<GameObject> list = new List<GameObject> ();

		float widthOrigin = barSprite.bounds.size.x;

		float scale = 1 / widthOrigin / maxHP;
		float width = widthOrigin * scale;

		this.maxHP = maxHP;

		for(int i=0; i<maxHP; i++)
		{
			GameObject child = new GameObject();
			child.AddComponent<SpriteRenderer>().sprite = barSprite;

			child.transform.SetParent(gameObject.transform);

			child.transform.localPosition = new Vector3(-0.5f + width/2 + width*i, 0, 0);
			child.transform.localScale = new Vector3(scale, 1, 1);

			list.Add(child);
		}

		barList = list.ToArray ();
	}

	public void SetCurrentHP(int hp)
	{
		if(barList == null)
		{
			return;
		}
		for(int i=0; i<barList.Length; i++)
		{
			if(i < hp)
			{
				barList[i].SetActive(true);
			}
			else
			{
				barList[i].SetActive(false);
			}
		}
	}
}
