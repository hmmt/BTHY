using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextAppearEffect : MonoBehaviour {

	private float elapsedTime;
	private Vector2 worldPos;

	// Use this for initialization
	void Start () {
		elapsedTime = 0;
	}


	void FixedUpdate()
	{
		elapsedTime += Time.deltaTime;

		worldPos.y += Time.deltaTime/2;

		transform.position = Camera.main.WorldToScreenPoint ((Vector3)worldPos);

		if (elapsedTime > 6f)
			Destroy (gameObject);
	}

	public static TextAppearEffect Create(Vector2 pos, string text, Color color)
	{
		/*
		GameObject newEffect = new GameObject("Effect");

		Text textUI = newEffect.AddComponent<Text> ();

		textUI.text = text;
		textUI.color = color;

		newEffect.transform.parent = GameObject.Find ("Canvas").transform;
		TextAppearEffect effect = newEffect.AddComponent<TextAppearEffect> ();

		effect.worldPos = pos;
		*/

		GameObject newEffect = Prefab.LoadPrefab ("Text");
		newEffect.transform.SetParent(GameObject.FindGameObjectWithTag("GlobalCanvas").transform);

		Text textUI = newEffect.GetComponent<Text> ();

		textUI.text = text;
		textUI.color = color;

		TextAppearEffect effect = newEffect.GetComponent<TextAppearEffect> ();

		effect.worldPos = pos;
		newEffect.transform.position = Camera.main.WorldToScreenPoint ((Vector3)pos);

		return effect;
	}
}
