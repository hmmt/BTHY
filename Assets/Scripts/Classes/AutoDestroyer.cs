using UnityEngine;
using System.Collections;

public class AutoDestroyer : MonoBehaviour {

	public float destroyTime = 1f;

	private float elapsedTime = 0;

	void FixedUpdate()
	{
		elapsedTime += Time.deltaTime;

		if (elapsedTime > destroyTime)
			Destroy (gameObject);
	}
}
