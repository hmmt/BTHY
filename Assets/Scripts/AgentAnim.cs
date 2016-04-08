using UnityEngine;
using System.Collections.Generic;

public class AgentAnim : MonoBehaviour {
	private class ParameterInfo
	{
		public enum ParameterType
		{
			INT,
			FLOAT,
			BOOL
		}
		
		public string name;
		public int ivalue;
		public bool bvalue;
		public int state;
		public float remainTimer;

		public ParameterType type;

		public ParameterInfo(string name, int value, int state)
		{
			this.name = name;
			this.ivalue = value;
			this.state = state;
			this.type = ParameterType.INT;
		}

		public ParameterInfo(string name, bool value, int state)
		{
			this.name = name;
			this.bvalue = value;
			this.state = state;
			this.type = ParameterType.BOOL;
		}

		public ParameterInfo(string name, int value, int state, float remainTimer)
		{
			this.name = name;
			this.ivalue = value;
			this.state = state;
			this.remainTimer = remainTimer;
			this.type = ParameterType.INT;
		}

		public ParameterInfo(string name, bool value, int state, float remainTimer)
		{
			this.name = name;
			this.bvalue = value;
			this.state = state;
			this.remainTimer = remainTimer;
			this.type = ParameterType.BOOL;
		}
	}

	private Animator animator;

	public Sprite[] faceSprites;

	public SpriteRenderer hair;
	public SpriteRenderer face;

	public SpriteRenderer body;
	
	public SpriteRenderer B_low_leg;
	public SpriteRenderer B_up_leg;
	
	public SpriteRenderer F_low_leg;
	public SpriteRenderer F_up_leg;
	
	public SpriteRenderer B_hand;
	public SpriteRenderer B_low_arm;
	public SpriteRenderer B_up_arm;
	
	public SpriteRenderer F_hand;
	public SpriteRenderer F_low_arm;
	public SpriteRenderer F_up_arm;
	
	//private Dictionary<string, SpriteRenderer> rendererTable;
	private Dictionary<string, SpriteRenderer> clothesRendererTable;

	private Stack<ParameterInfo> updatedParameters;
	private List<ParameterInfo> updatedParametersMoment;

	
	
	void Awake()
	{
		updatedParameters = new Stack<ParameterInfo> ();
		updatedParametersMoment = new List<ParameterInfo> ();

		animator = GetComponent<Animator> ();
		if (animator == null) {
			Debug.LogError("Animator is not found!");
		}

		clothesRendererTable = new Dictionary<string, SpriteRenderer>();
		
		clothesRendererTable.Add("body", body);
		
		clothesRendererTable.Add("B_low_leg", B_low_leg);
		clothesRendererTable.Add("B_up_leg", B_up_leg);
		
		clothesRendererTable.Add("F_low_leg", F_low_leg);
		clothesRendererTable.Add("F_up_leg", F_up_leg);
		
		//clothesRendererTable.Add("B_hand", B_hand);
		clothesRendererTable.Add("B_low_arm", B_low_arm);
		clothesRendererTable.Add("B_up_arm", B_up_arm);
		
		//clothesRendererTable.Add("F_hand", F_hand);
		clothesRendererTable.Add("f_low_arm", F_low_arm);
		clothesRendererTable.Add("F_up_arm", F_up_arm);
	}
	
	
	public void SetHair(Sprite sprite)
	{
		this.hair.sprite = sprite;
	}
	public void SetFace(Sprite sprite)
	{
		this.face.sprite = sprite;
	}
	public void SetClothes(Sprite[] sprites)
	{
		foreach (Sprite sprite in sprites)
		{
			SpriteRenderer renderer;
			if (clothesRendererTable.TryGetValue(sprite.name, out renderer))
			{
				renderer.sprite = sprite;
			}
		}
	}

	public void SetSpeed(float speed)
	{
		animator.speed = speed;
	}

    public void PlayMatchGirlDead()
    {
        animator.speed = 1;
        animator.SetBool("Dead", true);
    }

	public void SetParameterOnce(string pname, int value)
	{
		updatedParameters.Push (new ParameterInfo (pname, animator.GetInteger (pname), animator.GetAnimatorTransitionInfo(0).nameHash));
		animator.SetInteger (pname, value);
	}

	public void SetParameterOnce(string pname, bool value)
	{
		updatedParameters.Push (new ParameterInfo (pname, animator.GetBool (pname), animator.GetAnimatorTransitionInfo(0).nameHash));
		animator.SetBool (pname, value);
	}

	public void SetParameterForSecond(string pname, int value, float time)
	{
		updatedParametersMoment.Add (new ParameterInfo (pname, animator.GetInteger (pname), animator.GetAnimatorTransitionInfo(0).nameHash, time));
		animator.SetInteger (pname, value);
	}
	public void SetParameterForSecond(string pname, bool value, float time)
	{
		updatedParametersMoment.Add (new ParameterInfo (pname, animator.GetBool (pname), animator.GetAnimatorTransitionInfo(0).nameHash, time));
		animator.SetBool (pname, value);
	}

	void LateUpdate()
	{
		while (updatedParameters.Count > 0) {
			ParameterInfo info = updatedParameters.Pop ();
			if(info.type == ParameterInfo.ParameterType.INT)
				animator.SetInteger(info.name, info.ivalue);
			else
				animator.SetBool(info.name, info.bvalue);
		}

		List<ParameterInfo> rmList = new List<ParameterInfo> ();
		foreach (ParameterInfo info in updatedParametersMoment)
		{
			info.remainTimer -= Time.deltaTime;

			if (info.remainTimer <= 0)
			{
				if(info.type == ParameterInfo.ParameterType.INT)
					animator.SetInteger(info.name, info.ivalue);
				else
					animator.SetBool(info.name, info.bvalue);
				
				rmList.Add (info);
			}
		}

		foreach (ParameterInfo info in rmList) {
			updatedParametersMoment.Remove (info);
		}
	}
}
