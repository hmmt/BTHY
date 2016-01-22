using UnityEngine;
using System.Collections;

public class Tile {
	public TileInfo info;
	public int x;
	public int y;

	public Tile()
	{
	}

	public Tile(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}
