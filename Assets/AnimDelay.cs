using UnityEngine;
using System.Collections;

public class AnimDelay : MonoBehaviour {
	public Animator[] animList;
	public bool started = false;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.W)) {
			if (!started) {
				started = true;
				StartCoroutine(playDelay());
			}

		}
	}

	IEnumerator playDelay() {
		int max = animList.Length;
		for (int i = 0; i < max; i++) {
			animList[i].SetBool("Play", true);
			yield return new WaitForSeconds(1f);    
		}

	}
}