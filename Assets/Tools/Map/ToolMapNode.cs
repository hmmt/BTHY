using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public enum TOOL_MAP_NODE
{
	none,
	sefira,
	dept
}

[ExecuteInEditMode]
public class ToolMapNode : MonoBehaviour {
	
	public string id;
	public string areaName;
	public float scaleFactor = 1.0f;

	[HideInInspector]
	public ToolMapEdge[] edges;

	public string type = "";


	void OnDestroy()
	{
	}

	void Update()
	{
	}

	public void AddEdge(ToolMapEdge edge)
	{
		if (edges == null)
		{
			edges = new ToolMapEdge[]{ edge };
		}
		else
		{
			List<ToolMapEdge> _list = new List<ToolMapEdge> (edges);
			_list.Add (edge);
			edges = _list.ToArray ();
		}
	}

	public static ToolMapNode AddMapNode(Vector3 pos, ToolMapPassage passage)
	{
		ToolMapRoot root = ToolMapGraph_Editor.GetMapRoot ();

		GameObject g = new GameObject ("MapNode");

		SpriteRenderer r = g.AddComponent<SpriteRenderer> ();
		#if UNITY_EDITOR
		r.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Tools/NodeImage.psd", typeof(Sprite)) as Sprite;
		#endif

		ToolMapNode newNode = g.AddComponent<ToolMapNode> ();

		if(passage != null)
		{
			g.transform.SetParent (passage.transform);
			g.transform.position = pos;
		}
		else
		{
			g.transform.SetParent (root.transform);
			g.transform.position = pos;
		}

		return newNode;
	}

	public static void CreateMapNode(Vector3 pos)
	{
		#if UNITY_EDITOR
		ToolMapRoot root = ToolMapGraph_Editor.GetMapRoot ();

		GameObject g = new GameObject ("MapNode");

		SpriteRenderer r = g.AddComponent<SpriteRenderer> ();
		r.sprite = AssetDatabase.LoadAssetAtPath("Assets/Tools/NodeImage.psd", typeof(Sprite)) as Sprite;

		ToolMapNode newNode = g.AddComponent<ToolMapNode> ();

		g.transform.SetParent (root.transform);
		g.transform.position = pos;

		if (Selection.activeGameObject)
		{
			if(Selection.activeGameObject.GetComponent<ToolMapNode>())
			{
				ToolMapNode node = Selection.activeGameObject.GetComponent<ToolMapNode> ();

				ToolMapEdge.CreateEdge (node, newNode);
			}	
		}

		Selection.activeGameObject = newNode.gameObject;
		#endif
	}
}
