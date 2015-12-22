﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))	]
public class RectDrawer : MonoBehaviour {

	public Texture2D texture;

	public float left = -1;
	public float top = 1;
	public float right = 1;
	public float bottom = -1;

	public bool fitTexture = true;

    public Vector2[] uvs = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };

    public Vector3 pos;
	
	void Start() {
		if(texture != null)
		{
			GetComponent<Renderer>().material.mainTexture = texture;
			GetComponent<Renderer>().material.shader = Shader.Find ("Sprites/Default");

			if(fitTexture)
			{
				right = texture.width/200f;
				left = -texture.width/200f;

				top = texture.height/200f;
				bottom = -texture.height/200f;
			}
		}

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = new Vector3[]{new Vector3(left, bottom, 0), new Vector3(right, bottom, 0), new Vector3(right, top, 0), new Vector3(left, top, 0)};
		//mesh.uv = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)};
        mesh.uv = uvs;
		mesh.triangles = new int[]{0,2,1, 0,3,2};


		mesh.colors = new Color[]{new Color(1, 1, 0, 1), new Color(1, 1, 1, 1), new Color(1, 1, 1, 1), new Color(1,1,1,1)};
	}
    /*
    void LateUpdate()
    {
        if ()
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh;

            float cx = pos.x * n / (n + pos.z);
            float cy = pos.y * n / (n + pos.z);

            mesh.vertices[0] = new Vector3(left, bottom, 0);
            mesh.vertices[1] = new Vector3(right, bottom, 0);
            mesh.vertices[2] = new Vector3(right, top, 0);
            mesh.vertices[3] = new Vector3(left, top, 0);
        }
    }*/
}
