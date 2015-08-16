using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextAppearNormalEffect : MonoBehaviour {

	private float elapsedTime = 0;
	private float goalTime = 2f;

	private Vector2 worldPos;
	private Text textUI;

	private Callback removeCallback = null;
	private GameObject target = null;
	private Vector2 offset = new Vector2(0,0);
	
	// Use this for initialization
	void Start () {
		elapsedTime = 0;
	}

	void OnDisable()
	{
		if(removeCallback != null)
		{
			removeCallback();
			removeCallback = null;
		}
	}
	void OnDestroy()
	{
		if(removeCallback != null)
		{
			removeCallback();
			removeCallback = null;
		}
	}

	private void UpdateState()
	{
		Vector3 cameraCenter = Camera.main.transform.position;
		
		float widthHalf = Camera.main.orthographicSize;
		float heightHalf = Camera.main.orthographicSize * Camera.main.aspect;

		Vector2 dist = (Vector2)cameraCenter - worldPos;
		
		float value = Mathf.Clamp(Mathf.Max(Mathf.Abs(dist.x) / widthHalf, Mathf.Abs(dist.y) / heightHalf) - 0.2f, 0, 1);
		
		Color newColor = textUI.color;
		newColor.a = 1 - value;
		textUI.color = newColor;
		
		/*
		Color newColor = textUI.color;
		newColor.a = 
		*/
		
		if(target != null)
		{
			transform.position = Camera.main.WorldToScreenPoint (target.transform.position + (Vector3)offset);
		}
		else
		{
			transform.position = Camera.main.WorldToScreenPoint ((Vector3)worldPos);
		}
	}
	
	void FixedUpdate()
	{
		elapsedTime += Time.deltaTime;

		UpdateState ();

		if (elapsedTime > goalTime)
			Destroy (gameObject);
	}
	
	public static TextAppearNormalEffect Create(Vector2 pos, string text, Color color)
	{
		
		GameObject newEffect = Prefab.LoadPrefab ("TextAppearNormalEffect");
		newEffect.transform.SetParent(GameObject.FindGameObjectWithTag("GlobalCanvas").transform);
		
		TextAppearNormalEffect effect = newEffect.GetComponent<TextAppearNormalEffect> ();

		effect.textUI = newEffect.GetComponent<Text> ();
		
		effect.textUI.text = text;
		effect.textUI.color = color;
		effect.worldPos = pos;

		newEffect.transform.position = Camera.main.WorldToScreenPoint ((Vector3)pos);

		effect.UpdateState ();
		
		return effect;
	}

	public static TextAppearNormalEffect Create(DestroyHandler handler, Vector2 offset, float goalTime, string text, Color color)
	{
		GameObject newEffect = Prefab.LoadPrefab ("TextAppearNormalEffect");
		newEffect.transform.SetParent(GameObject.FindGameObjectWithTag("GlobalCanvas").transform);
		
		TextAppearNormalEffect effect = newEffect.GetComponent<TextAppearNormalEffect> ();
		
		effect.textUI = newEffect.GetComponent<Text> ();
		
		effect.textUI.text = text;
		effect.textUI.color = color;
		effect.worldPos = (Vector2)handler.transform.position;
		
		newEffect.transform.position = Camera.main.WorldToScreenPoint (handler.transform.position);

		effect.removeCallback = handler.AddReceiver (effect.OnRemoveTarget);
		effect.target = handler.gameObject;
		effect.goalTime = goalTime;

		effect.offset = offset;

		effect.UpdateState ();
		
		return effect;
	}

    public bool IsSpeechOn()
    {
        if (textUI.IsActive())
            return true;
        else
            return false;
    }

	public void OnRemoveTarget()
	{
		Destroy (gameObject);
	}
}
