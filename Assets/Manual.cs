using UnityEngine;
using System.Collections;

public class Manual : MonoBehaviour {

    public UnityEngine.UI.Image[] manualImage;
    public int currentIndex = 0;

    public Animator manualMove;


	// Use this for initialization
	void Awake () {

        currentIndex = 0;
        manualMove.SetBool("manualOut", true);

        for (int i = 0; i < manualImage.Length; i++)
        {
            manualImage[i].gameObject.SetActive(false);
        }

        manualImage[currentIndex].gameObject.SetActive(true);
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
	}

    public void goPage()
    {
        if (currentIndex < manualImage.Length)
        {
            manualImage[currentIndex].gameObject.SetActive(false);
            currentIndex += 1;
            if (currentIndex == manualImage.Length)
            {
                currentIndex = manualImage.Length - 1;
            }
            manualImage[currentIndex].gameObject.SetActive(true);
        }
    }

    public void backPage()
    {
        if (currentIndex > 0)
        {
            manualImage[currentIndex].gameObject.SetActive(false);
            currentIndex -= 1;
            manualImage[currentIndex].gameObject.SetActive(true);
        }
    }

    public void manualAnim()
    {
        if (!manualMove.GetBool("manualOut"))
        {
            manualMove.SetBool("manualOut",true);
        }
        else
        {
            manualMove.SetBool("manualOut", false);
        }
    }
}
