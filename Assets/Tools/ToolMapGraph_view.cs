using UnityEngine;
using System.Collections;
using System.Xml;

[RequireComponent(typeof(SefiraMapLayer))]
public class ToolMapGraph_view : MonoBehaviour {

	private MapGraph mapGraph;
	private SefiraMapLayer sefiraMapLayer;

	void Awake()
	{
		sefiraMapLayer = GetComponent<SefiraMapLayer> ();
		MapGraph.instance.LoadMap ();
	}

	public void LoadMap(XmlNode nodeRoot, XmlNode edgeRoot)
	{
		//mapGraph = new MapGraph ();
		//mapGraph.LoadMap (nodeRoot, edgeRoot);
		GameStaticDataLoader loader = new GameStaticDataLoader ();
		if (PassageObjectTypeList.instance.loaded == false)
			loader.LoadPassageData();
		MapGraph.instance.LoadMap(nodeRoot, edgeRoot);

		SefiraMapLayer l = GetComponent<SefiraMapLayer> ();
		l.OnInit ();
	}

	void Start()
	{
		AgentManager.instance.GetAgentList () [0].MoveToNode ("malkuth-0-4");
	}


}
