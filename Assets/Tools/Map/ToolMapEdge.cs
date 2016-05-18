using UnityEngine;
using System.Collections;

public enum TOOL_MAP_EDGE
{
	door,
	road
}

[ExecuteInEditMode]
public class ToolMapEdge : MonoBehaviour {

	public string type;
	public ToolMapNode node1;
	public ToolMapNode node2;
	private SpriteRenderer renderer;


	void LateUpdate()
	{
		if (node1 == null || node2 == null)
		{
			if (renderer != null)
				renderer.enabled = false;
			return;
		}
		if (!Application.isPlaying)
		{
			transform.position = (node1.transform.position + node2.transform.position) / 2;

			float dist = Vector3.Distance (node1.transform.position, node2.transform.position);
			if (dist > 0)
				transform.rotation = Quaternion.LookRotation (node1.transform.position - node2.transform.position, Vector3.forward) * Quaternion.AngleAxis (90, Vector3.right);

			float length = (node1.transform.position - node2.transform.position).magnitude/8.0f;

			transform.localScale = new Vector3(length, length, length); 

			if (renderer == null)
				renderer = GetComponent<SpriteRenderer> ();

			if (renderer != null)
			{
				if (!node1.gameObject.activeInHierarchy || !node2.gameObject.activeInHierarchy)
					renderer.enabled = false;
				else
					renderer.enabled = true;
			}
		}
	}

	public static ToolMapEdge CreateEdge(ToolMapNode node1, ToolMapNode node2)
	{
		ToolMapRoot root = ToolMapGraph_Editor.GetMapRoot ();
		
		GameObject g = new GameObject ("MapEdge");
		g.transform.SetParent (root.transform);

		SpriteRenderer r = g.AddComponent<SpriteRenderer> ();
		#if UNITY_EDITOR
		r.sprite =  UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Tools/EdgeImage.psd", typeof(Sprite)) as Sprite;
		#endif
		ToolMapEdge edge = g.AddComponent<ToolMapEdge> ();

		edge.node1 = node1;
		edge.node2 = node2;

		edge.renderer = r;

		return edge;
	}
}
