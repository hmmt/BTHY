using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))	]
public class TextMessagePart : MonoBehaviour {

	void Start()
	{
		//Vector2 position = (Vector2)Camera.main.transform.position;
		Vector2 position = new Vector2 (0, 0);
		float height = Camera.main.orthographicSize;
		float width = height * Camera.main.aspect;

		float left = position.x - width;
		float top = position.y + height;
		float right = position.x + width;
		float bottom = position.y - height;

		bottom = top - (top - bottom) * 0.2f;

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = new Vector3[]{new Vector3(left, top, 0), new Vector3(right, top, 0), new Vector3(right, bottom, 0), new Vector3(left, bottom, 0)};
		mesh.uv = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)};
		mesh.triangles = new int[]{0,1,2, 0,2,3};

		Color color = new Color (0.5f, 1, 1, 1);

		mesh.colors = new Color[]{color,color,color,color};

	}
}
