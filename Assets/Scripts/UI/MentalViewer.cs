using UnityEngine;
using System.Collections;

public class MentalViewer : MonoBehaviour {

    public SpriteRenderer mentalZeroRenderer;
    public SpriteRenderer mentalFullRenderer;

    //public UnityEngine.UI.Image mentalZeroRenderer;
    //public UnityEngine.UI.Image mentalFullRenderer;

    public void SetMentalRate(float rate)
	{
            Color color = mentalFullRenderer.color;
            color.a = rate;
            mentalFullRenderer.color = color;
        
	}
}
