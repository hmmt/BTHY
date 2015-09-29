﻿using UnityEngine;
using System.Collections;

public class AgentSlotPanel : MonoBehaviour {
    [HideInInspector]
    public AgentModel targetAgent;
	public UnityEngine.UI.Image agentIcon;
    public UnityEngine.UI.Image agentBody;
    public UnityEngine.UI.Image agentHair;
    public UnityEngine.UI.Image agentFace;
    public UnityEngine.UI.Text agentName;
    public UnityEngine.UI.Text agentHealth;
    public UnityEngine.UI.Text agentMental;
    public UnityEngine.UI.Text agentLevel;
	public UnityEngine.UI.Button skillButton1;
	public UnityEngine.UI.Button skillButton2;
	public UnityEngine.UI.Button skillButton3;
	public UnityEngine.UI.Button skillButton4;
}
