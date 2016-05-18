using UnityEngine;
using System.Collections;

public class RedShoesAnim : CreatureAnimScript {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GetRedShoesAnim()
	{
		//AgentModel model;
		//model.GetMovableNode().GetPassage() == passage

		SetParameter ("Find", true);
		SetParameter ("Return", false);
	}

	public void ReturnRedShoesAnim()
	{
		SetParameter ("Find", false);
		SetParameter ("Return", true);
	}


}
