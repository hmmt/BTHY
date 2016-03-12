using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Xml;

public class ToolMapGraph_Editor 
#if UNITY_EDITOR
	: EditorWindow 
#endif
{

	public static bool nodeCreation = false;
	#if UNITY_EDITOR
	[MenuItem("Examples/Load Map")]
	static void Apply()
	{
		string path = EditorUtility.OpenFilePanel(
			"Open File",
			"",
			"xml");

		XmlDocument doc = new XmlDocument();
		doc.LoadXml(System.IO.File.ReadAllText(path));

		XmlNode nodeXml = doc.SelectSingleNode ("/map_graph/node_list");
		XmlNode edgeXml = doc.SelectSingleNode ("/map_graph/edge_list");
		//MapGraph.instance.LoadMap (nodeXml, edgeXml);

		ToolMapGraph_view view = GameObject.FindObjectOfType<ToolMapGraph_view>();
		if (view == null)
		{
			GameObject g = new GameObject ();
			view = g.AddComponent<ToolMapGraph_view> ();
		}
		view.LoadMap (nodeXml, edgeXml);
	}

	[MenuItem ("Window/ToolMapGraph")]
	static void Init () 
	{
		ToolMapGraph_Editor window = (ToolMapGraph_Editor)EditorWindow.GetWindow (typeof (ToolMapGraph_Editor));
		window.Show();
	}

	[MenuItem("Window/SaveTest")]
	static void SaveTest()
	{
		GetMapRoot ().SaveMap ();
	}

	[MenuItem("Window/LoadTest")]
	static void LoadTest()
	{
		GetMapRoot ().LoadMap ();
	}

	[MenuItem("Window/MakeEdge")]
	static void MakeEdge()
	{
		GameObject[] gs = Selection.gameObjects;
		if (gs.Length >= 2) {
			if (gs [0].GetComponent<ToolMapNode> () && gs [1].GetComponent<ToolMapNode> ()) {
				ToolMapEdge edge;
				edge = ToolMapEdge.CreateEdge (gs [0].GetComponent<ToolMapNode> (), gs [1].GetComponent<ToolMapNode> ());
				edge.type = "door";
			}
		}
	}
	void OnGUI ()
	{
		GUILayout.Label(nodeCreation.ToString(), EditorStyles.boldLabel);

		if (!nodeCreation)
		{
			if (GUILayout.Button ("Node Make Mode"))
			{
				nodeCreation = true;
				//EditorPrefs.SetBool("Puppet2D_BoneCreation", BoneCreation);
			}
		}
		else
		{
			if (GUILayout.Button ("Node Make Mode Cancle"))
			{
				nodeCreation = false;
				//EditorPrefs.SetBool("Puppet2D_BoneCreation", BoneCreation);
			}
		}

		GUILayout.Button ("");
	}
	void OnFocus()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}
	void OnSceneGUI(SceneView sceneView)
	{
		
		Event e = Event.current;

		switch (e.type)
		{
		case EventType.keyDown:
			break;
		case EventType.MouseDown:
			{
				if (Event.current.button == 0)
				{
					if (nodeCreation)
					{
						Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
						int controlID = GUIUtility.GetControlID(FocusType.Passive);
						HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
						GameObject c = HandleUtility.PickGameObject(Event.current.mousePosition, true);

						ToolMapNode.CreateMapNode (worldRay.GetPoint (0));
					}
				}
				else if(Event.current.button == 1)
				{
					nodeCreation = false;
				}
			}
			break;
		}

	}
	#endif
	public static ToolMapRoot GetMapRoot()
	{
		ToolMapRoot root = Transform.FindObjectOfType<ToolMapRoot> ();

		if (root)
		{
			return root;
		}
		else
		{
			GameObject g = new GameObject("ToolMapRoot");
			#if UNITY_EDITOR
			Undo.RegisterCreatedObjectUndo (g, "Created ToolMapRoot");
			#endif

			root = g.AddComponent<ToolMapRoot>();

			return root ;
		}
	}
}
