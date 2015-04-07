/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))	]
public class CreatureRoom : MonoBehaviour {

	private static CreatureRoom _instance;
	public static CreatureRoom instance
	{
		get
		{
			return _instance;
		}
	}

	private Tile[] mapTiles = new Tile[]{};
	private float tileSize = 0.5f;
	private int width = 0;
	private int height = 0;

	private Vector2[] roamingPoints = new Vector2[]{new Vector2(0,0)};

	void Awake()
	{
		_instance = this;
	}

	void Start ()
	{
		LoadTilePathData ();
		UpdateTileMapMesh ();
		LoadMapData ();
	}

	void UpdateTileMapMesh()
	{
		Vector3[] vertices = new Vector3[mapTiles.Length*4];
		Vector2[] uv = new Vector2[mapTiles.Length*4];
		int[] triangles = new int[mapTiles.Length * 6];

		int i = 0;
		foreach(Tile tile in mapTiles)
		{
			int x = tile.x;
			int y = - tile.y - 1;
			vertices[i*4  ] = new Vector3(x*tileSize, y*tileSize, 0);
			vertices[i*4+1] = new Vector3((x+1)*tileSize, y*tileSize, 0);
			vertices[i*4+2] = new Vector3((x+1)*tileSize, (y+1)*tileSize, 0);
			vertices[i*4+3] = new Vector3(x*tileSize, (y+1)*tileSize, 0);

			uv[i*4] = new Vector2(0,0);
			uv[i*4+1] = new Vector2(1,0);
			uv[i*4+2] = new Vector2(1,1);
			uv[i*4+3] = new Vector2(0,1);

			triangles[i*6  ] = i*4 ;
			triangles[i*6+1] = i*4 + 2;
			triangles[i*6+2] = i*4 + 1;
			triangles[i*6+3] = i*4 ;
			triangles[i*6+4] = i*4 + 3;
			triangles[i*6+5] = i*4 + 2;

			//new Vector3(left, top, 0), new Vector3(right, top, 0), new Vector3(right, bottom, 0), new Vector3(left, bottom, 0)
			//mesh.uv = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)};
			i++;
		}

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		Texture2D tex = Resources.Load<Texture2D> ("Sprites/temp_tile");
		GetComponent<MeshRenderer>().material.mainTexture = tex;
	}

	public void LoadTilePathData()
	{
		StreamReader sr = new StreamReader (Application.dataPath + "/map/map.txt");
		
		ArrayList list = new ArrayList ();
		
		string line;
		int x=0, y=0;
		while ((line=sr.ReadLine()) != null)
		{
			string[] splited = line.Split(' ');
			
			x=0;
			foreach(string tile in splited)
			{
				int a = int.Parse(tile);
				if(a==1)
				{
					list.Add(new Tile(x,y));
				}
				x++;
				width = width < x ? x : width;
			}
			y++;
			height = height < y ? y : height;
		}
		
		mapTiles = (Tile[])list.ToArray (typeof(Tile));
		
		sr.Close ();
	}

	public void LoadMapData()
	{
		StreamReader sr = new StreamReader (Application.dataPath + "/map/MapInfo.xml");

		string text = sr.ReadToEnd ();
		sr.Close ();
		XmlDocument doc = new XmlDocument ();
		doc.LoadXml (text);

		XmlNodeList nodes = doc.SelectNodes("map_info/level1/creature");

		foreach(XmlNode node in nodes)
		{
			//Debug.Log(node.Attributes.GetNamedItem("x").InnerText);
		}

		XmlNodeList roamings = doc.SelectNodes("map_info/level1/roaming_point");

		List<Vector2> temp = new List<Vector2>();

		foreach(XmlNode node in roamings)
		{
			int x = int.Parse(node.Attributes.GetNamedItem("x").InnerText);
			int y = int.Parse(node.Attributes.GetNamedItem("y").InnerText);

			temp.Add(new Vector2(x, y));
		}

		roamingPoints = temp.ToArray ();
	}

	public Vector2 GetNearRoamingPoint(Vector2 pos)
	{
		Vector2 selected = new Vector2 (0, 0);
		float minDistance = float.MaxValue;
		foreach(Vector2 point in roamingPoints)
		{
			float dis = Vector2.Distance(point, pos);
			if(dis < minDistance)
			{
				minDistance = dis;
				selected = point;
			}
		}
		return selected;
	}

	public Vector2 GetRandomRoamingPoint()
	{
		return roamingPoints[Random.Range(0, roamingPoints.Length-1)];
	}

	public Vector2 TileToWorld(int x, int y)
	{
		return new Vector2(x*tileSize + tileSize/2f + transform.position.x,
		                   -y*tileSize - tileSize/2f + transform.position.y);
	}

	public void WorldToTile(Vector2 pos, out int x, out int y)
	{
		x = (int)Mathf.Floor((pos.x-transform.position.x)/tileSize);
		y = (int)Mathf.Floor(-(pos.y-transform.position.y)/tileSize);
	}

	public int[,] GetTileMap()
	{
		int[,] output = new int[height, width];

		for(int i=0; i<height; i++)
		{
			for(int j=0; j<width; j++)
			{
				output[i,j] = 0;
			}
		}

		foreach(Tile tile in mapTiles)
		{
			output[tile.y, tile.x] = 1;
		}

		return output;
	}
	
}

*/