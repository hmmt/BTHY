using UnityEngine;
using System.Collections.Generic;

public class AgentAnim : MonoBehaviour {

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
	
	
	void Awake()
	{
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
}
