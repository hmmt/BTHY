using UnityEngine;
using System.Collections;

public class MapObjectModel : ObjectModelBase
{
	public MapObjectTypeInfo metaInfo;

	// mapObject가 있는 passage
	public PassageObjectModel passage;
	public int horrorPoint;
}

public class BloodMapObjectModel : MapObjectModel
{
	public Sprite bloodSprite;
}