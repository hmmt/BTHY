using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;

[ExecuteInEditMode]
public class ToolMapRoot : MonoBehaviour {


	public void SaveMap()
	{
		string path = EditorUtility.SaveFilePanel(
			"save File",
			"",
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

				XmlElement passageNode = doc.CreateElement ("node_group");

				XmlAttribute passageAttrX =  doc.CreateAttribute ("x");
				XmlAttribute passageAttrY = doc.CreateAttribute ("y");
				XmlAttribute passageAttrSrc = doc.CreateAttribute ("src");

				passageAttrX.InnerText = x.ToString ();
				passageAttrY.InnerText = y.ToString ();
				passageAttrSrc.InnerXml = src;

				passageNode.Attributes.Append (passageAttrX);
				passageNode.Attributes.Append (passageAttrY);
				if (src != "") {
					passageNode.Attributes.Append (passageAttrSrc);
				}

				sefiraNode.AppendChild (passageNode);

				foreach (ToolMapNode mapNode in passage.GetComponentsInChildren<ToolMapNode>())
				{
					while (true) {
						string nodeId = mapNode.id;
						float nodeX = mapNode.transform.position.x;
						float nodeY = mapNode.transform.position.y;
						string nodeType = mapNode.type;

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

						nodeAttrId.InnerText = nodeId;
						nodeAttrX.InnerText = nodeX.ToString ();
						nodeAttrY.InnerText = nodeY.ToString ();
						nodeAttrType.InnerText = nodeType;

						mapNodeElement.Attributes.Append (nodeAttrId);
						mapNodeElement.Attributes.Append (nodeAttrX);
						mapNodeElement.Attributes.Append (nodeAttrY);
						if(nodeType != "")
							mapNodeElement.Attributes.Append (nodeAttrType);

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
	}
	public void LoadMap()
	{
		string path = EditorUtility.OpenFilePanel(
			"Load File",
			"",
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



					foreach (XmlNode node in nodeGroup.ChildNodes)
					{
						string id = node.Attributes.GetNamedItem("id").InnerText;
						float x = float.Parse(node.Attributes.GetNamedItem("x").InnerText);
						float y = float.Parse(node.Attributes.GetNamedItem("y").InnerText);

						XmlNode typeNode = node.Attributes.GetNamedItem("type");

						ToolMapNode newMapNode = ToolMapNode.AddMapNode(new Vector3(x, y, 0), passage);
						newMapNode.id = id;

						if (typeNode != null) {
							newMapNode.type = typeNode.InnerText;
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
	}
}
