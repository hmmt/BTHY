using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Rect3DDrawer : MonoBehaviour {

    public Texture2D texture;

    //public bool fitTexture = true;

    public Vector3 origin1;
    public Vector3 origin2;
    public Vector3 origin3;
    public Vector3 origin4;

    void Start()
    {
        if (texture != null)
        {
            GetComponent<Renderer>().material.mainTexture = texture;
            GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");
            /*
            if (fitTexture)
            {
                right = texture.width / 200f;
                left = -texture.width / 200f;

                top = texture.height / 200f;
                bottom = -texture.height / 200f;
            }
            */
        }

        /*
        origin = new Vector3[] {
            new Vector3(left, bottom, 0),
            new Vector3(right, bottom, 1),
            new Vector3(right, top, 1),
            new Vector3(left, top, 0) };
        */

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = new Vector3[] {
            new Vector3(origin1.x, origin1.y, origin1.z),
            new Vector3(origin2.x, origin2.y, origin2.z),
            new Vector3(origin3.x, origin3.y, origin3.z),
            new Vector3(origin4.x, origin4.y, origin4.z) };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };


        mesh.colors = new Color[] { new Color(1, 1, 0, 1), new Color(1, 1, 1, 1), new Color(1, 1, 1, 1), new Color(1, 1, 1, 1) };
    }


    Vector3 CalCamera(Vector3 v)
    {
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 output = new Vector3();

        // out.x == px + v.x
        // px + v.x - cx
        // printx = out.x - cx
        if (v.z < -0.9f)
        {
            output.x = v.x;
            output.y = v.y;
            output.z = v.z;
            return output;
        }
        float x = v.x + transform.position.x;
        float y = v.y + transform.position.y;
        float z = v.x + transform.position.z;
        
        //output.x = v.x - (v.x + transform.position.x - cameraPos.x)/(v.z+1);
        //output.y = v.y - (v.y + transform.position.y - cameraPos.y)/(v.z+1);
        output.x = (x - cameraPos.x) / (z + 1) + cameraPos.x - transform.position.x;
        output.y = (y - cameraPos.y) / (z + 1) + cameraPos.y - transform.position.y;
        output.z = z - transform.position.z;

        return output;
    }

    void LateUpdate()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

		mesh.vertices = new Vector3[] {
			CalCamera(origin1),
			CalCamera(origin2),
			CalCamera(origin3),
			CalCamera(origin4)};

		mesh.RecalculateBounds ();
    }
}

