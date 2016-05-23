using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Xml;

[ExecuteInEditMode]
public class ToolMapRoot : MonoBehaviour {


	public bool showPassagePreb = true;
	private bool oldShowPassagePreb = true;

	void LateUpdate()
	{
		if (!Application.isPlaying)
		{

			if (showPassagePreb)
			{
				if (oldShowPassagePreb == false)
				{
					oldShowPassagePreb = showPassagePreb;

					ToolMapPassage[] passages = GetComponentsInChildren<ToolMapPassage> ();

					foreach (ToolMapPassage passage in passages) {
						if (passage.passageObject != null)
						{
							passage.passageObject.gameObject.SetActive (showPassagePreb);
						}
					}
				}
			}
			else
			{
				if (oldShowPassagePreb)
				{
					oldShowPassagePreb = showPassagePreb;

					ToolMapPassage[] passages = GetComponentsInChildren<ToolMapPassage> ();

					foreach (ToolMapPassage passage in passages) {
						if (passage.passageObject != null)
						{
							passage.passageObject.gameObject.SetActive (showPassagePreb);
						}
					}
				}
			}
		}
	}

	public void SaveMap()
	{
		#if UNITY_EDITOR
		string path = EditorUtility.SaveFilePanel(
			"save File",
			"Assets/Resources/xml",
			"MapGraph2.xml",
			"xml");
		XmlDocument doc = new XmlDocument();

		XmlDeclaration decl = doc.CreateXmlDeclaration ("1.0", "UTF-8", "");
		doc.AppendChild (decl);

		XmlElement mapGraph = doc.CreateElement ("map_graph");
		doc.AppendChild (mapGraph);

		XmlElement nodeList = doc.CreateElement ("node_list");
		mapGraph.AppendChild (nodeList);

		Dictionary<string, bool> dupChecker = new Dictionary<string, bool> ();

		//doc.Save (path);

		foreach (ToolMapSefiraArea sefira in GetComponentsInChildren<ToolMapSefiraArea>())
		{
			XmlElement sefiraNode = doc.CreateElement ("area");
			XmlAttribute sefiraAttrName = doc.CreateAttribute ("name");
			XmlAttribute sefiraAttrSub = doc.CreateAttribute ("sub");

			sefiraAttrName.InnerText = sefira.sefiraName;
			sefiraAttrSub.InnerText = sefira.sub.ToString();

			sefiraNode.Attributes.Append (sefiraAttrName);
			sefiraNode.Attributes.Append (sefiraAttrSub);

			nodeList.AppendChild (sefiraNode);

			foreach (ToolMapPassage passage in sefira.GetComponentsInChildren<ToolMapPassage>())
			{
				float x = passage.transform.position.x;
				float y = passage.transform.position.y;
				string src = passage.src;
				string passageType = passage.passageType;

				XmlElement passageNode = doc.CreateElement ("node_group");

				XmlAttribute passageAttrX =  doc.CreateAttribute ("x");
				XmlAttribute passageAttrY = doc.CreateAttribute ("y");
				XmlAttribute passageAttrSrc = doc.CreateAttribute ("src");
				XmlAttribute passageAttrType = doc.CreateAttribute ("passageType");

				passageAttrX.InnerText = x.ToString ();
				passageAttrY.InnerText = y.ToString ();
				passageAttrSrc.InnerXml = src;
				passageAttrType.InnerText = passageType;

				passageNode.Attributes.Append (passageAttrX);
				passageNode.Attributes.Append (passageAttrY);
				if (src != "") {
					passageNode.Attributes.Append (passageAttrSrc);
				}
				if(passageType != "")
				{
					passageNode.Attributes.Append(passageAttrType);
				}

				sefiraNode.AppendChild (passageNode);

				if(passage.ground != null && passage.ground.use)
				{
					XmlElement groundNode = doc.CreateElement ("ground");

					XmlAttribute heightAttr = doc.CreateAttribute ("height");

					heightAttr.InnerText = passage.ground.height.ToString();


					foreach(string spriteSrc in passage.ground.sprites)
					{
						XmlElement spriteNode = doc.CreateElement ("sprite");

						spriteNode.InnerText = spriteSrc;

						groundNode.AppendChild(spriteNode);
					}
					groundNode.Attributes.Append(heightAttr);
					passageNode.AppendChild(groundNode);
				}

				if(passage.wall != null && passage.wall.use)
				{
					XmlElement wallNode = doc.CreateElement ("wall");

					XmlAttribute heightAttr = doc.CreateAttribute ("height");

					heightAttr.InnerText = passage.wall.height.ToString();


					foreach(string spriteSrc in passage.wall.sprites)
					{
						XmlElement spriteNode = doc.CreateElement ("sprite");

						spriteNode.InnerText = spriteSrc;

						wallNode.AppendChild(spriteNode);
					}
					wallNode.Attributes.Append(heightAttr);
					passageNode.AppendChild(wallNode);
				}

				foreach (ToolMapNode mapNode in passage.GetComponentsInChildren<ToolMapNode>())
				{
					while (true) {
						string nodeId = mapNode.id;
						float nodeX = mapNode.transform.position.x;
						float nodeY = mapNode.transform.position.y;
						string nodeType = mapNode.type;
						float scaleFactor = mapNode.scaleFactor;
						TOOL_ELEVATOR_TYPE elevatorType = mapNode.elevatorType;

						if (dupChecker.ContainsKey (nodeId))
						{
							mapNode.id = mapNode.id + "(d)";
							continue;
						}
						dupChecker.Add (nodeId, true);

						XmlElement mapNodeElement = doc.CreateElement ("node");

						XmlAttribute nodeAttrId = doc.CreateAttribute ("id");
						XmlAttribute nodeAttrX = doc.CreateAttribute ("x");
						XmlAttribute nodeAttrY = doc.CreateAttribute ("y");
						XmlAttribute nodeAttrType = doc.CreateAttribute ("type");
						XmlAttribute nodeAttrScale = doc.CreateAttribute ("scale");
						XmlAttribute nodeAttrElevator = doc.CreateAttribute("elevator");

						nodeAttrId.InnerText = nodeId;
						nodeAttrX.InnerText = nodeX.ToString ();
						nodeAttrY.InnerText = nodeY.ToString ();
						nodeAttrType.InnerText = nodeType;
						nodeAttrScale.InnerText = scaleFactor.ToString ();

						mapNodeElement.Attributes.Append (nodeAttrId);
						mapNodeElement.Attributes.Append (nodeAttrX);
						mapNodeElement.Attributes.Append (nodeAttrY);
						if(nodeType != "")
							mapNodeElement.Attributes.Append (nodeAttrType);
						if (scaleFactor != 1.0f)
							mapNodeElement.Attributes.Append (nodeAttrScale);
						switch(elevatorType)
						{
						case TOOL_ELEVATOR_TYPE.LONG:
							nodeAttrElevator.InnerText = "long";
							mapNodeElement.Attributes.Append(nodeAttrElevator);
							break;
						case TOOL_ELEVATOR_TYPE.SHORT:
							nodeAttrElevator.InnerText = "short";
							mapNodeElement.Attributes.Append(nodeAttrElevator);
							break;
						default:
							break;
						}

						passageNode.AppendChild (mapNodeElement);
						break;
					}
				}
			}
		}

		XmlElement edgeList = doc.CreateElement ("edge_list");
		mapGraph.AppendChild (edgeList);
			
		foreach (ToolMapEdge edge in GetComponentsInChildren<ToolMapEdge>())
		{
			if(edge.node1 == null || edge.node2 == null ||
				!edge.node1.gameObject.activeInHierarchy ||
				!edge.node2.gameObject.activeInHierarchy)
			{
				continue;
			}
			
			string edgeType = edge.type;
			string node1 = edge.node1.id;
			string node2 = edge.node2.id;

			XmlElement edgeNode = doc.CreateElement ("edge");

			XmlAttribute edgeAttrNode1 =  doc.CreateAttribute ("node1");
			XmlAttribute edgeAttrNode2 = doc.CreateAttribute ("node2");
			XmlAttribute edgeAttrType = doc.CreateAttribute ("type");

			edgeAttrNode1.InnerText = node1;
			edgeAttrNode2.InnerText = node2;
			edgeAttrType.InnerText = edgeType;

			edgeNode.Attributes.Append (edgeAttrNode1);
			edgeNode.Attributes.Append (edgeAttrNode2);
			if(edgeType != "")
				edgeNode.Attributes.Append (edgeAttrType);

			edgeList.AppendChild (edgeNode);
		}

		Debug.Log ("Save!!?");
		doc.Save (path);
		#endif
	}
	public void LoadMap()
	{
		#if UNITY_EDITOR
		string path = EditorUtility.OpenFilePanel(
			"Load File",
			"Assets/Resources/xml",
			"xml");
		
		Dictionary<string, ToolMapNode> nodeDic = new Dictionary<string, ToolMapNode>();
		
		//TextAsset textAsset = Resources.Load<TextAsset>("xml/MapGraphT6");
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(System.IO.File.ReadAllText(path));


		XmlNode nodeXml = doc.SelectSingleNode ("/map_graph/node_list");
		XmlNode edgeXml = doc.SelectSingleNode ("/map_graph/edge_list");

		//LoadMap (nodeXml, edgeXml);

		int groupCount = 1;

		/*
        XmlDocument doc = new XmlDocument();
		doc.LoadXml(xmlText);
		*/

		XmlNodeList areaNodes = nodeXml.SelectNodes("area");

		foreach (XmlNode areaNode in areaNodes)
		{
			//MapSefiraArea mapArea = new MapSefiraArea();
			ToolMapSefiraArea mapSefira;

			List<MapNode> additionalSefira = new List<MapNode>();
			string areaName = areaNode.Attributes.GetNamedItem("name").InnerText;
			//mapArea.sefiraName = areaName;

			mapSefira = ToolMapSefiraArea.AddSefiraArea (areaName);

			int max = int.Parse(areaNode.Attributes.GetNamedItem("sub").InnerText);

			mapSefira.sub = max;

			foreach (XmlNode nodeGroup in areaNode.ChildNodes)
			{
				if (nodeGroup.Name == "node_group") {
					string groupName = "group@" + groupCount;
					groupCount++;


					XmlAttributeCollection attrs = nodeGroup.Attributes;
					XmlNode passageSrcNode = attrs.GetNamedItem ("src");
					XmlNode passageTypeIdNode = attrs.GetNamedItem ("typeId");
					XmlNode passageXNode = attrs.GetNamedItem ("x");
					XmlNode passageYNode = attrs.GetNamedItem ("y");
					XmlNode passageTypeNode = attrs.GetNamedItem ("passageType");

					Vector3 passagePos = new Vector3 (0, 0, 0);
					/*
					PassageObjectModel passage = null;
					if (passageTypeIdNode != null)
					{
						long passageTypeId = long.Parse(passageTypeIdNode.InnerText);
						float x = 0, y = 0;
						if (passageXNode != null) x = float.Parse(passageXNode.InnerText);
						if (passageYNode != null) y = float.Parse(passageYNode.InnerText);
						passage = new PassageObjectModel(groupName, areaName, PassageObjectTypeList.instance.GetData(passageTypeId));
						passage.position = new Vector3(x, y, 0);
					}
					*/

					//long passageTypeId = long.Parse(passageTypeIdNode.InnerText);
					string passageSrc = "";
					if (passageSrcNode != null)
					{
						passageSrc = passageSrcNode.InnerText;
					}
					if (passageXNode != null) passagePos.x = float.Parse(passageXNode.InnerText);
					if (passageYNode != null) passagePos.y = float.Parse(passageYNode.InnerText);

					ToolMapPassage passage = ToolMapPassage.AddPassage (mapSefira);
					passage.transform.position = passagePos;
					//passage.typeId = passageTypeId;
					passage.src = passageSrc;

					if(passageTypeNode != null) passage.passageType = passageTypeNode.InnerText;

					XmlNode groundNode = nodeGroup.SelectSingleNode("ground");
					XmlNode wallNode = nodeGroup.SelectSingleNode("wall");

					if (groundNode != null) {
						ToolMapPassage_bloodPoint groundInfo = new ToolMapPassage_bloodPoint();

						XmlNode groundHeight = groundNode.Attributes.GetNamedItem ("height");
						if (groundHeight != null)
							groundInfo.height = float.Parse (groundHeight.InnerText);

						foreach (XmlNode groundSprNode in groundNode.SelectNodes("sprite")) {
							groundInfo.sprites.Add(groundSprNode.InnerText);
						}

						groundInfo.use = true;
						passage.ground = groundInfo;
					}

					if (wallNode != null) {
						ToolMapPassage_bloodPoint wallInfo = new ToolMapPassage_bloodPoint();

						XmlNode wallHeight = groundNode.Attributes.GetNamedItem ("height");
						if (wallHeight != null)
							wallInfo.height = float.Parse (wallHeight.InnerText);

						foreach (XmlNode groundSprNode in groundNode.SelectNodes("sprite")) {
							wallInfo.sprites.Add(groundSprNode.InnerText);
						}

						wallInfo.use = true;
						passage.wall = wallInfo;
					}

					foreach (XmlNode node in nodeGroup.SelectNodes("node"))
					{
						string id = node.Attributes.GetNamedItem("id").InnerText;
						float x = float.Parse(node.Attributes.GetNamedItem("x").InnerText);
						float y = float.Parse(node.Attributes.GetNamedItem("y").InnerText);
						XmlNode scaleFactorNode = node.Attributes.GetNamedItem ("scale");

						XmlNode typeNode = node.Attributes.GetNamedItem("type");

						ToolMapNode newMapNode = ToolMapNode.AddMapNode(new Vector3(x, y, 0), passage);
						newMapNode.id = id;

						if (scaleFactorNode != null)
							newMapNode.scaleFactor = float.Parse (scaleFactorNode.InnerText);

						if (typeNode != null) {
							newMapNode.type = typeNode.InnerText;
						}

						XmlNode elevatorAttr = node.Attributes.GetNamedItem ("elevator");
						if (elevatorAttr != null) {
							//newMapNode.isElevator = true;

							string elevatorTypeStr = elevatorAttr.InnerText;
							switch(elevatorTypeStr)
							{
							case "short":
								newMapNode.elevatorType = TOOL_ELEVATOR_TYPE.SHORT;
								break;
							default:
								newMapNode.elevatorType = TOOL_ELEVATOR_TYPE.LONG;
								break;
							}
						}


						/*
						MapNode newMapNode = new MapNode(id, new Vector2(x, y), areaName, passage);

						newMapNode.activate = false;

						nodeDic.Add(id, newMapNode);
						*/

						nodeDic.Add (id, newMapNode);

						/*
						MapNode optionalNode = null;
						XmlNodeList optionList = node.SelectNodes("option");
						int doorCount = 1;
						foreach (XmlNode optionNode in optionList)
						{
							if (optionNode.InnerText == "room")
							{
							}
						}
						XmlNode doorNode = node.SelectSingleNode("door");
						if (doorNode != null)
						{
							string doorId = passage.GetId() + "@" + doorCount;
							newMapNode.SetClosable(true);
							DoorObjectModel door = new DoorObjectModel(doorId, doorNode.InnerText, passage, newMapNode);
							door.position = new Vector3(newMapNode.GetPosition().x,
								newMapNode.GetPosition().y, -0.01f);
							passage.AddDoor(door);
							newMapNode.SetDoor(door);
							door.Close();
						}

						if(passage != null)
							passage.AddNode(newMapNode);
						mapArea.AddNode(newMapNode);
						if (optionalNode != null)
							mapArea.AddNode (optionalNode);
						*/
					}
					/*
					if (passage != null)
						passageDic.Add(groupName, passage);
					*/
				}
				else if(nodeGroup.Name == "#comment")
				{
					// skip
				}
				else
				{
					Debug.Log("this is not node_group >>> "+nodeGroup.Name);
				}
			}
		}
		XmlNodeList nodes = edgeXml.SelectNodes("edge");

		//List<MapEdge> edgeList = new List<MapEdge>();

		foreach (XmlNode node in nodes)
		{
			string node1Id = node.Attributes.GetNamedItem("node1").InnerText;
			string node2Id = node.Attributes.GetNamedItem("node2").InnerText;

			string type = node.Attributes.GetNamedItem("type").InnerText;

			ToolMapNode node1, node2;

			if (nodeDic.TryGetValue(node1Id, out node1) == false ||
				nodeDic.TryGetValue(node2Id, out node2) == false)
			{
				Debug.Log("cannot create edge - (" + node1Id + ", " + node2Id + ")");
				continue;
			}

			/*
				MapNode node1, node2;

				if (nodeDic.TryGetValue(node1Id, out node1) == false ||
					nodeDic.TryGetValue(node2Id, out node2) == false)
				{
					Debug.Log("cannot create edge - (" + node1Id + ", " + node2Id + ")");
					continue;
				}
				*/

			XmlNode costNode = node.Attributes.GetNamedItem("cost");

			ToolMapEdge edge;
			edge = ToolMapEdge.CreateEdge (node1, node2);
			edge.type = type;

			/*
				MapEdge edge;
				if (costNode != null)
				{
					edge = new MapEdge(node1, node2, type, float.Parse(costNode.InnerText));
				}
				else
				{
					edge = new MapEdge(node1, node2, type);
				}
				edgeList.Add(edge);
				*/


			node1.AddEdge(edge);
			node2.AddEdge(edge);
		}
		#endif
	}
}
